namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

public record PolicyCreatedV2(
    Guid PolicyId, 
    string PolicyNumber, 
    string ProductCode
) : IEvent;