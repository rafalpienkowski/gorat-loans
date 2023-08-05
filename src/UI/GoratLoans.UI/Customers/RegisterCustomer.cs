using System.ComponentModel.DataAnnotations;

namespace GoratLoans.UI.Customers;

public class RegisterCustomer
{
    public Guid CustomerId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string LastName { get; set; }

    [Required] public DateOnly BirthDate { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Address { get; set; }
}