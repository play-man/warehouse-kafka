namespace Warehouse.Models;

public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderAmount { get; set; }
}