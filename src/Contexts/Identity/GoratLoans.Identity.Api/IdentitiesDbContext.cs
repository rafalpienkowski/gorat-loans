using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoratLoans.Identity.Api;

internal class Identity
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public bool IsAdmin { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}


internal class IdentityConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder
            .HasIndex(u => u.Username)
            .IsUnique();
    }
}

internal class IdentitiesDbContext : DbContext
{
    public DbSet<Identity> Identities { get; set; }

    public IdentitiesDbContext(DbContextOptions<IdentitiesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new IdentityConfiguration());
    }
}