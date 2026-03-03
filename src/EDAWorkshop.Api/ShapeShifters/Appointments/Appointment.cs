namespace EDAWorkshop.Api.ShapeShifters.Appointments;

public class Appointment
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime ScheduledAt { get; set; }   
}