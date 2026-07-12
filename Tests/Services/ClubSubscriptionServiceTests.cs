using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sportiva.Abstractions;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Enums;
using Sportiva.Persistence;
using Sportiva.Services;
using Sportiva.Services.Implementation;
using Xunit;

namespace Sportiva.Tests.Services;

public class ClubSubscriptionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IWalletService> _walletServiceMock;
    private readonly ClubSubscriptionService _service;

    public ClubSubscriptionServiceTests()
    {
        // Use a unique in-memory database name per test to ensure isolation
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _walletServiceMock = new Mock<IWalletService>();
        _service = new ClubSubscriptionService(_context, _walletServiceMock.Object);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region SubscribeAsync Tests

    [Fact]
    public async Task SubscribeAsync_WithValidRequest_CreatesSubscriptionAndReturnsSuccess()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "plan-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = planId,
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29); // 30 days
        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Mock wallet deduction success
        _walletServiceMock
            .Setup(w => w.DeductAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.SubscriptionId.Should().NotBeNullOrEmpty();
        result.Value.IsActive.Should().BeTrue();

        var savedSubscription = await _context.ClubSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.ClubId == clubId);
        savedSubscription.Should().NotBeNull();
        savedSubscription.Status.Should().Be(SubscriptionStatus.Active);
        savedSubscription.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task SubscribeAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var clubId = "club-123";
        var planId = "plan-456";
        var request = new CreateClubSubscriptionRequest(clubId, planId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _service.SubscribeAsync(null!, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WithNullClubId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-123";
        var planId = "plan-456";
        var request = new CreateClubSubscriptionRequest("club-id", planId, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _service.SubscribeAsync(userId, null!, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.ClubId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WithNullPlanId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var request = new CreateClubSubscriptionRequest(clubId, null!, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(30));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.PlanId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WithInvalidDateRange_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "plan-789";
        var startDate = DateTime.UtcNow.AddDays(10);
        var endDate = startDate.AddDays(-1); // End before start

        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.DateRange");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubscribeAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "nonexistent-club";
        var planId = "plan-789";
        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29);

        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task SubscribeAsync_WhenClubIsInactive_ReturnsClubInactive()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "plan-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Inactive Club",
            IsActive = false,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = planId,
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            IsActive = true,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29);
        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Club.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task SubscribeAsync_WhenPlanDoesNotExist_ReturnsPlanNotFound()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "nonexistent-plan";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.SaveChangesAsync();

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29);
        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Plan.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task SubscribeAsync_WhenPlanIsInactive_ReturnsPlanInactive()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "plan-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = planId,
            Name = "Inactive Plan",
            MonthlyPrice = 100m,
            IsActive = false,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29);
        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Plan.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task SubscribeAsync_WhenUserAlreadyHasActiveSubscription_ReturnsDuplicateActive()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "plan-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = planId,
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            IsActive = true,
            IsDeleted = false
        };

        var existingSubscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = planId,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.ClubSubscriptions.AddAsync(existingSubscription);
        await _context.SaveChangesAsync();

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29);
        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.DuplicateActive");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task SubscribeAsync_WhenWalletBalanceInsufficient_ReturnsInsufficientBalance()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var planId = "plan-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = planId,
            Name = "Expensive Plan",
            MonthlyPrice = 10000m, // Very expensive
            IsActive = true,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var startDate = DateTime.UtcNow.AddDays(1);
        var endDate = startDate.AddDays(29);
        var request = new CreateClubSubscriptionRequest(clubId, planId, startDate, endDate);

        // Mock wallet deduction failure
        _walletServiceMock
            .Setup(w => w.DeductAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(new Error("Wallet.InsufficientBalance", "Insufficient wallet balance", 402)));

        // Act
        var result = await _service.SubscribeAsync(userId, clubId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Wallet.InsufficientBalance");
        result.Error.StatusCode.Should().Be(402);

        // Verify no subscription was created
        var subscription = await _context.ClubSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.ClubId == clubId);
        subscription.Should().BeNull();
    }

    #endregion

    #region CancelSubscriptionAsync Tests

    [Fact]
    public async Task CancelSubscriptionAsync_WithValidActiveSubscription_CancelledSuccessfully()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Clubs.AddAsync(club);
        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Mock wallet credit for refund
        _walletServiceMock
            .Setup(w => w.CreditAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var cancelledSubscription = await _context.ClubSubscriptions.FirstOrDefaultAsync(s => s.Id == subscription.Id);
        cancelledSubscription.Status.Should().Be(SubscriptionStatus.Cancelled);
        cancelledSubscription.CancelledAt.Should().NotBeNull();
        cancelledSubscription.RefundAmount.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var clubId = "club-456";

        // Act
        var result = await _service.CancelSubscriptionAsync(null!, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenSubscriptionDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "nonexistent-club";

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenSubscriptionBelongsToDifferentUser_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-123";
        var otherUserId = "user-999";
        var clubId = "club-456";

        var subscription = new ClubSubscription
        {
            UserId = otherUserId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.NotFound"); // From the query filter
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WhenAlreadyCancelled_ReturnsAlreadyCancelledError()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Cancelled,
            CancelledAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.AlreadyCancelled");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WithRefund_CreditsWallet()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var club = new Club
        {
            Id = clubId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20), // 30 days total, midway through
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Clubs.AddAsync(club);
        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Mock wallet credit
        _walletServiceMock
            .Setup(w => w.CreditAsync(userId, It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.CancelSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _walletServiceMock.Verify(
            w => w.CreditAsync(userId, It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region GetActiveSubscriptionAsync Tests

    [Fact]
    public async Task GetActiveSubscriptionAsync_WithValidSubscription_ReturnsCorrectSubscription()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false,
            LogoUrl = "https://example.com/logo.png",
            City = "Cairo"
        };

        var plan = new SubscriptionPlan
        {
            Id = "plan-789",
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.SubscriptionId.Should().Be(subscription.Id);
        result.Value.IsActive.Should().BeTrue();
        result.Value.Club.ClubId.Should().Be(clubId);
        result.Value.Plan.PlanId.Should().Be("plan-789");
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_WhenNoActiveSubscriptionExists_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetActiveSubscriptionAsync_IgnoresCancelledSubscriptions()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Cancelled,
            CancelledAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetActiveSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.NotFound");
    }

    #endregion

    #region GetSubscriptionHistoryAsync Tests

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithValidFilters_ReturnsPaginatedResults()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = "plan-789",
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            IsActive = true,
            IsDeleted = false
        };

        var subscription1 = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-50),
            EndDate = DateTime.UtcNow.AddDays(-20),
            Price = 1000m,
            Status = SubscriptionStatus.Cancelled,
            CancelledAt = DateTime.UtcNow.AddDays(-20),
            CreatedAt = DateTime.UtcNow.AddDays(-50)
        };

        var subscription2 = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.ClubSubscriptions.AddAsync(subscription1);
        await _context.ClubSubscriptions.AddAsync(subscription2);
        await _context.SaveChangesAsync();

        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithInvalidPageNumber_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var filters = new RequestFilters { PageNumber = 0, PageSize = 10 }; // Invalid: < 1

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithInvalidPageSize_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 0 }; // Invalid: < 1

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_WithNoSubscriptions_ReturnsEmptyPaginatedList()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetSubscriptionHistoryAsync_ReturnsSortedByCreatedAtDescending()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = "plan-789",
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            IsActive = true,
            IsDeleted = false
        };

        var subscription1 = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-50),
            EndDate = DateTime.UtcNow.AddDays(-20),
            Price = 1000m,
            Status = SubscriptionStatus.Cancelled,
            CreatedAt = DateTime.UtcNow.AddDays(-50)
        };

        var subscription2 = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow.AddDays(-10)
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.ClubSubscriptions.AddAsync(subscription1);
        await _context.ClubSubscriptions.AddAsync(subscription2);
        await _context.SaveChangesAsync();

        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetSubscriptionHistoryAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items[0].SubscriptionId.Should().Be(subscription2.Id); // Most recent first
        result.Value.Items[1].SubscriptionId.Should().Be(subscription1.Id);
    }

    #endregion

    #region RenewSubscriptionAsync Tests

    [Fact]
    public async Task RenewSubscriptionAsync_WithValidCancelledSubscription_CreatesNewActiveSubscription()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = "plan-789",
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        var oldSubscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-50),
            EndDate = DateTime.UtcNow.AddDays(-20),
            Price = 1000m,
            Status = SubscriptionStatus.Cancelled,
            CancelledAt = DateTime.UtcNow.AddDays(-20),
            CreatedAt = DateTime.UtcNow.AddDays(-50)
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.ClubSubscriptions.AddAsync(oldSubscription);
        await _context.SaveChangesAsync();

        _walletServiceMock
            .Setup(w => w.DeductAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.IsActive.Should().BeTrue();

        var newSubscription = await _context.ClubSubscriptions
            .Where(s => s.UserId == userId && s.ClubId == clubId && s.Status == SubscriptionStatus.Active)
            .FirstOrDefaultAsync();
        newSubscription.Should().NotBeNull();
        newSubscription.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenNoSubscriptionExists_ReturnsNotFound()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenSubscriptionStillActive_CannotRenew()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            Price = 1000m,
            Status = SubscriptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Subscription.CannotRenew");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task RenewSubscriptionAsync_WhenClubIsInactive_ReturnsForbidden()
    {
        // Arrange
        var userId = "user-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            Name = "Inactive Club",
            IsActive = false,
            IsDeleted = false
        };

        var plan = new SubscriptionPlan
        {
            Id = "plan-789",
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            IsActive = true,
            IsDeleted = false
        };

        var subscription = new ClubSubscription
        {
            UserId = userId,
            ClubId = clubId,
            PlanId = "plan-789",
            StartDate = DateTime.UtcNow.AddDays(-50),
            EndDate = DateTime.UtcNow.AddDays(-20),
            Price = 1000m,
            Status = SubscriptionStatus.Cancelled,
            CancelledAt = DateTime.UtcNow.AddDays(-20),
            CreatedAt = DateTime.UtcNow.AddDays(-50)
        };

        await _context.Clubs.AddAsync(club);
        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.ClubSubscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.RenewSubscriptionAsync(userId, clubId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Club.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    #endregion
}
