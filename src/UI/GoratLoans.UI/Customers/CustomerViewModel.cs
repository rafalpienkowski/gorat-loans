namespace GoratLoans.UI.Customers;

public class CustomerViewModel
{
    public Guid CustomerId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Address { get; set; }
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
}