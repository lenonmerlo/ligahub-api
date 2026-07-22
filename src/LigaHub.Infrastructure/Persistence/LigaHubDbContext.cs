using LigaHub.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.Persistence;

public sealed class LigaHubDbContext(
    DbContextOptions<LigaHubDbContext> options)
    : DbContext(options)
{
    public DbSet<Organization> Organizations =>
        Set<Organization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LigaHubDbContext).Assembly);
    }
}