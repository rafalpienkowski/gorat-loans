namespace GoratLoans.Identity.Api;

public static class DatabaseMigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication host)
    {
        using var scope = host.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<IdentitiesDbContext>();

        if (!dbContext.Identities.Any(u => u.Username.ToLowerInvariant() == "admin"))
        {
            AddAdminAccount(dbContext);
        }
        
        return host;
    }

    private static void AddAdminAccount(IdentitiesDbContext dbContext)
    {
        PasswordService.CreatePasswordHash("admin", out var passwordHash, out var passwordSalt);
        dbContext.Identities.Add(
            new Identity
            {
                Id = Guid.NewGuid(), IsAdmin = true, Username = "Admin", PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            });
        dbContext.SaveChanges();
    }
}