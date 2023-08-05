namespace GoratLoans.CRM.Customers;

public class Customer
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string Address { get; private set; }
    public bool IsVerified { get; private set; }
    public bool IsActive { get; private set; }

    private Customer()
    {
    }

    public Customer(Guid id, string firstName, string lastName, DateOnly birthDate, string address)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
        IsActive = true;
        IsVerified = false;
    }

    public void Verify()
    {
        IsVerified = true;
    }

    public void Suspend()
    {
        IsActive = false;
    }
}