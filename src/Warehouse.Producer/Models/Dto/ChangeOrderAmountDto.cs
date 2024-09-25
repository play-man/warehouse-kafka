namespace Warehouse.Controllers;

public class ChangeOrderAmountDto
{
    public Guid OrderId { get; set; }
    public decimal NewAmount { get; set; }
}