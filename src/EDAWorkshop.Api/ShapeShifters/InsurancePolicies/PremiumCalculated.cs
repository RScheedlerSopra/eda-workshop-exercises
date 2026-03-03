
namespace EDAWorkshop.Api.ShapeShifters.Policies;

public record PremiumCalculatedV2(
    Guid PolicyId, 
    decimal GrossPremium
) : IEvent;