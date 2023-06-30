namespace GoratLoans.Customers;

public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string Address { get; private set; }
    public bool IsVerified { get; private set; }
}