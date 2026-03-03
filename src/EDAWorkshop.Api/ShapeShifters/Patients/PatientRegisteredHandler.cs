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
            BSN = "" // We do not want to process BSN anymore even when we receive it in an old event
        };
    }

    public void Handle(PatientRegisteredV2 message)
    {
        _storage[message.PatientId] = new Patient
        {
            PatientId = message.PatientId,
            Name = message.Name,
            DateOfBirth = message.DateOfBirth,
            BSN = ""
        };
    }
}