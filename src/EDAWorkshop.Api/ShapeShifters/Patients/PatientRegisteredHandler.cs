namespace EDAWorkshop.Api.ShapeShifters.Patients;

public class PatientRegisteredHandler
{
    // In-memory store for simplicity. In a real application, this would likely be a database or a distributed cache.
    private readonly Dictionary<Guid, Patient> _storage = [];

    public void Handle(PatientRegistered message)
    {
        _storage[message.PatientId] = new Patient
        {
            PatientId = message.PatientId,
            Name = message.Name,
            DateOfBirth = message.DateOfBirth,
            BSN = message.BSN
        };
    }
}