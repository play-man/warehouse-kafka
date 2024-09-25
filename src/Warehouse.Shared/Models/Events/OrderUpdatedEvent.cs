namespace Warehouse.Models;

public class OrderUpdatedEvent
{
    public Guid OrderId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; }
}