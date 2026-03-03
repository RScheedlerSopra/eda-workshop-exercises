namespace EDAWorkshop.Api.ShapeShifters.Orders;

public record OrderPlaced(Guid OrderId, decimal Amount) : IEvent;