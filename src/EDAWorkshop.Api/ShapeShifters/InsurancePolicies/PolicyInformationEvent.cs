namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

// Event from a fictional third party
public record PolicyInformationEvent(
    Guid PolicyId, 
    string PolicyNumber, 
    string ProductCode, 
    decimal GrossPremium
) : IEvent;
