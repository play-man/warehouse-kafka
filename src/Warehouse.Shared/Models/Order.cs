namespace Warehouse.Shared.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; }
    public decimal OrderAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string Status { get; set; }
}
