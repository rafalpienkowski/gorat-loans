using GoratLoans.UI.Customers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(opt => opt
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin());
});
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("CRMApi", (_, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7139/api/");
});
builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>()!.CreateClient("CRMApi"));
builder.Services.AddScoped<ICustomerService, CustomersService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseCors();

app.Run();
