namespace Dotnet8DifyAgentSample.Models;

using Microsoft.EntityFrameworkCore;

public class SkDbContext : DbContext
{
    public SkDbContext(DbContextOptions<SkDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            // set int Id as auto-increment
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            entity.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");

            entity.Property(e => e.SalePrice)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.SaleCount)
                .IsRequired()
                .HasDefaultValue(0)
                .HasColumnType("int");
        });
    }
}