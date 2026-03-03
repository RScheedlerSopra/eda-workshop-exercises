namespace EDAWorkshop.Api.ShapeShifters.Appointments;

public class AppointmentScheduledHandler
{
    // In-memory store for simplicity. In a real application, this would likely be a database or a distributed cache.
    private readonly Dictionary<Guid, Appointment> _storage = [];

    public void Handle(AppointmentScheduled message)
    {
        if (!IsBeforeStartOfNextMonth(message.ScheduledAt))
        {
            return;
        }

        _storage[message.AppointmentId] = new Appointment
        {
            AppointmentId = message.AppointmentId,
            PatientId = message.PatientId,
            ScheduledAt = message.ScheduledAt
        };
    }

    private static bool IsBeforeStartOfNextMonth(DateTime scheduledAt)
    {
        var now = DateTime.UtcNow;
        var startOfNextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
        return scheduledAt < startOfNextMonth;
    }
}