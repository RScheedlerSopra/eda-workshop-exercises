namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

public record PolicyCreated(
    Guid PolicyId, 
    string PolicyNumber, 
    string ProductCode
) : IEvent;