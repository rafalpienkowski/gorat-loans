using GoratLoans.CRM.Api;
using GoratLoans.Identity.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddIdentityApi(builder.Environment)
    .AddCustomersApi(false);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
    app.UseHsts();
}

app.UseIdentityApiAsync()
    .UseCustomersApiAsync()
    .Run();