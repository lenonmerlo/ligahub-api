using LigaHub.Domain.Organizations;
using LigaHub.Domain.Teams;
using Microsoft.EntityFrameworkCore;

namespace LigaHub.Infrastructure.Persistence;

public sealed class LigaHubDbContext(
    DbContextOptions<LigaHubDbContext> options)
    : DbContext(options)
{
    public DbSet<Organization> Organizations =>
        Set<Organization>();

    public DbSet<Team> Teams =>
        Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(LigaHubDbContext).Assembly);
    }
}