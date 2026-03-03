namespace EDAWorkshop.Api.ShapeShifters.Patients;

public record PatientRegistered(
    Guid PatientId,
    string Name,
    DateTime DateOfBirth,
    string BSN
) : IEvent;