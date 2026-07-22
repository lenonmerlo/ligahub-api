using LigaHub.Domain.Organizations;

namespace LigaHub.Domain.UnitTests.Organizations;

public sealed class OrganizationTests
{
    [Fact]
    public void Create_ShouldGenerateIdAndTrimName()
    {
        var organization = Organization.Create("  Liga Regional  ");

        Assert.NotEqual(Guid.Empty, organization.Id);
        Assert.Equal("Liga Regional", organization.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Organization.Create(name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Organization.Create(null!));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var name = new string('a', Organization.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => Organization.Create(name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldAcceptNameAtMaximumLength()
    {
        var name = new string('a', Organization.MaxNameLength);

        var organization = Organization.Create(name);

        Assert.Equal(name, organization.Name);
    }

    [Fact]
    public void Rename_ShouldUpdateNameAndPreserveId()
    {
        var organization = Organization.Create("Old Name");
        var originalId = organization.Id;

        organization.Rename("  New Name  ");

        Assert.Equal("New Name", organization.Name);
        Assert.Equal(originalId, organization.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Rename_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var organization = Organization.Create("Original Name");

        var exception = Assert.Throws<ArgumentException>(
            () => organization.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", organization.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var organization = Organization.Create("Original Name");

        var exception = Assert.Throws<ArgumentException>(
            () => organization.Rename(null!));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", organization.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var organization = Organization.Create("Original Name");
        var name = new string('a', Organization.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(() => organization.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", organization.Name);
    }
}