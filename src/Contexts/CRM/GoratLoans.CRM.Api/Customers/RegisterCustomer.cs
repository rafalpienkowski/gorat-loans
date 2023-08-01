using GoratLoans.CRM.Customers;

namespace GoratLoans.CRM.Api.Customers;

public record RegisterCustomerRequest(
    string? CustomerId,
    string? FirstName,
    string? LastName,
    string? BirthDate,
    string? Address);

public record RegisterCustomer(Guid CustomerId, string FirstName, string LastName, DateOnly BirthDate, string Address)
{
    public static Customer From(string? customerId, string? firstName, string? lastName, string? birthDate,
        string? address)
    {
        if (string.IsNullOrEmpty(firstName)) throw new ArgumentOutOfRangeException(nameof(firstName));
        if (string.IsNullOrEmpty(lastName)) throw new ArgumentOutOfRangeException(nameof(lastName));
        if (string.IsNullOrEmpty(address)) throw new ArgumentOutOfRangeException(nameof(address));
        if (!Guid.TryParse(customerId, out var id)) throw new ArgumentOutOfRangeException(nameof(customerId));
        if (!DateOnly.TryParse(birthDate, out var bDate)) throw new ArgumentOutOfRangeException(nameof(birthDate));

        var today = DateTime.UtcNow;
        if (bDate.AddYears(18) > new DateOnly(today.Year, today.Month, today.Day))
        {
            throw new InvalidOperationException("Only adults can be our customers");
        }

        return new Customer(id, firstName, lastName, bDate, address);
    }
}