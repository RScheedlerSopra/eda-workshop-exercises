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
            Amount = message.Amount,
            Currency = Currency.EUR // Euros were the only supported currency in the original version. If we skip this line, the enum default value 0 would be used, being USD.
        };

        return Task.CompletedTask;
    }

    public Task Handle(OrderPlacedV2 message)
    {
        _store[message.OrderId] = new OrderSummary
        {
            OrderId = message.OrderId,
            Amount = message.Amount,
            Currency = message.Currency
        };

        return Task.CompletedTask;
    }
}