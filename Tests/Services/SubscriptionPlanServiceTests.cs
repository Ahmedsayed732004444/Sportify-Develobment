using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Sportiva.Abstractions;
using Sportiva.Contracts.Subscriptions;
using Sportiva.Entities;
using Sportiva.Errors;
using Sportiva.Persistence;
using Sportiva.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Sportiva.Tests.Services;

/// <summary>
/// Unit tests for SubscriptionPlanService.
/// Adheres strictly to xUnit, Moq, FluentAssertions, and AAA structure.
/// </summary>
public class SubscriptionPlanServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ILogger<SubscriptionPlanService>> _loggerMock;
    private readonly SubscriptionPlanService _service;

    public SubscriptionPlanServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<SubscriptionPlanService>>();
        _service = new SubscriptionPlanService(_context, _loggerMock.Object);
    }

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    #region GetPlansAsync Tests

    [Fact]
    public async Task GetPlansAsync_WhenPlansExist_ReturnsActivePlansOrderedByMonthlyPrice()
    {
        // Arrange
        var plan1 = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Premium Plan",
            MonthlyPrice = 200m,
            MaxCourts = 10,
            IsActive = true,
            IsDeleted = false
        };

        var plan2 = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Basic Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        var inactivePlan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Inactive Plan",
            MonthlyPrice = 50m,
            MaxCourts = 2,
            IsActive = false,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddRangeAsync(plan1, plan2, inactivePlan);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPlansAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].PlanId.Should().Be(plan2.Id); // Basic Plan (100) first due to order by price
        result.Value[1].PlanId.Should().Be(plan1.Id); // Premium Plan (200) second
    }

    [Fact]
    public async Task GetPlansAsync_DatabaseThrowsException_ReturnsFailureResultWithLogging()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var throwingContext = new ThrowingTestDbContext(options) { ShouldThrow = true };
        var serviceWithThrowingContext = new SubscriptionPlanService(throwingContext, _loggerMock.Object);

        // Act
        var result = await serviceWithThrowingContext.GetPlansAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.Error);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region GetPlanAsync Tests

    [Fact]
    public async Task GetPlanAsync_WithValidPlanId_ReturnsPlanResponse()
    {
        // Arrange
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Standard Plan",
            MonthlyPrice = 150m,
            MaxCourts = 8,
            IsActive = true,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetPlanAsync(plan.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.PlanId.Should().Be(plan.Id);
        result.Value.Name.Should().Be(plan.Name);
        result.Value.Price.Should().Be(plan.MonthlyPrice);
        result.Value.MaxCourts.Should().Be(plan.MaxCourts);
        result.Value.IsActive.Should().Be(plan.IsActive);
    }

    [Fact]
    public async Task GetPlanAsync_WithNullOrWhitespacePlanId_ReturnsPlanNotFound()
    {
        // Arrange & Act
        var resultNull = await _service.GetPlanAsync(null!, CancellationToken.None);
        var resultEmpty = await _service.GetPlanAsync("   ", CancellationToken.None);

        // Assert
        resultNull.IsSuccess.Should().BeFalse();
        resultNull.Error.Should().Be(SubscriptionErrors.PlanNotFound);

        resultEmpty.IsSuccess.Should().BeFalse();
        resultEmpty.Error.Should().Be(SubscriptionErrors.PlanNotFound);
    }

    [Fact]
    public async Task GetPlanAsync_WhenPlanDoesNotExist_ReturnsPlanNotFound()
    {
        // Arrange & Act
        var result = await _service.GetPlanAsync("nonexistent-plan-id", CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.PlanNotFound);
    }

    [Fact]
    public async Task GetPlanAsync_DatabaseThrowsException_ReturnsFailureResultWithLogging()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var throwingContext = new ThrowingTestDbContext(options) { ShouldThrow = true };
        var serviceWithThrowingContext = new SubscriptionPlanService(throwingContext, _loggerMock.Object);

        // Act
        var result = await serviceWithThrowingContext.GetPlanAsync("some-plan-id", CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.Error);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region CreatePlanAsync Tests

    [Fact]
    public async Task CreatePlanAsync_WithValidRequest_CreatesPlanAndReturnsSuccess()
    {
        // Arrange
        var request = new CreateSubscriptionPlanRequest("Basic Plan", "Basic Description", 100m, 5);

        // Act
        var result = await _service.CreatePlanAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
        result.Value.Price.Should().Be(request.Price);
        result.Value.MaxCourts.Should().Be(request.MaxCourts);
        result.Value.IsActive.Should().BeTrue();

        var savedPlan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == result.Value.PlanId);
        savedPlan.Should().NotBeNull();
        savedPlan!.Name.Should().Be(request.Name);
        savedPlan.MonthlyPrice.Should().Be(request.Price);
        savedPlan.MaxCourts.Should().Be(request.MaxCourts);
    }

    [Fact]
    public async Task CreatePlanAsync_WithNullOrWhitespaceName_ReturnsInvalidPlan()
    {
        // Arrange
        var requestNull = new CreateSubscriptionPlanRequest(null!, "Desc", 100m, 5);
        var requestEmpty = new CreateSubscriptionPlanRequest("  ", "Desc", 100m, 5);

        // Act
        var resultNull = await _service.CreatePlanAsync(requestNull, CancellationToken.None);
        var resultEmpty = await _service.CreatePlanAsync(requestEmpty, CancellationToken.None);

        // Assert
        resultNull.IsSuccess.Should().BeFalse();
        resultNull.Error.Should().Be(SubscriptionErrors.InvalidPlan);

        resultEmpty.IsSuccess.Should().BeFalse();
        resultEmpty.Error.Should().Be(SubscriptionErrors.InvalidPlan);
    }

    [Fact]
    public async Task CreatePlanAsync_WithNegativePrice_ReturnsInvalidPlan()
    {
        // Arrange
        var request = new CreateSubscriptionPlanRequest("Plan", "Desc", -10m, 5);

        // Act
        var result = await _service.CreatePlanAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.InvalidPlan);
    }

    [Fact]
    public async Task CreatePlanAsync_WithZeroOrNegativeMaxCourts_ReturnsInvalidPlan()
    {
        // Arrange
        var requestZero = new CreateSubscriptionPlanRequest("Plan", "Desc", 100m, 0);
        var requestNegative = new CreateSubscriptionPlanRequest("Plan", "Desc", 100m, -5);

        // Act
        var resultZero = await _service.CreatePlanAsync(requestZero, CancellationToken.None);
        var resultNegative = await _service.CreatePlanAsync(requestNegative, CancellationToken.None);

        // Assert
        resultZero.IsSuccess.Should().BeFalse();
        resultZero.Error.Should().Be(SubscriptionErrors.InvalidPlan);

        resultNegative.IsSuccess.Should().BeFalse();
        resultNegative.Error.Should().Be(SubscriptionErrors.InvalidPlan);
    }

    [Fact]
    public async Task CreatePlanAsync_DatabaseThrowsException_ReturnsFailureResultWithLogging()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var throwingContext = new ThrowingTestDbContext(options) { ShouldThrow = true };
        var serviceWithThrowingContext = new SubscriptionPlanService(throwingContext, _loggerMock.Object);
        var request = new CreateSubscriptionPlanRequest("Plan", "Desc", 100m, 5);

        // Act
        var result = await serviceWithThrowingContext.CreatePlanAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.Error);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region UpdatePlanAsync Tests

    [Fact]
    public async Task UpdatePlanAsync_WithValidRequest_UpdatesPlanAndReturnsSuccess()
    {
        // Arrange
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Old Name",
            Description = "Old Description",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var request = new UpdateSubscriptionPlanRequest("New Name", "New Description", 150m, 10, false);

        // Act
        var result = await _service.UpdatePlanAsync(plan.Id, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
        result.Value.Price.Should().Be(request.Price);
        result.Value.MaxCourts.Should().Be(request.MaxCourts);
        result.Value.IsActive.Should().BeFalse();

        var updatedPlan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == plan.Id);
        updatedPlan.Should().NotBeNull();
        updatedPlan!.Name.Should().Be(request.Name);
        updatedPlan.MonthlyPrice.Should().Be(request.Price);
        updatedPlan.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UpdatePlanAsync_WithNullOrWhitespacePlanId_ReturnsPlanNotFound()
    {
        // Arrange
        var request = new UpdateSubscriptionPlanRequest("Name", "Desc", 100m, 5, true);

        // Act
        var resultNull = await _service.UpdatePlanAsync(null!, request, CancellationToken.None);
        var resultEmpty = await _service.UpdatePlanAsync("   ", request, CancellationToken.None);

        // Assert
        resultNull.IsSuccess.Should().BeFalse();
        resultNull.Error.Should().Be(SubscriptionErrors.PlanNotFound);

        resultEmpty.IsSuccess.Should().BeFalse();
        resultEmpty.Error.Should().Be(SubscriptionErrors.PlanNotFound);
    }

    [Fact]
    public async Task UpdatePlanAsync_WhenPlanDoesNotExist_ReturnsPlanNotFound()
    {
        // Arrange
        var request = new UpdateSubscriptionPlanRequest("Name", "Desc", 100m, 5, true);

        // Act
        var result = await _service.UpdatePlanAsync("nonexistent-plan-id", request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.PlanNotFound);
    }

    [Fact]
    public async Task UpdatePlanAsync_WithNullOrWhitespaceName_ReturnsInvalidPlan()
    {
        // Arrange
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Standard Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var requestNull = new UpdateSubscriptionPlanRequest(null!, "Desc", 100m, 5, true);
        var requestEmpty = new UpdateSubscriptionPlanRequest("  ", "Desc", 100m, 5, true);

        // Act
        var resultNull = await _service.UpdatePlanAsync(plan.Id, requestNull, CancellationToken.None);
        var resultEmpty = await _service.UpdatePlanAsync(plan.Id, requestEmpty, CancellationToken.None);

        // Assert
        resultNull.IsSuccess.Should().BeFalse();
        resultNull.Error.Should().Be(SubscriptionErrors.InvalidPlan);

        resultEmpty.IsSuccess.Should().BeFalse();
        resultEmpty.Error.Should().Be(SubscriptionErrors.InvalidPlan);
    }

    [Fact]
    public async Task UpdatePlanAsync_WithNegativePrice_ReturnsInvalidPlan()
    {
        // Arrange
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Standard Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var request = new UpdateSubscriptionPlanRequest("Name", "Desc", -5m, 5, true);

        // Act
        var result = await _service.UpdatePlanAsync(plan.Id, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.InvalidPlan);
    }

    [Fact]
    public async Task UpdatePlanAsync_WithZeroOrNegativeMaxCourts_ReturnsInvalidPlan()
    {
        // Arrange
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Standard Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        var requestZero = new UpdateSubscriptionPlanRequest("Name", "Desc", 100m, 0, true);
        var requestNegative = new UpdateSubscriptionPlanRequest("Name", "Desc", 100m, -1, true);

        // Act
        var resultZero = await _service.UpdatePlanAsync(plan.Id, requestZero, CancellationToken.None);
        var resultNegative = await _service.UpdatePlanAsync(plan.Id, requestNegative, CancellationToken.None);

        // Assert
        resultZero.IsSuccess.Should().BeFalse();
        resultZero.Error.Should().Be(SubscriptionErrors.InvalidPlan);

        resultNegative.IsSuccess.Should().BeFalse();
        resultNegative.Error.Should().Be(SubscriptionErrors.InvalidPlan);
    }

    [Fact]
    public async Task UpdatePlanAsync_DatabaseThrowsException_ReturnsFailureResultWithLogging()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var throwingContext = new ThrowingTestDbContext(options);
        var serviceWithThrowingContext = new SubscriptionPlanService(throwingContext, _loggerMock.Object);

        var plan = new SubscriptionPlan
        {
            Id = "plan-id-123",
            Name = "Standard Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await throwingContext.SubscriptionPlans.AddAsync(plan);
        await throwingContext.SaveChangesAsync();

        var request = new UpdateSubscriptionPlanRequest("New Name", "Desc", 150m, 10, true);

        throwingContext.ShouldThrow = true;

        // Act
        var result = await serviceWithThrowingContext.UpdatePlanAsync(plan.Id, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.Error);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region ArchivePlanAsync Tests

    [Fact]
    public async Task ArchivePlanAsync_WithValidPlanId_MarksPlanDeletedAndInactive()
    {
        // Arrange
        var plan = new SubscriptionPlan
        {
            Id = Guid.CreateVersion7().ToString(),
            Name = "Standard Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await _context.SubscriptionPlans.AddAsync(plan);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.ArchivePlanAsync(plan.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var archivedPlan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == plan.Id);
        archivedPlan.Should().NotBeNull();
        archivedPlan!.IsDeleted.Should().BeTrue();
        archivedPlan.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ArchivePlanAsync_WithNullOrWhitespacePlanId_ReturnsPlanNotFound()
    {
        // Act
        var resultNull = await _service.ArchivePlanAsync(null!, CancellationToken.None);
        var resultEmpty = await _service.ArchivePlanAsync("   ", CancellationToken.None);

        // Assert
        resultNull.IsSuccess.Should().BeFalse();
        resultNull.Error.Should().Be(SubscriptionErrors.PlanNotFound);

        resultEmpty.IsSuccess.Should().BeFalse();
        resultEmpty.Error.Should().Be(SubscriptionErrors.PlanNotFound);
    }

    [Fact]
    public async Task ArchivePlanAsync_WhenPlanDoesNotExist_ReturnsPlanNotFound()
    {
        // Act
        var result = await _service.ArchivePlanAsync("nonexistent-plan-id", CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.PlanNotFound);
    }

    [Fact]
    public async Task ArchivePlanAsync_DatabaseThrowsException_ReturnsFailureResultWithLogging()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var throwingContext = new ThrowingTestDbContext(options);
        var serviceWithThrowingContext = new SubscriptionPlanService(throwingContext, _loggerMock.Object);

        var plan = new SubscriptionPlan
        {
            Id = "plan-id-123",
            Name = "Standard Plan",
            MonthlyPrice = 100m,
            MaxCourts = 5,
            IsActive = true,
            IsDeleted = false
        };

        await throwingContext.SubscriptionPlans.AddAsync(plan);
        await throwingContext.SaveChangesAsync();

        throwingContext.ShouldThrow = true;

        // Act
        var result = await serviceWithThrowingContext.ArchivePlanAsync(plan.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(SubscriptionErrors.Error);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    #endregion

    #region Inner Helper Throwing DB Context

    private class ThrowingTestDbContext : ApplicationDbContext
    {
        public bool ShouldThrow { get; set; }
        public ThrowingTestDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (ShouldThrow)
            {
                throw new DbUpdateException("Database save failed simulated exception.");
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion
}
