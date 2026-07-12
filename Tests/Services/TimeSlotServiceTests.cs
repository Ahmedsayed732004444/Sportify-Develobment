using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sportiva.Abstractions;
using Sportiva.Contracts.TimeSlots;
using Sportiva.Entities;
using Sportiva.Enums;
using Sportiva.Persistence;
using Sportiva.Services.Implementation;
using Xunit;

namespace Sportiva.Tests.Services;

public class TimeSlotServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly TimeSlotService _service;

    public TimeSlotServiceTests()
    {
        // Use a unique in-memory database name per test to ensure isolation
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new TimeSlotService(_context);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    #region Helper Methods

    private async Task<Court> CreateTestCourt(string courtId, string clubId, string clubOwnerId)
    {
        var club = new Club
        {
            Id = clubId,
            OwnerId = clubOwnerId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            MaxCapacity = 20,
            IsActive = true,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.SaveChangesAsync();

        return court;
    }

    private CreateTimeSlotRequest CreateTimeSlotRequest(DateOnly day, int startHour, int endHour)
    {
        return new CreateTimeSlotRequest(
            "court-id",
            day,
            new TimeOnly(startHour, 0),
            new TimeOnly(endHour, 0)
        );
    }

    #endregion

    #region CreateTimeSlotAsync Tests

    [Fact]
    public async Task CreateTimeSlotAsync_WithValidRequest_CreatesSlotSuccessfully()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var request = new CreateTimeSlotRequest(courtId, futureDate, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TimeSlotId.Should().NotBeNullOrEmpty();
        result.Value.IsBooked.Should().BeFalse();

        var savedSlot = await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.Id == result.Value.TimeSlotId);
        savedSlot.Should().NotBeNull();
        savedSlot.StartTime.Should().Be(new TimeOnly(10, 0));
        savedSlot.EndTime.Should().Be(new TimeOnly(11, 0));
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithNullUserId_ReturnsValidationError()
    {
        // Arrange
        var courtId = "court-123";
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var request = new CreateTimeSlotRequest(courtId, futureDate, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(null!, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.UserId");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithInvalidTimeRange_ReturnsValidationError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var request = new CreateTimeSlotRequest(courtId, futureDate, new TimeOnly(11, 0), new TimeOnly(10, 0)); // End before start

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.TimeRange");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithSlotTooShort_ReturnsValidationError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var request = new CreateTimeSlotRequest(courtId, futureDate, new TimeOnly(10, 0), new TimeOnly(10, 5)); // 5 minutes < 15 min minimum

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.SlotDuration");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithSlotInThePast_ReturnsValidationError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var request = new CreateTimeSlotRequest(courtId, pastDate, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.SlotInPast");
        result.Error.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WhenCourtDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "nonexistent-court";
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var request = new CreateTimeSlotRequest(courtId, futureDate, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Court.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WhenUserDoesNotOwnCourt_ReturnsForbidden()
    {
        // Arrange
        var userId = "owner-001";
        var otherOwnerId = "owner-999";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, otherOwnerId);

        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var request = new CreateTimeSlotRequest(courtId, futureDate, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Court.Forbidden");
        result.Error.StatusCode.Should().Be(403);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithExactlyOverlappingSlot_ReturnsOverlappingError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        var court = await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var existingSlot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(existingSlot);
        await _context.SaveChangesAsync();

        var request = new CreateTimeSlotRequest(courtId, date, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.Overlapping");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithPartiallyOverlappingSlot_ReturnsOverlappingError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var existingSlot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(existingSlot);
        await _context.SaveChangesAsync();

        // New slot starts inside existing slot (10:30 - 11:30)
        var request = new CreateTimeSlotRequest(courtId, date, new TimeOnly(10, 30), new TimeOnly(11, 30));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.Overlapping");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_WithContainingSlot_ReturnsOverlappingError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var existingSlot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(existingSlot);
        await _context.SaveChangesAsync();

        // New slot fully contains existing slot (9:00 - 12:00)
        var request = new CreateTimeSlotRequest(courtId, date, new TimeOnly(9, 0), new TimeOnly(12, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.Overlapping");
        result.Error.StatusCode.Should().Be(409);
    }

    [Fact]
    public async Task CreateTimeSlotAsync_OnDifferentCourt_DoesNotOverlap()
    {
        // Arrange
        var userId = "owner-001";
        var courtId1 = "court-123";
        var courtId2 = "court-456";
        var clubId = "club-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = userId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court1 = new Court
        {
            Id = courtId1,
            ClubId = clubId,
            Name = "Court 1",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var court2 = new Court
        {
            Id = courtId2,
            ClubId = clubId,
            Name = "Court 2",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court1);
        await _context.Courts.AddAsync(court2);
        await _context.SaveChangesAsync();

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var existingSlot = new TimeSlot
        {
            CourtId = courtId1,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(existingSlot);
        await _context.SaveChangesAsync();

        // New slot on different court at same time
        var request = new CreateTimeSlotRequest(courtId2, date, new TimeOnly(10, 0), new TimeOnly(11, 0));

        // Act
        var result = await _service.CreateTimeSlotAsync(userId, courtId2, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    #endregion

    #region BulkCreateTimeSlotsAsync Tests

    [Fact]
    public async Task BulkCreateTimeSlotsAsync_WithValidRequests_CreatesAllSlotsAtomically()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var requests = new List<CreateTimeSlotRequest>
        {
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(9, 0), new TimeOnly(10, 0)),
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(10, 0), new TimeOnly(11, 0)),
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(11, 0), new TimeOnly(12, 0))
        };

        // Act
        var result = await _service.BulkCreateTimeSlotsAsync(userId, courtId, requests.AsReadOnly(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        var savedSlots = await _context.TimeSlots.Where(ts => ts.CourtId == courtId).ToListAsync();
        savedSlots.Should().HaveCount(3);
    }

    [Fact]
    public async Task BulkCreateTimeSlotsAsync_WhenOneSlotOverlapsExisting_RollsBackAll()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        // Create an existing slot
        var existingSlot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(15, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(existingSlot);
        await _context.SaveChangesAsync();

        // Try to create batch with overlapping slot
        var requests = new List<CreateTimeSlotRequest>
        {
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(9, 0), new TimeOnly(10, 0)),
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(14, 30), new TimeOnly(15, 30)), // Overlaps existing
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(16, 0), new TimeOnly(17, 0))
        };

        // Act
        var result = await _service.BulkCreateTimeSlotsAsync(userId, courtId, requests.AsReadOnly(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.Overlapping");

        // Verify no new slots were created (transaction rolled back)
        var allSlots = await _context.TimeSlots.Where(ts => ts.CourtId == courtId).ToListAsync();
        allSlots.Should().HaveCount(1); // Only the original existing slot
    }

    [Fact]
    public async Task BulkCreateTimeSlotsAsync_WhenTwoSlotsInBatchOverlap_ReturnsError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        // Batch with overlapping slots
        var requests = new List<CreateTimeSlotRequest>
        {
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(9, 0), new TimeOnly(10, 0)),
            new CreateTimeSlotRequest(courtId, date, new TimeOnly(9, 30), new TimeOnly(11, 0)) // Overlaps previous
        };

        // Act
        var result = await _service.BulkCreateTimeSlotsAsync(userId, courtId, requests.AsReadOnly(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.Overlapping");

        // Verify nothing was created
        var slots = await _context.TimeSlots.Where(ts => ts.CourtId == courtId).ToListAsync();
        slots.Should().BeEmpty();
    }

    [Fact]
    public async Task BulkCreateTimeSlotsAsync_WithEmptyBatch_ReturnsValidationError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var requests = new List<CreateTimeSlotRequest>();

        // Act
        var result = await _service.BulkCreateTimeSlotsAsync(userId, courtId, requests.AsReadOnly(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Validation.BatchEmpty");
        result.Error.StatusCode.Should().Be(400);
    }

    #endregion

    #region DeleteTimeSlotAsync Tests

    [Fact]
    public async Task DeleteTimeSlotAsync_WithUnbookedSlot_SoftDeletesSuccessfully()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(slot);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteTimeSlotAsync(userId, courtId, slot.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var deletedSlot = await _context.TimeSlots.IgnoreQueryFilters().FirstOrDefaultAsync(ts => ts.Id == slot.Id);
        deletedSlot.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteTimeSlotAsync_WithBookedSlot_ReturnsCannot_DeleteError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";
        var bookingUserId = "user-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = userId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var booking = new Booking
        {
            CourtId = courtId,
            UserId = bookingUserId,
            TimeSlotId = slot.Id,
            TimeSlot = slot,
            BookingDate = DateTime.UtcNow,
            Price = 100m,
            Status = BookingStatus.Confirmed,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(slot);
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteTimeSlotAsync(userId, courtId, slot.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.HasActiveBooking");
        result.Error.StatusCode.Should().Be(409);

        // Verify slot was NOT deleted
        var notDeletedSlot = await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.Id == slot.Id);
        notDeletedSlot.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteTimeSlotAsync_WhenSlotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        // Act
        var result = await _service.DeleteTimeSlotAsync(userId, courtId, "nonexistent-slot", CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    #endregion

    #region UpdateTimeSlotAsync Tests

    [Fact]
    public async Task UpdateTimeSlotAsync_WithUnbookedSlot_UpdatesSuccessfully()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(slot);
        await _context.SaveChangesAsync();

        var newRequest = new CreateTimeSlotRequest(courtId, date, new TimeOnly(14, 0), new TimeOnly(15, 0));

        // Act
        var result = await _service.UpdateTimeSlotAsync(userId, courtId, slot.Id, newRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedSlot = await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.Id == slot.Id);
        updatedSlot.StartTime.Should().Be(new TimeOnly(14, 0));
        updatedSlot.EndTime.Should().Be(new TimeOnly(15, 0));
    }

    [Fact]
    public async Task UpdateTimeSlotAsync_WithBookedSlot_ReturnsCannotUpdateError()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";
        var bookingUserId = "user-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = userId,
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var booking = new Booking
        {
            CourtId = courtId,
            UserId = bookingUserId,
            TimeSlotId = slot.Id,
            TimeSlot = slot,
            BookingDate = DateTime.UtcNow,
            Price = 100m,
            Status = BookingStatus.Pending,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(slot);
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var newRequest = new CreateTimeSlotRequest(courtId, date, new TimeOnly(14, 0), new TimeOnly(15, 0));

        // Act
        var result = await _service.UpdateTimeSlotAsync(userId, courtId, slot.Id, newRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.HasActiveBooking");
        result.Error.StatusCode.Should().Be(409);

        // Verify slot was NOT updated
        var notUpdatedSlot = await _context.TimeSlots.FirstOrDefaultAsync(ts => ts.Id == slot.Id);
        notUpdatedSlot.StartTime.Should().Be(new TimeOnly(10, 0));
    }

    [Fact]
    public async Task UpdateTimeSlotAsync_ExcludesSelfFromOverlapCheck()
    {
        // Arrange
        var userId = "owner-001";
        var courtId = "court-123";
        var clubId = "club-456";

        await CreateTestCourt(courtId, clubId, userId);

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.TimeSlots.AddAsync(slot);
        await _context.SaveChangesAsync();

        // Update slot to slightly different time (no actual overlap with any other slot)
        var newRequest = new CreateTimeSlotRequest(courtId, date, new TimeOnly(10, 15), new TimeOnly(11, 15));

        // Act
        var result = await _service.UpdateTimeSlotAsync(userId, courtId, slot.Id, newRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.StartTime.Should().Be(new TimeOnly(10, 15));
    }

    #endregion

    #region GetTimeSlotAsync Tests

    [Fact]
    public async Task GetTimeSlotAsync_WithValidSlot_ReturnsSlot()
    {
        // Arrange
        var courtId = "court-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var slot = new TimeSlot
        {
            CourtId = courtId,
            Day = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(slot);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTimeSlotAsync(courtId, slot.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TimeSlotId.Should().Be(slot.Id);
        result.Value.Court.CourtId.Should().Be(courtId);
    }

    [Fact]
    public async Task GetTimeSlotAsync_WhenSlotDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var courtId = "court-123";
        var slotId = "nonexistent-slot";

        // Act
        var result = await _service.GetTimeSlotAsync(courtId, slotId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("TimeSlot.NotFound");
        result.Error.StatusCode.Should().Be(404);
    }

    #endregion

    #region GetTimeSlotsAsync Tests

    [Fact]
    public async Task GetTimeSlotsAsync_WithSpecificDate_ReturnsOnlyThatDaySlots()
    {
        // Arrange
        var courtId = "court-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var date1 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));
        var date2 = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6));

        var slot1 = new TimeSlot { CourtId = courtId, Day = date1, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };
        var slot2 = new TimeSlot { CourtId = courtId, Day = date2, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(slot1);
        await _context.TimeSlots.AddAsync(slot2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTimeSlotsAsync(courtId, date1, null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Day.Should().Be(date1);
    }

    [Fact]
    public async Task GetTimeSlotsAsync_WithAvailableFilter_ReturnsOnlyUnbookedFutureSlots()
    {
        // Arrange
        var courtId = "court-123";
        var clubId = "club-456";
        var bookingUserId = "user-789";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        var unbooked = new TimeSlot { CourtId = courtId, Day = date, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };
        var booked = new TimeSlot { CourtId = courtId, Day = date, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(15, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };

        var booking = new Booking
        {
            CourtId = courtId,
            UserId = bookingUserId,
            TimeSlotId = booked.Id,
            TimeSlot = booked,
            BookingDate = DateTime.UtcNow,
            Price = 100m,
            Status = BookingStatus.Confirmed,
            IsDeleted = false
        };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(unbooked);
        await _context.TimeSlots.AddAsync(booked);
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTimeSlotsAsync(courtId, date, true, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].IsBooked.Should().BeFalse();
    }

    [Fact]
    public async Task GetTimeSlotsAsync_WithoutDateAndOutsideLookahead_ExcludesFutureSlots()
    {
        // Arrange
        var courtId = "court-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var withinLookahead = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15));
        var outsideLookahead = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(60));

        var slot1 = new TimeSlot { CourtId = courtId, Day = withinLookahead, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };
        var slot2 = new TimeSlot { CourtId = courtId, Day = outsideLookahead, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(slot1);
        await _context.TimeSlots.AddAsync(slot2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTimeSlotsAsync(courtId, null, null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Day.Should().Be(withinLookahead);
    }

    [Fact]
    public async Task GetTimeSlotsAsync_ReturnsResultsSortedByStartTimeAscending()
    {
        // Arrange
        var courtId = "court-123";
        var clubId = "club-456";

        var club = new Club
        {
            Id = clubId,
            OwnerId = "owner-001",
            Name = "Test Club",
            IsActive = true,
            IsDeleted = false
        };

        var court = new Court
        {
            Id = courtId,
            ClubId = clubId,
            Club = club,
            Name = "Test Court",
            PricePerHour = 100m,
            SportType = SportType.Football,
            IsActive = true,
            IsDeleted = false
        };

        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5));

        var slot1 = new TimeSlot { CourtId = courtId, Day = date, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(15, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };
        var slot2 = new TimeSlot { CourtId = courtId, Day = date, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };
        var slot3 = new TimeSlot { CourtId = courtId, Day = date, StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(13, 0), CreatedAt = DateTime.UtcNow, IsDeleted = false };

        await _context.Clubs.AddAsync(club);
        await _context.Courts.AddAsync(court);
        await _context.TimeSlots.AddAsync(slot1);
        await _context.TimeSlots.AddAsync(slot2);
        await _context.TimeSlots.AddAsync(slot3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetTimeSlotsAsync(courtId, date, null, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
        result.Value[0].StartTime.Should().Be(new TimeOnly(10, 0));
        result.Value[1].StartTime.Should().Be(new TimeOnly(12, 0));
        result.Value[2].StartTime.Should().Be(new TimeOnly(14, 0));
    }

    #endregion
}
