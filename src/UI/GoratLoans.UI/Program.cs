using System.Text;
using Blazored.LocalStorage;
using GoratLoans.UI.Areas.Identity;
using GoratLoans.UI.Customers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c04cdab52c5248a79126344da513f64536505e149bc7452fbf3baf59f11caf0b"))
    };
});

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddHttpClient();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("Api", (_, client) =>
{
    client.BaseAddress = new Uri("https://localhost:7139/api/");
});
builder.Services.AddScoped(sp => sp.GetService<IHttpClientFactory>()!.CreateClient("Api"));
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
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseCors();

app.Run();
