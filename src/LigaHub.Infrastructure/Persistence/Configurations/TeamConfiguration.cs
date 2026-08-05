using LigaHub.Domain.Organizations;
using LigaHub.Domain.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LigaHub.Infrastructure.Persistence.Configurations;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(
        EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(team => team.Id);

        builder.Property(team => team.Id).ValueGeneratedNever();

        builder.Property(team => team.OrganizationId).IsRequired();

        builder.Property(team => team.Sport)
            .HasConversion<int>()
            .HasDefaultValue(Sport.Volleyball)
            .HasSentinel((Sport)0)
            .IsRequired();

        builder.Property(team => team.Name)
            .HasMaxLength(Team.MaxNameLength)
            .IsRequired();

        builder.HasIndex(team => new
        {
            team.OrganizationId,
            team.Name
        }).IsUnique();

        builder.HasOne<Organization>()
            .WithMany()
            .HasForeignKey(team => team.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

