using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt): base(opt)
    {
        
    }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands {get; set;}

    protected override void OnModelCreating(ModelBuilder bldr){
        bldr.Entity<Platform>()
            .HasMany(c => c.Commands)
            .WithOne(p => p.Platform)
            .HasForeignKey(p => p.PlatformId);

        bldr.Entity<Command>()
            .HasOne(p => p.Platform)
            .WithMany(c => c.Commands)
            .HasForeignKey(p => p.PlatformId);
    }
}   