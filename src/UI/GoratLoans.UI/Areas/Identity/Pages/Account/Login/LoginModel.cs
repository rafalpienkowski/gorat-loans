using System.ComponentModel.DataAnnotations;

namespace GoratLoans.UI.Areas.Identity.Pages.Account.Login;

public class LoginModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Password { get; set; }
}