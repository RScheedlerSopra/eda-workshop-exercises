namespace EDAWorkshop.Api.ShapeShifters.Orders;

public class OrderPlacedHandler
{
    // In-memory store for simplicity. In a real application, this would likely be a database or a distributed cache.
    private readonly Dictionary<Guid, OrderSummary> _store = [];

    public Task Handle(OrderPlaced message)
    {
        _store[message.OrderId] = new OrderSummary
        {
            OrderId = message.OrderId,
            Amount = message.Amount
        };

        return Task.CompletedTask;
    }
}