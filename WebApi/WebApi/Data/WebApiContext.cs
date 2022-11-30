using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Areas.Identity.Data;
using WebApi.Models.DataModels;
using WebApi.Models.DataModels.Interfaces;

namespace WebApi.Data;

public class WebApiContext : IdentityDbContext<WebApiUser>
{
    public WebApiContext(DbContextOptions<WebApiContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(x =>
        {
            x.Property(x => x.CreatedDate)
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.UpdatedDate)
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.Id).UseIdentityColumn();
            x.HasOne(x => x.Creator).WithMany(x => x.CreatedProducts).HasForeignKey("CreatorId");
            x.HasKey(x => x.Id);
            x.ToTable("Products");
        });

        builder.Entity<Project>(x =>
        {
            x.Property(x => x.CreatedDate)
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.UpdatedDate)
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.Id).UseIdentityColumn();
            x.HasOne(x => x.Creator).WithMany(x => x.CreatedProjects).HasForeignKey("CreatorId");
            x.HasMany(x => x.AssignedUsers).WithMany(user => user.AssignedProjects)
                .UsingEntity<Dictionary<string, object>>(
                    "UserProjectAssignments",
                    table => table.HasOne<WebApiUser>()
                            .WithMany().HasForeignKey("UserId"),
                    table => table.HasOne<Project>()
                            .WithMany().HasForeignKey("ProjectId")
                );
            x.HasMany(x => x.AssignedProducts).WithMany(user => user.AssignedProjects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductProjectAssignments",
                    table => table.HasOne<Product>()
                            .WithMany().HasForeignKey("ProductId"),
                    table => table.HasOne<Project>()
                            .WithMany().HasForeignKey("ProjectId")
                );
            x.HasKey(x => x.Id);
            x.ToTable("Projects");

        });
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is ITimeEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((ITimeEntity)entityEntry.Entity).UpdatedDate = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((ITimeEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Product> Projects { get; set; }

    public DbSet<WebApi.Models.DataModels.Project> Project { get; set; } = default!;

}
