using GoratLoans.CRM.Api;
using GoratLoans.Users.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddUsersApi(builder.Environment)
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

app.UseUsersApiAsync()
    .UseCustomersApiAsync()
    .Run();