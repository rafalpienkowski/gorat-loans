using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GoratLoans.Infrastructure.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace GoratLoans.Users.Api;

public class UserDto
{
    public string Name { get; set; }
    public string Password { get; set; }
}

internal class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public bool IsAdmin { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}

internal class UsersDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsersConfiguration());
    }
}

internal class UsersConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasIndex(u => u.Username)
            .IsUnique();

        PasswordService.CreatePasswordHash("admin", out var passwordHash, out var passwordSalt);
        builder
            .HasData(new User
            {
                Id = Guid.NewGuid(), IsAdmin = true, Username = "Admin", PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
    }
}

internal static class PasswordService
{
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
}

internal static class TokenService
{
    public static string CreateToken(User user, string signingToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username)
        };
        if (user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingToken));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: cred);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication host)
    {
        using var scope = host.Services.CreateScope();
        using var appContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        PasswordService.CreatePasswordHash("admin", out var passwordHash, out var passwordSalt);
        appContext.Users.Add(
            new User
            {
                Id = Guid.NewGuid(), IsAdmin = true, Username = "Admin", PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
        appContext.SaveChanges();
        return host;
    }
}

public static class WebApplicationExtensions
{
    public static IServiceCollection AddUsersApi(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (environment.IsProduction())
        {
            services.AddDbContext<UsersDbContext>(options => options.UseNpgsql("name=ConnectionStrings:UsersDB"));
        }
        else
        {
            services.AddDbContext<UsersDbContext>(options => options.UseInMemoryDatabase("UsersDB"));
        }

        return services;
    }


    public static WebApplication UseUsersApiAsync(this WebApplication app)
    {
        app.MigrateDatabase();

        app.MapPost(
                "/api/users/register",
                async (
                    [FromServices] UsersDbContext dbContext,
                    UserDto request,
                    CancellationToken ct
                ) =>
                {
                    PasswordService.CreatePasswordHash(request.Password, out var passwordHash,
                        out var passwordSalt);
                    var user = new User
                    {
                        Username = request.Name,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt
                    };
                    try
                    {
                        await dbContext.AddAndSave(user, ct);
                        return Results.Created($"/api/users/{user.Username}", user.Username);
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
                "/api/users/login",
                async (
                    [FromServices] UsersDbContext dDbContext,
                    UserDto request
                ) =>
                {
                    var user = await dDbContext.Users.SingleOrDefaultAsync(u => u.Username == request.Name);
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