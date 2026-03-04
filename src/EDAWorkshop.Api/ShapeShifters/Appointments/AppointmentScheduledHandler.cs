namespace EDAWorkshop.Api.ShapeShifters.Appointments;

public class AppointmentScheduledHandler
{
    // In-memory store for simplicity. In a real application, this would likely be a database or a distributed cache.
    private readonly Dictionary<Guid, Appointment> _storage = [];

    public void Handle(AppointmentScheduled message)
    {
        // V1: ScheduledAt is local time, convert to UTC for consistent storage
        var scheduledAtUtc = message.ScheduledAt.ToUniversalTime();

        if (!IsBeforeStartOfNextMonth(scheduledAtUtc))
        {
            return;
        }

        _storage[message.AppointmentId] = new Appointment
        {
            AppointmentId = message.AppointmentId,
            PatientId = message.PatientId,
            ScheduledAt = scheduledAtUtc
        };
    }

    public void Handle(AppointmentScheduledV2 message)
    {
        // V2: ScheduledAt is already UTC
        var scheduledAtUtc = message.ScheduledAt.UtcDateTime;

        if (!IsBeforeStartOfNextMonth(scheduledAtUtc))
        {
            return;
        }

        _storage[message.AppointmentId] = new Appointment
        {
            AppointmentId = message.AppointmentId,
            PatientId = message.PatientId,
            ScheduledAt = scheduledAtUtc
        };
    }

    public Appointment? GetAppointment(Guid appointmentId) =>
        _storage.TryGetValue(appointmentId, out var appointment) ? appointment : null;

    private static bool IsBeforeStartOfNextMonth(DateTime scheduledAt)
    {
        var now = DateTime.UtcNow;
        var startOfNextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
        return scheduledAt < startOfNextMonth;
    }
}