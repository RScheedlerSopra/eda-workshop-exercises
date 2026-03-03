namespace EDAWorkshop.Api.ShapeShifters.Orders;

public class OrderSummary
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
}