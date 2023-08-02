using GoratLoans.CRM.Customers;
using Microsoft.EntityFrameworkCore;

namespace GoratLoans.CRM.Api.Customers;

internal static class Configuration
{
    public static void SetupCustomerModel(this ModelBuilder modelBuilder) => modelBuilder.Entity<Customer>();
}