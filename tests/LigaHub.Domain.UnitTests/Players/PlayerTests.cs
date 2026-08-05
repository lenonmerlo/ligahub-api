using LigaHub.Domain.Players;

namespace LigaHub.Domain.UnitTests.Players;

public sealed class PlayerTests
{
    private static readonly DateOnly ValidBirthDate =
        new(2000, 1, 1);

    [Fact]
    public void Create_ShouldGenerateIdAndAssignTeamAndDetailsAndTrimName()
    {
        var teamId = Guid.NewGuid();

        var player = Player.Create(
            teamId,
            "  Jogador Regional  ",
            ValidBirthDate,
            Sex.Male,
            10);

        Assert.NotEqual(Guid.Empty, player.Id);
        Assert.Equal(teamId, player.TeamId);
        Assert.Equal("Jogador Regional", player.Name);
        Assert.Equal(ValidBirthDate, player.BirthDate);
        Assert.Equal(Sex.Male, player.Sex);
        Assert.Equal(10, player.JerseyNumber);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenTeamIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(
                Guid.Empty,
                "Jogador Regional",
                ValidBirthDate,
                Sex.Male,
                10));

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
            () => Player.Create(
                Guid.NewGuid(),
                name,
                ValidBirthDate,
                Sex.Male,
                10));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(
                Guid.NewGuid(),
                null!,
                ValidBirthDate,
                Sex.Male,
                10));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var name = new string('a', Player.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(
                Guid.NewGuid(),
                name,
                ValidBirthDate,
                Sex.Male,
                10));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldAcceptNameAtMaximumLength()
    {
        var name = new string('a', Player.MaxNameLength);

        var player = Player.Create(
            Guid.NewGuid(),
            name,
            ValidBirthDate,
            Sex.Female,
            99);

        Assert.Equal(name, player.Name);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenBirthDateIsDefault()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => Player.Create(
                Guid.NewGuid(),
                "Jogador Regional",
                default,
                Sex.Male,
                10));

        Assert.Equal("birthDate", exception.ParamName);
    }

    [Fact]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenBirthDateIsInFuture()
    {
        var futureBirthDate = DateOnly
            .FromDateTime(DateTime.UtcNow)
            .AddDays(1);

        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => Player.Create(
                Guid.NewGuid(),
                "Jogador Regional",
                futureBirthDate,
                Sex.Male,
                10));

        Assert.Equal("birthDate", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenSexIsInvalid(
        int sexValue)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => Player.Create(
                Guid.NewGuid(),
                "Jogador Regional",
                ValidBirthDate,
                (Sex)sexValue,
                10));

        Assert.Equal("sex", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Create_ShouldThrowArgumentOutOfRangeException_WhenJerseyNumberIsInvalid(
        int jerseyNumber)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(
            () => Player.Create(
                Guid.NewGuid(),
                "Jogador Regional",
                ValidBirthDate,
                Sex.Male,
                jerseyNumber));

        Assert.Equal("jerseyNumber", exception.ParamName);
    }

    [Theory]
    [InlineData(Player.MinJerseyNumber)]
    [InlineData(Player.MaxJerseyNumber)]
    public void Create_ShouldAcceptJerseyNumberAtBoundaries(
        int jerseyNumber)
    {
        var player = Player.Create(
            Guid.NewGuid(),
            "Jogador Regional",
            ValidBirthDate,
            Sex.Female,
            jerseyNumber);

        Assert.Equal(jerseyNumber, player.JerseyNumber);
    }

    [Fact]
    public void Rename_ShouldUpdateNameAndPreserveDetails()
    {
        var teamId = Guid.NewGuid();
        var player = Player.Create(
            teamId,
            "Old Name",
            ValidBirthDate,
            Sex.Female,
            7);
        var originalId = player.Id;

        player.Rename("  New Name  ");

        Assert.Equal("New Name", player.Name);
        Assert.Equal(originalId, player.Id);
        Assert.Equal(teamId, player.TeamId);
        Assert.Equal(ValidBirthDate, player.BirthDate);
        Assert.Equal(Sex.Female, player.Sex);
        Assert.Equal(7, player.JerseyNumber);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Rename_ShouldThrowArgumentException_WhenNameIsEmpty(
        string name)
    {
        var player = CreateValidPlayer();

        var exception = Assert.Throws<ArgumentException>(
            () => player.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", player.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameIsNull()
    {
        var player = CreateValidPlayer();

        var exception = Assert.Throws<ArgumentException>(
            () => player.Rename(null!));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", player.Name);
    }

    [Fact]
    public void Rename_ShouldThrowArgumentException_WhenNameExceedsMaximumLength()
    {
        var player = CreateValidPlayer();
        var name = new string('a', Player.MaxNameLength + 1);

        var exception = Assert.Throws<ArgumentException>(
            () => player.Rename(name));

        Assert.Equal("name", exception.ParamName);
        Assert.Equal("Original Name", player.Name);
    }

    private static Player CreateValidPlayer()
    {
        return Player.Create(
            Guid.NewGuid(),
            "Original Name",
            ValidBirthDate,
            Sex.Male,
            10);
    }
}
