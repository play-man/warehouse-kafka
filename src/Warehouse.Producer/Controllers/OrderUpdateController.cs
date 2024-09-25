using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Models.Dto;
using Warehouse.Services;

namespace Warehouse.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderUpdateController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public OrderUpdateController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        var orderId = Guid.NewGuid();
        await _warehouseService.CreateOrder(orderId, dto.CustomerName, dto.OrderAmount);
        return Ok(new { Message = "Order created", OrderId = orderId });
    }

    [HttpPut("update-status")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto dto)
    {
        await _warehouseService.UpdateOrderStatus(dto.OrderId, dto.Status);
        return Ok(new { Message = "Order status updated", OrderId = dto.OrderId });
    }

    [HttpPut("change-amount")]
    public async Task<IActionResult> ChangeOrderAmount([FromBody] ChangeOrderAmountDto dto)
    {
        if (dto.NewAmount < 0)
        {
            throw new InvalidOperationException("Order amount cannot be negative.");
        }

        await _warehouseService.ChangeOrderAmount(dto.OrderId, dto.NewAmount);
        return Ok(new { Message = "Order amount updated", OrderId = dto.OrderId });
    }

    [HttpPut("cancel")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderDto dto)
    {
        await _warehouseService.CancelOrder(dto.OrderId);
        return Ok(new { Message = "Order cancelled", OrderId = dto.OrderId });
    }
}