using GameProject.Game;

namespace GameProject.Tests;

public class ReputationManagerTests
{
    [Fact]
    public void Constructor_SetsStartingScore()
    {
        var manager = new ReputationManager(5);

        Assert.Equal(5, manager.Score);
        Assert.Empty(manager.Events);
    }

    [Fact]
    public void ApplyPositiveEvent_IncreasesScoreAndLogs()
    {
        var manager = new ReputationManager();

        manager.ApplyPositiveEvent("Great service", 10);

        Assert.Equal(10, manager.Score);
        var evt = Assert.Single(manager.Events);
        Assert.Equal(10, evt.Delta);
        Assert.Equal(10, evt.ResultingScore);
        Assert.Equal("Great service", evt.Description);
    }

    [Fact]
    public void ApplyNegativeEvent_DecreasesScoreAndDoesNotGoBelowZero()
    {
        var manager = new ReputationManager(5);

        manager.ApplyNegativeEvent("Long lines", 3);
        Assert.Equal(2, manager.Score);

        manager.ApplyNegativeEvent("Angry guest", 10);
        Assert.Equal(0, manager.Score);

        Assert.Equal(2, manager.Events.Count);
        Assert.Contains(manager.Events, e => e.Description == "Angry guest" && e.ResultingScore == 0);
    }
}
