using FluentAssertions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Sportiva.Abstractions;
using Sportiva.Contracts.Bookings;
using Sportiva.Contracts.Common;
using Sportiva.Contracts.Shared.Enums;
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
/// Unit tests for BookingService.
/// Adheres strictly to xUnit, Moq, FluentAssertions, and AAA structure.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<DatabaseFacade> _databaseMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;

    private readonly List<Club> _clubs;
    private readonly List<Court> _courts;
    private readonly List<TimeSlot> _timeSlots;
    private readonly List<Booking> _bookings;

    private readonly BookingService _service;

    static BookingServiceTests()
    {
        // 3. Mapster Configuration: Register mappings in global config
        TypeAdapterConfig.GlobalSettings.Scan(typeof(BookingMappingConfig).Assembly);
    }

    public BookingServiceTests()
    {
        _clubs = new List<Club>();
        _courts = new List<Court>();
        _timeSlots = new List<TimeSlot>();
        _bookings = new List<Booking>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _contextMock = new Mock<ApplicationDbContext>(options);

        // 4. EF Core Mocking: Create mocked DbSets supporting IAsyncEnumerable
        var clubsDbSet = CreateDbSetMock(_clubs);
        var courtsDbSet = CreateDbSetMock(_courts);
        var timeSlotsDbSet = CreateDbSetMock(_timeSlots);
        var bookingsDbSet = CreateDbSetMock(_bookings);

        // Set properties directly
        _contextMock.Object.Clubs = clubsDbSet.Object;
        _contextMock.Object.Courts = courtsDbSet.Object;
        _contextMock.Object.TimeSlots = timeSlotsDbSet.Object;
        _contextMock.Object.Bookings = bookingsDbSet.Object;

        // Mock Transaction
        _transactionMock = new Mock<IDbContextTransaction>();
        _databaseMock = new Mock<DatabaseFacade>(_contextMock.Object);
        _databaseMock.Setup(d => d.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

        _contextMock.Setup(c => c.Database).Returns(_databaseMock.Object);
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        _service = new BookingService(_contextMock.Object);
    }

    #region CreateBookingAsync Tests

    [Fact]
    public async Task CreateBookingAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var userId = "";
        var request = new CreateBookingRequest("court-1", "slot-1");

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNullCourtId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var request = new CreateBookingRequest("", "slot-1");

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.CourtId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateBookingAsync_WithNullTimeSlotId_ReturnsValidationError()
    {
        // Arrange
        var userId = "user-1";
        var request = new CreateBookingRequest("court-1", " ");

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.TimeSlotId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenTimeSlotDoesNotExist_ReturnsTimeSlotNotFound()
    {
        // Arrange
        var userId = "user-1";
        var request = new CreateBookingRequest("court-1", "slot-not-exist");

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TimeSlot.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenTimeSlotInThePast_ReturnsTimeSlotInThePast()
    {
        // Arrange
        var userId = "user-1";
        var courtId = "court-1";
        var slotId = "slot-past";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        var localPast = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(-2), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1", IsActive = true };
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsActive = true };
        var pastSlot = new TimeSlot
        {
            Id = slotId,
            CourtId = courtId,
            Court = court,
            Day = DateOnly.FromDateTime(localPast),
            StartTime = TimeOnly.FromDateTime(localPast),
            EndTime = TimeOnly.FromDateTime(localPast.AddHours(1))
        };

        _clubs.Add(club);
        _courts.Add(court);
        _timeSlots.Add(pastSlot);

        var request = new CreateBookingRequest(courtId, slotId);

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TimeSlot.InThePast");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenCourtInactive_ReturnsCourtInactive()
    {
        // Arrange
        var userId = "user-1";
        var courtId = "court-1";
        var slotId = "slot-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        var localFuture = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(5), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1", IsActive = true };
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsActive = false }; // Inactive
        var slot = new TimeSlot
        {
            Id = slotId,
            CourtId = courtId,
            Court = court,
            Day = DateOnly.FromDateTime(localFuture),
            StartTime = TimeOnly.FromDateTime(localFuture),
            EndTime = TimeOnly.FromDateTime(localFuture.AddHours(1))
        };

        _clubs.Add(club);
        _courts.Add(court);
        _timeSlots.Add(slot);

        var request = new CreateBookingRequest(courtId, slotId);

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Court.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenClubInactive_ReturnsClubInactive()
    {
        // Arrange
        var userId = "user-1";
        var courtId = "court-1";
        var slotId = "slot-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        var localFuture = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(5), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1", IsActive = false }; // Inactive
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsActive = true };
        var slot = new TimeSlot
        {
            Id = slotId,
            CourtId = courtId,
            Court = court,
            Day = DateOnly.FromDateTime(localFuture),
            StartTime = TimeOnly.FromDateTime(localFuture),
            EndTime = TimeOnly.FromDateTime(localFuture.AddHours(1))
        };

        _clubs.Add(club);
        _courts.Add(court);
        _timeSlots.Add(slot);

        var request = new CreateBookingRequest(courtId, slotId);

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Inactive");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenRaceConditionOccurs_RollsBackTransactionAndReturnsConflict()
    {
        // Arrange
        var userId = "user-1";
        var courtId = "court-1";
        var slotId = "slot-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        var localFuture = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(5), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1", IsActive = true };
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsActive = true, PricePerHour = 100m };
        var slot = new TimeSlot
        {
            Id = slotId,
            CourtId = courtId,
            Court = court,
            Day = DateOnly.FromDateTime(localFuture),
            StartTime = TimeOnly.FromDateTime(localFuture),
            EndTime = TimeOnly.FromDateTime(localFuture.AddHours(1))
        };

        _clubs.Add(club);
        _courts.Add(court);
        _timeSlots.Add(slot);

        var request = new CreateBookingRequest(courtId, slotId);

        var dbUpdateException = new DbUpdateException("DB error", new Exception("UNIQUE constraint violation occurred"));
        _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(dbUpdateException);

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("TimeSlot.AlreadyBooked");
        result.Error.StatusCode.Should().Be(409);

        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WhenSuccess_CreatesBookingAsPendingAndCommitsTransaction()
    {
        // Arrange
        var userId = "user-1";
        var courtId = "court-1";
        var slotId = "slot-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        var localFuture = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(5), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1", IsActive = true };
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsActive = true, PricePerHour = 100m };
        var slot = new TimeSlot
        {
            Id = slotId,
            CourtId = courtId,
            Court = court,
            Day = DateOnly.FromDateTime(localFuture),
            StartTime = TimeOnly.FromDateTime(localFuture),
            EndTime = TimeOnly.FromDateTime(localFuture.AddHours(2)) // 2 hours duration
        };

        _clubs.Add(club);
        _courts.Add(court);
        _timeSlots.Add(slot);

        var request = new CreateBookingRequest(courtId, slotId);

        // Act
        var result = await _service.CreateBookingAsync(userId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Price.Should().Be(200m); // 100m * 2 hours
        result.Value.Status.Should().Be(BookingStatusDto.Pending); // Status starts as Pending

        _bookings.Should().ContainSingle(b => b.UserId == userId && b.TimeSlotId == slotId && b.Price == 200m && b.Status == BookingStatus.Pending);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetBookingAsync Tests

    [Fact]
    public async Task GetBookingAsync_WithNullBookingId_ReturnsValidationError()
    {
        // Act
        var result = await _service.GetBookingAsync("", "user-1", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.BookingId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetBookingAsync_WithNullUserId_ReturnsValidationError()
    {
        // Act
        var result = await _service.GetBookingAsync("booking-1", " ", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetBookingAsync_WhenBookingDoesNotExist_ReturnsBookingNotFound()
    {
        // Act
        var result = await _service.GetBookingAsync("booking-not-exist", "user-1", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetBookingAsync_WhenCallerNotOwnerNorClubOwner_ReturnsBookingForbidden()
    {
        // Arrange
        var userId = "unauthorized-user";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = "booking-owner-user",
            Court = court,
            TimeSlot = slot
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.GetBookingAsync(bookingId, userId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetBookingAsync_WhenCallerIsBookingOwner_ReturnsSuccess()
    {
        // Arrange
        var userId = "booking-owner-user";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.GetBookingAsync(bookingId, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task GetBookingAsync_WhenCallerIsClubOwner_ReturnsSuccess()
    {
        // Arrange
        var userId = "owner-1";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = userId };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = "booking-owner-user",
            Court = court,
            TimeSlot = slot
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.GetBookingAsync(bookingId, userId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BookingId.Should().Be(bookingId);
    }

    #endregion

    #region GetBookingReceiptAsync Tests

    [Fact]
    public async Task GetBookingReceiptAsync_WithNullUserId_ReturnsValidationError()
    {
        // Act
        var result = await _service.GetBookingReceiptAsync("", "booking-1", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetBookingReceiptAsync_WithNullBookingId_ReturnsValidationError()
    {
        // Act
        var result = await _service.GetBookingReceiptAsync("user-1", " ", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.TimeSlotId"); // Validated as timeSlotId parameter in ValidateIds
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetBookingReceiptAsync_WhenBookingDoesNotExist_ReturnsBookingNotFound()
    {
        // Act
        var result = await _service.GetBookingReceiptAsync("user-1", "booking-not-exist", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetBookingReceiptAsync_WhenCallerNotOwner_ReturnsBookingForbidden()
    {
        // Arrange
        var userId = "club-owner-user"; // Club owner can view booking but not the financial receipt
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = userId };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = "booking-owner-user",
            Court = court,
            TimeSlot = slot
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.GetBookingReceiptAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetBookingReceiptAsync_WhenCallerIsOwner_ReturnsReceiptResponse()
    {
        // Arrange
        var userId = "booking-owner-user";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.GetBookingReceiptAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BookingId.Should().Be(bookingId);
    }

    #endregion

    #region CancelBookingAsync Tests

    [Fact]
    public async Task CancelBookingAsync_WithNullUserId_ReturnsValidationError()
    {
        // Act
        var result = await _service.CancelBookingAsync("", "booking-1", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CancelBookingAsync_WithNullBookingId_ReturnsValidationError()
    {
        // Act
        var result = await _service.CancelBookingAsync("user-1", " ", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.TimeSlotId"); // Validated as timeSlotId parameter in ValidateIds
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenBookingDoesNotExist_ReturnsBookingNotFound()
    {
        // Act
        var result = await _service.CancelBookingAsync("user-1", "booking-not-exist", CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenCallerNotOwner_ReturnsBookingForbidden()
    {
        // Arrange
        var userId = "another-user";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = "booking-owner-user",
            Court = court,
            TimeSlot = slot
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.CancelBookingAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenAlreadyCancelled_ReturnsAlreadyCancelled()
    {
        // Arrange
        var userId = "user-1";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot,
            Status = BookingStatus.Cancelled
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.CancelBookingAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.AlreadyCancelled");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenAlreadyCompleted_ReturnsCannotCancelCompleted()
    {
        // Arrange
        var userId = "user-1";
        var bookingId = "booking-1";

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot { Id = "slot-1" };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot,
            Status = BookingStatus.Completed
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.CancelBookingAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.Completed");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenCancellationWindowClosed_ReturnsCancellationWindowClosed()
    {
        // Arrange
        var userId = "user-1";
        var bookingId = "booking-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        // Time slot starts in 30 minutes (0.5 hours)
        var localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddMinutes(30), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot = new TimeSlot
        {
            Id = "slot-1",
            CourtId = "court-1",
            Day = DateOnly.FromDateTime(localStart),
            StartTime = TimeOnly.FromDateTime(localStart),
            EndTime = TimeOnly.FromDateTime(localStart.AddHours(1))
        };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot,
            Status = BookingStatus.Confirmed
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.CancelBookingAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Booking.CancellationWindowClosed");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenInsidePartialRefundWindow_CancelsSuccessfully()
    {
        // Arrange
        var userId = "user-1";
        var bookingId = "booking-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        // Start time is in 5 hours
        var localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(5), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club, Name = "Center Court" };
        var slot = new TimeSlot
        {
            Id = "slot-1",
            CourtId = "court-1",
            Day = DateOnly.FromDateTime(localStart),
            StartTime = TimeOnly.FromDateTime(localStart),
            EndTime = TimeOnly.FromDateTime(localStart.AddHours(1))
        };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot,
            Status = BookingStatus.Confirmed,
            Price = 100m
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.CancelBookingAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelBookingAsync_WhenInsideFullRefundWindow_CancelsSuccessfully()
    {
        // Arrange
        var userId = "user-1";
        var bookingId = "booking-1";

        var cairoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        // Start time is in 26 hours (> 24 hours)
        var localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(26), cairoTimeZone);

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club, Name = "Center Court" };
        var slot = new TimeSlot
        {
            Id = "slot-1",
            CourtId = "court-1",
            Day = DateOnly.FromDateTime(localStart),
            StartTime = TimeOnly.FromDateTime(localStart),
            EndTime = TimeOnly.FromDateTime(localStart.AddHours(1))
        };
        var booking = new Booking
        {
            Id = bookingId,
            UserId = userId,
            Court = court,
            TimeSlot = slot,
            Status = BookingStatus.Confirmed,
            Price = 100m
        };

        _bookings.Add(booking);

        // Act
        var result = await _service.CancelBookingAsync(userId, bookingId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        booking.Status.Should().Be(BookingStatus.Cancelled);
        _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetMyBookingsAsync Tests

    [Fact]
    public async Task GetMyBookingsAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetMyBookingsAsync("", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetMyBookingsAsync_WithInvalidPageNumber_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 0, PageSize = 10 };

        // Act
        var result = await _service.GetMyBookingsAsync("user-1", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetMyBookingsAsync_WithInvalidPageSize_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 0 }; // < 1

        // Act
        var result = await _service.GetMyBookingsAsync("user-1", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetMyBookingsAsync_WhenValidRequestWithoutFilter_ReturnsAllUserBookings()
    {
        // Arrange
        var userId = "user-1";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot1 = new TimeSlot { Id = "slot-1", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(10, 0) };
        var slot2 = new TimeSlot { Id = "slot-2", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(12, 0) };

        var booking1 = new Booking { Id = "booking-1", UserId = userId, Court = court, TimeSlot = slot1, Status = BookingStatus.Confirmed };
        var booking2 = new Booking { Id = "booking-2", UserId = userId, Court = court, TimeSlot = slot2, Status = BookingStatus.Cancelled };
        var bookingOfOther = new Booking { Id = "booking-3", UserId = "other-user", Court = court, TimeSlot = slot1, Status = BookingStatus.Confirmed };

        _bookings.AddRange(new[] { booking1, booking2, bookingOfOther });

        // Act
        var result = await _service.GetMyBookingsAsync(userId, filters, null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
        result.Value.Items.Select(i => i.BookingId).Should().Contain(new[] { "booking-1", "booking-2" });
    }

    [Fact]
    public async Task GetMyBookingsAsync_WhenValidRequestWithStatusFilter_ReturnsFilteredBookings()
    {
        // Arrange
        var userId = "user-1";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        var club = new Club { Id = "club-1", OwnerId = "owner-1" };
        var court = new Court { Id = "court-1", Club = club };
        var slot1 = new TimeSlot { Id = "slot-1", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(10, 0) };
        var slot2 = new TimeSlot { Id = "slot-2", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(12, 0) };

        var booking1 = new Booking { Id = "booking-1", UserId = userId, Court = court, TimeSlot = slot1, Status = BookingStatus.Confirmed };
        var booking2 = new Booking { Id = "booking-2", UserId = userId, Court = court, TimeSlot = slot2, Status = BookingStatus.Cancelled };

        _bookings.AddRange(new[] { booking1, booking2 });

        // Act
        var result = await _service.GetMyBookingsAsync(userId, filters, BookingStatus.Confirmed, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items[0].BookingId.Should().Be("booking-1");
    }

    #endregion

    #region GetCourtBookingsAsync Tests

    [Fact]
    public async Task GetCourtBookingsAsync_WithNullUserIdOrCourtId_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetCourtBookingsAsync("", "court-1", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetCourtBookingsAsync_WithInvalidFilters_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 0 };

        // Act
        var result = await _service.GetCourtBookingsAsync("owner-1", "court-1", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetCourtBookingsAsync_WhenCourtDoesNotExist_ReturnsCourtNotFound()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetCourtBookingsAsync("owner-1", "court-not-exist", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Court.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetCourtBookingsAsync_WhenCallerNotClubOwner_ReturnsCourtForbidden()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };
        var club = new Club { Id = "club-1", OwnerId = "another-owner" };
        var court = new Court { Id = "court-1", ClubId = "club-1", Club = club, IsDeleted = false };
        _clubs.Add(club);
        _courts.Add(court);

        // Act
        var result = await _service.GetCourtBookingsAsync("owner-1", "court-1", filters, null, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Court.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetCourtBookingsAsync_WhenValidRequestWithoutDateFilter_ReturnsAllCourtBookings()
    {
        // Arrange
        var userId = "owner-1";
        var courtId = "court-1";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        var club = new Club { Id = "club-1", OwnerId = userId };
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsDeleted = false };
        var slot1 = new TimeSlot { Id = "slot-1", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(10, 0) };
        var slot2 = new TimeSlot { Id = "slot-2", Day = new DateOnly(2026, 8, 2), StartTime = new TimeOnly(10, 0) };

        var booking1 = new Booking { Id = "booking-1", CourtId = courtId, Court = court, TimeSlot = slot1, Status = BookingStatus.Confirmed };
        var booking2 = new Booking { Id = "booking-2", CourtId = courtId, Court = court, TimeSlot = slot2, Status = BookingStatus.Confirmed };

        _clubs.Add(club);
        _courts.Add(court);
        _bookings.AddRange(new[] { booking1, booking2 });

        // Act
        var result = await _service.GetCourtBookingsAsync(userId, courtId, filters, null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetCourtBookingsAsync_WhenValidRequestWithDateFilter_ReturnsFilteredBookings()
    {
        // Arrange
        var userId = "owner-1";
        var courtId = "court-1";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        var club = new Club { Id = "club-1", OwnerId = userId };
        var court = new Court { Id = courtId, ClubId = "club-1", Club = club, IsDeleted = false };
        var slot1 = new TimeSlot { Id = "slot-1", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(10, 0) };
        var slot2 = new TimeSlot { Id = "slot-2", Day = new DateOnly(2026, 8, 2), StartTime = new TimeOnly(10, 0) };

        var booking1 = new Booking { Id = "booking-1", CourtId = courtId, Court = court, TimeSlot = slot1, Status = BookingStatus.Confirmed };
        var booking2 = new Booking { Id = "booking-2", CourtId = courtId, Court = court, TimeSlot = slot2, Status = BookingStatus.Confirmed };

        _clubs.Add(club);
        _courts.Add(court);
        _bookings.AddRange(new[] { booking1, booking2 });

        // Act
        var result = await _service.GetCourtBookingsAsync(userId, courtId, filters, new DateOnly(2026, 8, 2), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().ContainSingle(i => i.BookingId == "booking-2");
    }

    #endregion

    #region GetClubBookingsAsync Tests

    [Fact]
    public async Task GetClubBookingsAsync_WithNullUserIdOrClubId_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetClubBookingsAsync("", "club-1", filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetClubBookingsAsync_WithInvalidFilters_ReturnsValidationError()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 0 }; // Invalid: < 1

        // Act
        var result = await _service.GetClubBookingsAsync("owner-1", "club-1", filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation.Filters");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetClubBookingsAsync_WhenClubDoesNotExist_ReturnsClubNotFound()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _service.GetClubBookingsAsync("owner-1", "club-not-exist", filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetClubBookingsAsync_WhenCallerNotClubOwner_ReturnsClubForbidden()
    {
        // Arrange
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };
        var club = new Club { Id = "club-1", OwnerId = "another-owner", IsDeleted = false };
        _clubs.Add(club);

        // Act
        var result = await _service.GetClubBookingsAsync("owner-1", "club-1", filters, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Club.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task GetClubBookingsAsync_WhenValidRequest_ReturnsClubBookings()
    {
        // Arrange
        var userId = "owner-1";
        var clubId = "club-1";
        var filters = new RequestFilters { PageNumber = 1, PageSize = 10 };

        var club = new Club { Id = clubId, OwnerId = userId, IsDeleted = false };
        var court1 = new Court { Id = "court-1", ClubId = clubId, Club = club };
        var court2 = new Court { Id = "court-2", ClubId = clubId, Club = club };

        var slot1 = new TimeSlot { Id = "slot-1", Day = new DateOnly(2026, 8, 1), StartTime = new TimeOnly(10, 0) };
        var slot2 = new TimeSlot { Id = "slot-2", Day = new DateOnly(2026, 8, 2), StartTime = new TimeOnly(10, 0) };

        var booking1 = new Booking { Id = "booking-1", CourtId = "court-1", Court = court1, TimeSlot = slot1, Status = BookingStatus.Confirmed };
        var booking2 = new Booking { Id = "booking-2", CourtId = "court-2", Court = court2, TimeSlot = slot2, Status = BookingStatus.Confirmed };

        _clubs.Add(club);
        _courts.AddRange(new[] { court1, court2 });
        _bookings.AddRange(new[] { booking1, booking2 });

        // Act
        var result = await _service.GetClubBookingsAsync(userId, clubId, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
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
