namespace Warehouse.Controllers;

public class CreateOrderDto
{
    public string CustomerName { get; set; }
    public decimal OrderAmount { get; set; }
}