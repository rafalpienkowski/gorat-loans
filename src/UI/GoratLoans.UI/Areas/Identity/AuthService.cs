using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using GoratLoans.UI.Areas.Identity.Pages.Account;
using GoratLoans.UI.Areas.Identity.Pages.Account.Login;
using GoratLoans.UI.Areas.Identity.Pages.Account.Register;
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
        var result = await _httpClient.PostAsJsonAsync("identity/register", registerModel);

        if (result.IsSuccessStatusCode)
        {
            return new RegisterResult
            {
                Successful = true
            };
        }

        return new RegisterResult
        {
            Successful = false,
            Errors = new[] { await result.Content.ReadAsStringAsync() }
        };
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var loginAsJson = JsonSerializer.Serialize(loginModel);
        var response = await _httpClient.PostAsync("identity/login",
            new StringContent(loginAsJson, Encoding.UTF8, "application/json"));
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