namespace EDAWorkshop.Api.ShapeShifters.Orders;

public record OrderPlacedV2(Guid OrderId, decimal Amount, Currency Currency) : IEvent;