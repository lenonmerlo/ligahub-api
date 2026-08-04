using LigaHub.Domain.Teams;

namespace LigaHub.Domain.UnitTests.Teams;

public sealed class TeamTests
{
    [Fact]
    public void Create_ShouldGenerateIdAndAssignOrganizationAndTrimName()
    {
        var organizationId = Guid.NewGuid();

        var team = Team.Create(
            organizationId,
            "  Time Regional  ");

        Assert.NotEqual(Guid.Empty, team.Id);
        Assert.Equal(organizationId, team.OrganizationId);
        Assert.Equal("Time Regional", team.Name);
        Assert.Equal(Sport.Volleyball, team.Sport);
    }

    [Theory]
    [InlineData(Sport.Volleyball)]
    [InlineData(Sport.Football)]
    [InlineData(Sport.Basketball)]
    public void Create_ShouldAssignSport_WhenSportIsValid(
    Sport sport)
    {
        var team = Team.Create(
            Guid.NewGuid(),
            "Time Regional",
            sport);

        Assert.Equal(sport, team.Sport);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenSportIsInvalid(
        int sportValue)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => Team.Create(
                Guid.NewGuid(),
                "Time Regional",
                (Sport)sportValue));

        Assert.Equal("sport", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenOrganizationIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Team.Create(Guid.Empty, "Time Regional"));

        Assert.Equal("organizationId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Team.Create(Guid.NewGuid(), name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Team.Create(Guid.NewGuid(), null!));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var name = new string('a', Team.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => Team.Create(Guid.NewGuid(), name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldAcceptNameAtMaximumLength()
    {
        var name = new string('a', Team.MaxNameLength);

        var team = Team.Create(
            Guid.NewGuid(),
            name);

        Assert.Equal(name, team.Name);
    }

    [Fact]
    public void Rename_ShouldUpdateNameAndPreserveIds()
    {
        var organizationId = Guid.NewGuid();
        var team = Team.Create(
            organizationId,
            "Old Name");
        var originalId = team.Id;

        team.Rename("  New Name  ");

        Assert.Equal("New Name", team.Name);
        Assert.Equal(originalId, team.Id);
        Assert.Equal(organizationId, team.OrganizationId);
        Assert.Equal(Sport.Volleyball, team.Sport);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Rename_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var team = Team.Create(
            Guid.NewGuid(),
            "Original Name");

        var exception = Assert.Throws<ArgumentException>(
            () => team.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", team.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var team = Team.Create(
            Guid.NewGuid(),
            "Original Name");

        var exception = Assert.Throws<ArgumentException>(
            () => team.Rename(null!));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", team.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var team = Team.Create(
            Guid.NewGuid(),
            "Original Name");
        var name = new string('a', Team.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => team.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", team.Name);
    }
}
