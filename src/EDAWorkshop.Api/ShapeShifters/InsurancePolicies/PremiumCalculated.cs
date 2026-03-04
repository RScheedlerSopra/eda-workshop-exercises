
namespace EDAWorkshop.Api.ShapeShifters.InsurancePolicies;

public record PremiumCalculated(
    Guid PolicyId, 
    decimal GrossPremium
) : IEvent;