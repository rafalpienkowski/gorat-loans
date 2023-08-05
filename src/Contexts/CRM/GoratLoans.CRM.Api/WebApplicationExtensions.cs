using GoratLoans.CRM.Api.Customers;
using GoratLoans.Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoratLoans.CRM.Api;

public static class WebApplicationExtensions
{
    public static void AddCustomersApi(this IServiceCollection services, bool isProduction)
    {
        if (isProduction)
        {
            services.AddDbContext<CustomersDbContext>(
                options => options.UseNpgsql("name=ConnectionStrings:CustomersDB"));
        }
        else
        {
            services.AddDbContext<CustomersDbContext>(
                options => options.UseInMemoryDatabase("CustomersDB"));
        }
    }

    public static void UseCustomersApiAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            //using var scope = app.Services.CreateScope();
            //await using var dbContext = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
            //await dbContext.Database.MigrateAsync();
        }

        app.MapPost(
                "/api/customers/",
                async (
                    [FromServices] CustomersDbContext dbContext,
                    RegisterCustomerRequest request,
                    CancellationToken ct
                ) =>
                {
                    var (customerId, firstName, lastName, birthDate, address) = request;
                    var customer = RegisterCustomer.From(customerId, firstName, lastName, birthDate, address);
                    await dbContext.AddAndSave(customer, ct);
                    return Results.Created($"/api/customers/{customerId}", customerId);
                }
            )
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        app.MapGet("/api/customers",
            ([FromServices] CustomersDbContext dbContext, CancellationToken ct) =>
                dbContext.Customers.AsNoTracking().ToListAsync(ct)).Produces(StatusCodes.Status200OK);
    }
}