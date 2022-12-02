using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.Areas.Identity.Data;
using WebApi.Models.DataModels;
using WebApi.Models.DataModels.Interfaces;

namespace WebApi.Data;

public class WebApiContext : IdentityDbContext<WebApiUser, WebApiRole, Guid>
{
    public WebApiContext(DbContextOptions<WebApiContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WebApiRole>(x =>
        {
            x.HasData(
                new WebApiRole { Id = Guid.NewGuid(), Name = "Admin" },
                new WebApiRole { Id = Guid.NewGuid(), Name = "Client" });
        });

        builder.Entity<WebApiUser>(x =>
        {
            x.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("newsequentialid()");
            x.Property(x => x.CreatedDate)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("getdate()");
        });

        builder.Entity<Product>(x =>
        {
            x.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("newsequentialid()");
            x.Property(x => x.CreatedDate)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("getdate()");
            x.HasOne(x => x.Creator)
                .WithMany(x => x.CreatedProducts)
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.NoAction);
            x.HasKey(x => x.Id);
            x.ToTable("Products");
        });

        builder.Entity<Project>(x =>
        {
            x.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("newsequentialid()");
            x.Property(x => x.CreatedDate)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("getdate()");
            x.Property(x => x.UpdatedDate)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("getdate()");
            x.HasOne(x => x.Creator)
                .WithMany(x => x.CreatedProjects)
                .HasForeignKey("CreatorId")
                .OnDelete(DeleteBehavior.NoAction);
            x.HasMany(x => x.AssignedUsers)
                .WithMany(user => user.AssignedProjects)
                .UsingEntity<Dictionary<string, object>>(
                    "UserProjectAssignments",
                    table => table.HasOne<WebApiUser>()
                            .WithMany()
                            .HasForeignKey("UserId")
                            .OnDelete(DeleteBehavior.Cascade),
                    table => table.HasOne<Project>()
                            .WithMany()
                            .HasForeignKey("ProjectId")
                            .OnDelete(DeleteBehavior.Cascade)
                );
            x.HasMany(x => x.AssignedProducts)
                .WithMany(user => user.AssignedProjects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductProjectAssignments",
                    table => table.HasOne<Product>()
                            .WithMany()
                            .HasForeignKey("ProductId")
                            .OnDelete(DeleteBehavior.Cascade),
                    table => table.HasOne<Project>()
                            .WithMany()
                            .HasForeignKey("ProjectId")
                            .OnDelete(DeleteBehavior.Cascade)
                );
            x.HasKey(x => x.Id);
            x.ToTable("Projects");
        });
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is ITimeEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

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

    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<Project> Projects { get; set; } = default!;
}
