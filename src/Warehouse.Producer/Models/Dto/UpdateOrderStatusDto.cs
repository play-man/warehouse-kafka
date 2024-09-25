namespace Warehouse.Models.Dto;

public class UpdateOrderStatusDto
{
    public Guid OrderId { get; set; }
    public string Status { get; set; }
}