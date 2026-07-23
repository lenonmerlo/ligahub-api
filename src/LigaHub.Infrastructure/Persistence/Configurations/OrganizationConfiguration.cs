using LigaHub.Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LigaHub.Infrastructure.Persistence.Configurations;

internal sealed class OrganizationConfiguration
    : IEntityTypeConfiguration<Organization>
{
    public void Configure(
        EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.HasKey(organization => organization.Id);

        builder.Property(organization => organization.Id)
            .ValueGeneratedNever();

        builder.Property(organization => organization.Name)
            .HasMaxLength(Organization.MaxNameLength)
            .IsRequired();

        builder.HasIndex(organization => organization.Name)
            .IsUnique();
    }
}