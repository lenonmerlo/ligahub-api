using LigaHub.Domain.Players;

namespace LigaHub.Domain.UnitTests.Players;

public sealed class PlayerTests
{
    [Fact]
    public void Create_ShouldGenerateIdAndAssignTeamAndTrimName()
    {
        var teamId = Guid.NewGuid();

        var player = Player.Create(
            teamId,
            "  Jogador Regional  ");

        Assert.NotEqual(Guid.Empty, player.Id);
        Assert.Equal(teamId, player.TeamId);
        Assert.Equal("Jogador Regional", player.Name);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenTeamIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(Guid.Empty, "Jogador Regional"));

        Assert.Equal("teamId", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(Guid.NewGuid(), name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(Guid.NewGuid(), null!));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var name = new string('a', Player.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(Guid.NewGuid(), name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldAcceptNameAtMaximumLength()
    {
        var name = new string('a', Player.MaxNameLength);

        var player = Player.Create(
            Guid.NewGuid(),
            name);

        Assert.Equal(name, player.Name);
    }

    [Fact]
    public void Rename_ShouldUpdateNameAndPreserveIds()
    {
        var teamId = Guid.NewGuid();
        var player = Player.Create(
            teamId,
            "Old Name");
        var originalId = player.Id;

        player.Rename("  New Name  ");

        Assert.Equal("New Name", player.Name);
        Assert.Equal(originalId, player.Id);
        Assert.Equal(teamId, player.TeamId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Rename_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var player = Player.Create(
            Guid.NewGuid(),
            "Original Name");

        var exception = Assert.Throws<ArgumentException>(
            () => player.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", player.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var player = Player.Create(
            Guid.NewGuid(),
            "Original Name");

        var exception = Assert.Throws<ArgumentException>(
            () => player.Rename(null!));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", player.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var player = Player.Create(
            Guid.NewGuid(),
            "Original Name");
        var name = new string('a', Player.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => player.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", player.Name);
    }
}
