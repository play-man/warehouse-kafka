using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Warehouse.Consumer;

using Order = Shared.Models.Order;

public class ApplicationDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}