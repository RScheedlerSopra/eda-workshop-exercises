namespace EDAWorkshop.Api.ShapeShifters.Appointments;

public record AppointmentScheduled(
    Guid AppointmentId, 
    Guid PatientId, 
    DateTime ScheduledAt
) : IEvent;