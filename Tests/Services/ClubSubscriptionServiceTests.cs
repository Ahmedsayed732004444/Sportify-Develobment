using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Sportiva.Abstractions;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Enums;
using Sportiva.Mapping;
using Sportiva.Persistence;
using Sportiva.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sportiva.Tests.Services;

/// <summary>
/// Unit tests for ClubSubscriptionService.
/// Adheres strictly to xUnit, Moq, FluentAssertions, and AAA structure.
/// </summary>
public class ClubSubscriptionServiceTests
{
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<DatabaseFacade> _databaseMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;

    private readonly List<Club> _clubs;
    private readonly List<SubscriptionPlan> _plans;
    private readonly List<ClubSubscription> _subscriptions;
    private readonly List<SubscriptionPayment> _payments;

    private readonly ClubSubscriptionService _service;

    static ClubSubscriptionServiceTests()
    {
        // 3. Mapster Configuration: Register mappings in the global config.
        TypeAdapterConfig.GlobalSettings.Scan(typeof(MappingConfigurations).Assembly);
    }

    public ClubSubscriptionServiceTests()
    {
        _clubs = new List<Club>();
        _plans = new List<SubscriptionPlan>();
        _subscriptions = new List<ClubSubscription>();
        _payments = new List<SubscriptionPayment>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _contextMock = new Mock<ApplicationDbContext>(options);

        // 4. EF Core Mocking: Create mocked DbSets supporting IAsyncEnumerable
        var clubsDbSet = CreateDbSetMock(_clubs);
        var plansDbSet = CreateDbSetMock(_plans);
        var subscriptionsDbSet = CreateDbSetMock(_subscriptions);
        var paymentsDbSet = CreateDbSetMock(_payments);

        // Set the non-virtual DbSet properties using their public setters
        _contextMock.Object.Clubs = clubsDbSet.Object;
        _contextMock.Object.SubscriptionPlans = plansDbSet.Object;
        _contextMock.Object.ClubSubscriptions = subscriptionsDbSet.Object;
        _contextMock.Object.SubscriptionPayments = paymentsDbSet.Object;

        // Mock DatabaseFacade for transactions
        _transactionMock = new Mock<IDbContextTransaction>();
        _databaseMock = new Mock<DatabaseFacade>(_contextMock.Object);
        _databaseMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

        _contextMock.Setup(c => c.Database).Returns(_databaseMock.Object);

        // Mock SaveChangesAsync
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _service = new ClubSubscriptionService(_contextMock.Object);
    }

    #region GetActiveSubscriptionAsync Tests

    [Fact]
    public async Task GetActiveSubscriptionAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var userId = "";
        var clubId = "club-1";

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_WithNullClubId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = " ";

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.ClubId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-not-exist";

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_WhenCallerNotClubOwner_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        _clubs.Add(new Club { Id = clubId, OwnerId = "another-user", IsDeleted = false });

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_WhenNoActiveSubscriptionExists_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        _clubs.Add(club);

