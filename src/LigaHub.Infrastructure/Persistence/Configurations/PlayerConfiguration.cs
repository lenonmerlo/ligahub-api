using LigaHub.Domain.Players;
using LigaHub.Domain.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LigaHub.Infrastructure.Persistence.Configurations;

internal sealed class PlayerConfiguration
    : IEntityTypeConfiguration<Player>
{
    public void Configure(
        EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(player => player.Id);

        builder.Property(player => player.Id)
            .ValueGeneratedNever();

        builder.Property(player => player.TeamId)
            .IsRequired();

        builder.Property(player => player.Name)
            .HasMaxLength(Player.MaxNameLength)
            .IsRequired();

        builder.Property(player => player.BirthDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(player => player.Sex)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(player => player.JerseyNumber)
            .IsRequired();

        builder.HasIndex(player => new
        {
            player.TeamId,
            player.JerseyNumber
        }).IsUnique();

        builder.HasOne<Team>()
            .WithMany()
            .HasForeignKey(player => player.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
