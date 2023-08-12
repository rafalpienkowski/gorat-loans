using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace GoratLoans.UI.Areas.Identity;

public interface IAuthService
{
    Task<LoginResult> Login(LoginModel loginModel);
    Task Logout();
    Task<RegisterResult> Register(RegisterModel registerModel);
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        var result = await _httpClient.PostAsJsonAsync("/accounts/register", registerModel);

        result.EnsureSuccessStatusCode();

        return new RegisterResult();
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var loginAsJson = JsonSerializer.Serialize(loginModel);
        var response = await _httpClient.PostAsync("users/login", new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return new LoginResult
            {
                Successful = false,
                Error = error
            };
        }
        var token = await response.Content.ReadAsStringAsync();

        await _localStorage.SetItemAsync("token", token);
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginModel.Name);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

        var loginResult = new LoginResult
        {
            Successful = true,
            Token = token
        };
        
        return loginResult;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("token");
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}

public class LoginResult
{
    public bool Successful { get; set; }
    public string Error { get; set; }
    public string Token { get; set; }
}

public class RegisterResult
{
    public bool Successful { get; set; }
    public IEnumerable<string> Errors { get; set; }
}

public class LoginModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Password { get; set; }
}

public class RegisterModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}