        // A subscription exists but it's expired/deleted or cancelled, so not active
        _subscriptions.Add(new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            Status = SubscriptionStatus.Cancelled,
            IsDeleted = false
        });

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Subscription.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_WhenActiveSubscriptionExists_ReturnsSuccess()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Gold Plan", MonthlyPrice = 100m, IsActive = true };
        
        var subscription = new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            PlanId = planId,
            Status = SubscriptionStatus.Active,
            IsDeleted = false,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 100m,
            Plan = plan,
            Club = club
        };

        _clubs.Add(club);
        _plans.Add(plan);
        _subscriptions.Add(subscription);

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.SubscriptionId.Should().Be(subscription.Id);
        result.Value.IsActive.Should().BeTrue();
    }

    #endregion

    #region GetSubscriptionHistoryAsync Tests

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var userId = "";
        var clubId = "club-1";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithNullClubId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = " ";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.ClubId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-not-exist";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WhenCallerNotClubOwner_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        _clubs.Add(new Club { Id = clubId, OwnerId = "another-user", IsDeleted = false });
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithInvalidPageNumber_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        _clubs.Add(club);

        var filters = new RequestFilters { PageNumber = 0, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.Description.Should().Contain("PageNumber");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithInvalidPageSize_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        _clubs.Add(club);

        var filters = new RequestFilters { PageNumber = 1, PageSize = 0 }; // Invalid: < 1

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.Description.Should().Contain("PageSize");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WhenValidRequest_ReturnsPaginatedList()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Plan", MonthlyPrice = 100m, IsActive = true };

        var sub1 = new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            PlanId = planId,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            CreatedAt = DateTime.UtcNow,
            Club = club,
            Plan = plan
        };

        _clubs.Add(club);
        _plans.Add(plan);
        _subscriptions.Add(sub1);

        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(1);
        result.Value.TotalCount.Should().Be(1);
    }

    #endregion

    #region SubscribeAsync Tests

    [Fact]
    public async Task SubscribeAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var userId = "";
        var clubId = "club-1";
        var request = new CreateClubSubscriptionRequest(clubId, "plan-1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WithNullClubId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = " ";
        var request = new CreateClubSubscriptionRequest(clubId, "plan-1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.ClubId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-not-exist";
        var request = new CreateClubSubscriptionRequest(clubId, "plan-1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task SubscribeAsync_WhenCallerNotClubOwner_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        _clubs.Add(new Club { Id = clubId, OwnerId = "another-user", IsDeleted = false });
        var request = new CreateClubSubscriptionRequest(clubId, "plan-1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task SubscribeAsync_WhenClubIsInactive_ReturnsClubInactive()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = false, IsDeleted = false };
        _clubs.Add(club);
        var request = new CreateClubSubscriptionRequest(clubId, "plan-1", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task SubscribeAsync_WithNullOrEmptyPlanId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        _clubs.Add(club);
        var request = new CreateClubSubscriptionRequest(clubId, "", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.PlanId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WhenPlanDoesNotExistOrIsInactiveOrIsDeleted_ReturnsPlanNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        _clubs.Add(club);

        var inactivePlan = new SubscriptionPlan { Id = "plan-inactive", IsActive = false, IsDeleted = false };
        var deletedPlan = new SubscriptionPlan { Id = "plan-deleted", IsActive = true, IsDeleted = true };
        _plans.AddRange(new[] { inactivePlan, deletedPlan });

        var request = new CreateClubSubscriptionRequest(clubId, "plan-not-exist", DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("SubscriptionPlan.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task SubscribeAsync_WhenActiveSubscriptionAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";

        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Plan", MonthlyPrice = 50m, IsActive = true, IsDeleted = false };
        var activeSub = new ClubSubscription { ClubId = clubId, UserId = userId, PlanId = planId, Status = SubscriptionStatus.Active, IsDeleted = false };

        _clubs.Add(club);
        _plans.Add(plan);
        _subscriptions.Add(activeSub);

        var request = new CreateClubSubscriptionRequest(clubId, planId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Subscription.Conflict");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task SubscribeAsync_WhenValidRequest_CreatesSubscriptionAndPaymentAndCommitsTransaction()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";

        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Plan", MonthlyPrice = 50m, IsActive = true, IsDeleted = false };

        _clubs.Add(club);
        _plans.Add(plan);

        var request = new CreateClubSubscriptionRequest(clubId, planId, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsActive.Should().BeFalse(); // Because status is PendingPayment initially

        _subscriptions.Should().ContainSingle(s => s.ClubId == clubId && s.PlanId == planId && s.Status == SubscriptionStatus.PendingPayment);
        _payments.Should().ContainSingle(p => p.Amount == plan.MonthlyPrice && p.Status == PaymentStatus.Pending);

        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region RenewSubscriptionAsync Tests

    [Fact]
    public async Task RenewSubscriptionAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var userId = "";
        var clubId = "club-1";

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WithNullClubId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = " ";

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.ClubId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-not-exist";

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenCallerNotClubOwner_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        _clubs.Add(new Club { Id = clubId, OwnerId = "another-user", IsDeleted = false });

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenClubIsInactive_ReturnsClubInactive()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = false, IsDeleted = false };
        _clubs.Add(club);

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenNoPreviousSubscriptionExists_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        _clubs.Add(club);

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Subscription.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenExistingSubscriptionStillActive_ReturnsConflict()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Plan", MonthlyPrice = 50m, IsActive = true };
        var existingSub = new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            PlanId = planId,
            Status = SubscriptionStatus.Active, // Still active, can't renew this state
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            Plan = plan,
            Club = club
        };

        _clubs.Add(club);
        _plans.Add(plan);
        _subscriptions.Add(existingSub);

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Subscription.CannotRenew");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenPlanIsInactiveOrDeleted_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Plan", MonthlyPrice = 50m, IsActive = false, IsDeleted = false }; // Inactive
        var existingSub = new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            PlanId = planId,
            Status = SubscriptionStatus.Cancelled,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            Plan = plan,
            Club = club
        };

        _clubs.Add(club);
        _plans.Add(plan);
        _subscriptions.Add(existingSub);

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Plan.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenValidRequest_CreatesNewSubscriptionAndPaymentAndCommitsTransaction()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var planId = "plan-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsActive = true, IsDeleted = false };
        var plan = new SubscriptionPlan { Id = planId, Name = "Plan", MonthlyPrice = 50m, IsActive = true, IsDeleted = false };
        var existingSub = new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            PlanId = planId,
            Status = SubscriptionStatus.Expired, // Expired, so we can renew it
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow.AddMonths(-1),
            Plan = plan,
            Club = club
        };

        _clubs.Add(club);
        _plans.Add(plan);
        _subscriptions.Add(existingSub);

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.IsActive.Should().BeFalse();

        _subscriptions.Should().Contain(s => s.ClubId == clubId && s.PlanId == planId && s.Status == SubscriptionStatus.PendingPayment);
        _payments.Should().ContainSingle(p => p.Amount == plan.MonthlyPrice && p.Status == PaymentStatus.Pending);

        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    #endregion

    #region CancelSubscriptionAsync Tests

    [Fact]
    public async Task CancelSubscriptionAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var userId = "";
        var clubId = "club-1";

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WithNullClubId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var clubId = " ";

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.ClubId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-not-exist";

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenCallerNotClubOwner_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        _clubs.Add(new Club { Id = clubId, OwnerId = "another-user", IsDeleted = false });

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenNoActiveSubscriptionExists_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        _clubs.Add(club);

        // Only a Cancelled/PendingPayment subscription exists, no active subscription
        var pendingSub = new ClubSubscription { ClubId = clubId, UserId = userId, Status = SubscriptionStatus.PendingPayment, IsDeleted = false };
        _subscriptions.Add(pendingSub);

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Subscription.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenActiveSubscriptionExists_CancelsSuccessfully()
    {
        // Arrange
        var userId = "user-1";
        var clubId = "club-1";
        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        var activeSub = new ClubSubscription
        {
            ClubId = clubId,
            UserId = userId,
            Status = SubscriptionStatus.Active,
            IsDeleted = false,
            Club = club
        };

        _clubs.Add(club);
        _subscriptions.Add(activeSub);

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        activeSub.Status.Should().Be(SubscriptionStatus.Cancelled);
        activeSub.CancelledAt.Should().NotBeNull();
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Generic EF DbSet Mock Helpers

    private static Mock<DbSet<T>> CreateDbSetMock<T>(List<T> elements) where T : class
    {
        var elementsQueryable = elements.AsQueryable();
        var mockDbSet = new Mock<DbSet<T>>();

        mockDbSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(elementsQueryable.GetEnumerator()));

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(elementsQueryable.Provider));

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.Expression)
            .Returns(elementsQueryable.Expression);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.ElementType)
            .Returns(elementsQueryable.ElementType);

        mockDbSet.As<IQueryable<T>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => elementsQueryable.GetEnumerator());

        mockDbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(elements.Add);
        mockDbSet.Setup(d => d.Update(It.IsAny<T>())).Callback<T>(entity => { });

        return mockDbSet;
    }

    private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression) : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object? Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var result = _inner.Execute(expression);

            if (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
            {
                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(expectedResultType)
                    .Invoke(null, new[] { result })!;
            }

            return (TResult)Activator.CreateInstance(typeof(TResult), result)!;
        }
    }

    #endregion
}
