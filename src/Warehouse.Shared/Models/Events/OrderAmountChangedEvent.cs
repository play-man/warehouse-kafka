namespace Warehouse.Models;

public class OrderAmountChangedEvent
{
    public Guid OrderId { get; set; }
    public decimal NewAmount { get; set; }
    public DateTime UpdatedAt { get; set; }
}