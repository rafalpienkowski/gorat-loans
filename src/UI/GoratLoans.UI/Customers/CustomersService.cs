namespace GoratLoans.UI.Customers;

public interface ICustomerService
{
    Task<string> RegisterCustomerAsync(RegisterCustomer customer);

    Task<List<CustomerViewModel>> GetCustomersAsync();
}

public class CustomersService : ICustomerService
{
    private readonly HttpClient _client;

    public CustomersService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> RegisterCustomerAsync(RegisterCustomer customer)
    {
        var response = await _client.PostAsJsonAsync("customers", customer);
        return response.IsSuccessStatusCode ? "ok" : "nok";
    }

    public async Task<List<CustomerViewModel>> GetCustomersAsync()
    {
        var customers = await _client.GetFromJsonAsync<List<CustomerViewModel>>("customers");

        return customers ?? new List<CustomerViewModel>();
    }
}