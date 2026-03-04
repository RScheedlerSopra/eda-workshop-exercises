namespace EDAWorkshop.Api.ShapeShifters.Appointments;

public record AppointmentScheduledV2(
    Guid AppointmentId,
    Guid PatientId,
    DateTimeOffset ScheduledAt
) : IEvent;
