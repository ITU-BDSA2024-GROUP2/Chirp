using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<Author>
{
    public DbSet<Cheep> Cheeps { get; set; }

    public DbSet<Author> Authors { get; set; }
    public DbSet<Like> Likes { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {

    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Author>()
            .HasIndex(c => c.UserName)
            .IsUnique();
        modelBuilder.Entity<Author>()
            .HasIndex(c => c.Email)
            .IsUnique();
    }
}