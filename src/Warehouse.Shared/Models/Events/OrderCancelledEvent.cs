namespace Warehouse.Models;

public class OrderCancelledEvent
{
    public Guid OrderId { get; set; }
    public DateTime CancelledAt { get; set; }
}