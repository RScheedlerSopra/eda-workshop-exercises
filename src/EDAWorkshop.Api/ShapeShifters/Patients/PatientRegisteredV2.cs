namespace EDAWorkshop.Api.ShapeShifters.Patients;

public record PatientRegisteredV2(
    Guid PatientId,
    string Name,
    DateTime DateOfBirth
) : IEvent;