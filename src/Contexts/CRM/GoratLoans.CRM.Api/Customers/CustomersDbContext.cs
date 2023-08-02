using GoratLoans.CRM.Api.Customers;
using GoratLoans.CRM.Customers;
using Microsoft.EntityFrameworkCore;

namespace GoratLoans.CRM.Adapters.Customers;

public class CustomersDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    public CustomersDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.SetupCustomerModel();
    }
}