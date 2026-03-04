using EDAWorkshop.Api.ShapeShifters.Appointments;

namespace EDAWorkshop.Tests.ShapeShifters;

public class AppointmentScheduledHandlerTests
{
    private readonly AppointmentScheduledHandler _handler = new();

    // CET = UTC+1 in winter (standard time), UTC+2 in summer (daylight saving / CEST)
    private static readonly TimeZoneInfo CetTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    // ---- V1 (local time) ----

    [Fact]
    public void Handle_V1_StoresAppointment_WhenScheduledAtIsBeforeStartOfNextMonth()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        // A date safely in the past is always before start of next month
        var localTime = new DateTime(2020, 6, 15, 9, 0, 0, DateTimeKind.Local);
        var message = new AppointmentScheduled(appointmentId, patientId, localTime);

        // Act
        _handler.Handle(message);

        // Assert
        var appointment = _handler.GetAppointment(appointmentId);
        Assert.NotNull(appointment);
        Assert.Equal(appointmentId, appointment.AppointmentId);
        Assert.Equal(patientId, appointment.PatientId);
    }

    [Fact]
    public void Handle_V1_DoesNotStoreAppointment_WhenScheduledAtIsAfterStartOfNextMonth()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var localTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Local);
        var message = new AppointmentScheduled(appointmentId, Guid.NewGuid(), localTime);

        // Act
        _handler.Handle(message);

        // Assert
        Assert.Null(_handler.GetAppointment(appointmentId));
    }

    [Fact]
    public void Handle_V1_ConvertsLocalTimeToUtc_WhenStoringScheduledAt()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var localTime = new DateTime(2020, 3, 10, 12, 0, 0, DateTimeKind.Local);
        var expectedUtc = localTime.ToUniversalTime();
        var message = new AppointmentScheduled(appointmentId, Guid.NewGuid(), localTime);

        // Act
        _handler.Handle(message);

        // Assert
        var appointment = _handler.GetAppointment(appointmentId);
        Assert.NotNull(appointment);
        Assert.Equal(expectedUtc, appointment.ScheduledAt);
        Assert.Equal(DateTimeKind.Utc, appointment.ScheduledAt.Kind);
    }

    // ---- V2 (UTC via DateTimeOffset) ----

    [Fact]
    public void Handle_V2_StoresAppointment_WhenScheduledAtIsBeforeStartOfNextMonth()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var utcTime = new DateTimeOffset(2020, 6, 15, 9, 0, 0, TimeSpan.Zero);
        var message = new AppointmentScheduledV2(appointmentId, patientId, utcTime);

        // Act
        _handler.Handle(message);

        // Assert
        var appointment = _handler.GetAppointment(appointmentId);
        Assert.NotNull(appointment);
        Assert.Equal(appointmentId, appointment.AppointmentId);
        Assert.Equal(patientId, appointment.PatientId);
    }

    [Fact]
    public void Handle_V2_DoesNotStoreAppointment_WhenScheduledAtIsAfterStartOfNextMonth()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var utcTime = new DateTimeOffset(9999, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var message = new AppointmentScheduledV2(appointmentId, Guid.NewGuid(), utcTime);

        // Act
        _handler.Handle(message);

        // Assert
        Assert.Null(_handler.GetAppointment(appointmentId));
    }

    [Fact]
    public void Handle_V2_StoresUtcDateTime_WhenScheduledAtHasNonZeroOffset()
    {
        // Arrange: a time expressed as +02:00 (e.g. CET summer time)
        var appointmentId = Guid.NewGuid();
        var offsetTime = new DateTimeOffset(2020, 3, 10, 14, 0, 0, TimeSpan.FromHours(2));
        var expectedUtc = offsetTime.UtcDateTime; // 12:00 UTC
        var message = new AppointmentScheduledV2(appointmentId, Guid.NewGuid(), offsetTime);

        // Act
        _handler.Handle(message);

        // Assert
        var appointment = _handler.GetAppointment(appointmentId);
        Assert.NotNull(appointment);
        Assert.Equal(expectedUtc, appointment.ScheduledAt);
        Assert.Equal(DateTimeKind.Utc, appointment.ScheduledAt.Kind);
    }

    // ---- V1 CET DST edge cases ----

    [Fact]
    public void Handle_V1_ConvertsWinterCetTimeToUtc_WhenOffsetIsPlus1()
    {
        // Arrange: CET winter (standard time) is UTC+1
        // January is always in standard time, so local 10:00 CET = 09:00 UTC
        var appointmentId = Guid.NewGuid();
        var winterCetOffset = CetTimeZone.GetUtcOffset(new DateTime(2020, 1, 15)); // +01:00
        Assert.Equal(TimeSpan.FromHours(1), winterCetOffset); // guard: ensure this is indeed winter

        var cetWinterTime = new DateTimeOffset(2020, 1, 15, 10, 0, 0, winterCetOffset);
        // Convert to the local DateTime as the V1 producer would have provided it
        var localWinterTime = cetWinterTime.LocalDateTime;
        var expectedUtc = new DateTime(2020, 1, 15, 9, 0, 0, DateTimeKind.Utc);

        var message = new AppointmentScheduled(appointmentId, Guid.NewGuid(), localWinterTime);

        // Act
        _handler.Handle(message);

        // Assert
        var appointment = _handler.GetAppointment(appointmentId);
        Assert.NotNull(appointment);
        Assert.Equal(expectedUtc, appointment.ScheduledAt);
        Assert.Equal(DateTimeKind.Utc, appointment.ScheduledAt.Kind);
    }

    [Fact]
    public void Handle_V1_ConvertsSummerCestTimeToUtc_WhenOffsetIsPlus2()
    {
        // Arrange: CET summer (daylight saving / CEST) is UTC+2
        // July is always in daylight saving time, so local 10:00 CEST = 08:00 UTC
        var appointmentId = Guid.NewGuid();
        var summerCetOffset = CetTimeZone.GetUtcOffset(new DateTime(2020, 7, 15)); // +02:00
        Assert.Equal(TimeSpan.FromHours(2), summerCetOffset); // guard: ensure this is indeed summer

        var cetSummerTime = new DateTimeOffset(2020, 7, 15, 10, 0, 0, summerCetOffset);
        // Convert to the local DateTime as the V1 producer would have provided it
        var localSummerTime = cetSummerTime.LocalDateTime;
        var expectedUtc = new DateTime(2020, 7, 15, 8, 0, 0, DateTimeKind.Utc);

        var message = new AppointmentScheduled(appointmentId, Guid.NewGuid(), localSummerTime);

        // Act
        _handler.Handle(message);

        // Assert
        var appointment = _handler.GetAppointment(appointmentId);
        Assert.NotNull(appointment);
        Assert.Equal(expectedUtc, appointment.ScheduledAt);
        Assert.Equal(DateTimeKind.Utc, appointment.ScheduledAt.Kind);
    }

    // ---- Boundary: first of next month ----

    [Fact]
    public void Handle_V1_StoresAppointment_WhenScheduledAtIsLastSecondOfCurrentMonth()
    {
        // Arrange: one second before the start of next month should still be stored
        var appointmentId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var lastSecondOfThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddMonths(1)
            .AddSeconds(-1);
        var message = new AppointmentScheduled(appointmentId, Guid.NewGuid(), lastSecondOfThisMonth);

        // Act
        _handler.Handle(message);

        // Assert: strictly before the boundary → stored
        Assert.NotNull(_handler.GetAppointment(appointmentId));
    }

    [Fact]
    public void Handle_V1_DoesNotStoreAppointment_WhenScheduledAtIsExactlyStartOfNextMonth()
    {
        // Arrange: exactly midnight on the first of next month should NOT be stored
        // The check is strict: scheduledAt < startOfNextMonth, so equal is excluded
        var appointmentId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var exactStartOfNextMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddMonths(1);
        var message = new AppointmentScheduled(appointmentId, Guid.NewGuid(), exactStartOfNextMonth);

        // Act
        _handler.Handle(message);

        // Assert: equal to the boundary → not stored
        Assert.Null(_handler.GetAppointment(appointmentId));
    }

    [Fact]
    public void Handle_V2_StoresAppointment_WhenScheduledAtIsLastSecondOfCurrentMonth()
    {
        // Arrange: one second before the start of next month should still be stored
        var appointmentId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var lastSecondOfThisMonth = new DateTimeOffset(
            new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddMonths(1)
                .AddSeconds(-1),
            TimeSpan.Zero);
        var message = new AppointmentScheduledV2(appointmentId, Guid.NewGuid(), lastSecondOfThisMonth);

        // Act
        _handler.Handle(message);

        // Assert: strictly before the boundary → stored
        Assert.NotNull(_handler.GetAppointment(appointmentId));
    }

    [Fact]
    public void Handle_V2_DoesNotStoreAppointment_WhenScheduledAtIsExactlyStartOfNextMonth()
    {
        // Arrange: exactly midnight on the first of next month should NOT be stored
        // The check is strict: scheduledAt < startOfNextMonth, so equal is excluded
        var appointmentId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var exactStartOfNextMonth = new DateTimeOffset(
            new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1),
            TimeSpan.Zero);
        var message = new AppointmentScheduledV2(appointmentId, Guid.NewGuid(), exactStartOfNextMonth);

        // Act
        _handler.Handle(message);

        // Assert: equal to the boundary → not stored
        Assert.Null(_handler.GetAppointment(appointmentId));
    }
}
