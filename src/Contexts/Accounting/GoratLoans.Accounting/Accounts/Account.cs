using GoratLoans.Domain;

namespace GoratLoans.CRM.Accounts;

public class Account
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public int NumberOfActiveLoans { get; private set; }
    public Money CurrentBalance { get; private set; }
}