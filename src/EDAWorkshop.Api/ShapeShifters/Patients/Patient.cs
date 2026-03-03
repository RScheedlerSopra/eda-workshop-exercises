namespace EDAWorkshop.Api.ShapeShifters.Patients;

public class Patient
{
    public Guid PatientId { get; set; }
    public required string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public required string BSN { get; set; }
}