using GoratLoans.Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoratLoans.Identity.Api;

public class UserDto
{
    public string Name { get; set; }
    public string Password { get; set; }
}

public static class WebApplicationExtensions
{
    public static IServiceCollection AddUsersApi(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            services.AddDbContext<IdentitiesDbContext>(options => options.UseNpgsql("name=ConnectionStrings:UsersDB"));
        }
        else
        {
            services.AddDbContext<IdentitiesDbContext>(options => options.UseInMemoryDatabase("UsersDB"));
        }

        return services;
    }

    public static WebApplication UseUsersApiAsync(this WebApplication app)
    {
        app.MigrateDatabase();

        app.MapPost(
                "/api/identity/register",
                async (
                    [FromServices] IdentitiesDbContext dbContext,
                    UserDto request,
                    CancellationToken ct
                ) =>
                {
                    PasswordService.CreatePasswordHash(request.Password, out var passwordHash,
                        out var passwordSalt);
                    var user = new Identity
                    {
                        Username = request.Name,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };
                    try
                    {
                        await dbContext.AddAndSave(user, ct);
                        return Results.Created($"/api/identity/{user.Username}", user.Username);
                    }
                    catch (Exception ex)
                    {
                        return Results.BadRequest(ex.Message);
                    }
                }
            )
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status201Created);

        app.MapPost(
                "/api/identity/login",
                async (
                    [FromServices] IdentitiesDbContext dDbContext,
                    UserDto request
                ) =>
                {
                    var user = await dDbContext.Identities.SingleOrDefaultAsync(u => u.Username == request.Name);
                    if (user == null)
                    {
                        return Results.BadRequest("Can't log in");
                    }

                    var isValidPassword =
                        PasswordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

                    if (!isValidPassword)
                    {
                        return Results.BadRequest("Can't log in");
                    }

                    var token = TokenService.CreateToken(user,
                        "c04cdab52c5248a79126344da513f64536505e149bc7452fbf3baf59f11caf0b");

                    return Results.Ok(token);
                }
            )
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status200OK);

        return app;
    }
}