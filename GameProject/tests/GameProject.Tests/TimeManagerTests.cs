using GameProject.Game;

namespace GameProject.Tests;

public class TimeManagerTests
{
    [Fact]
    public void Constructor_SetsStartTime()
    {
        var start = new DateTime(2024, 1, 1, 6, 0, 0, DateTimeKind.Utc);
        var manager = new TimeManager(start);

        Assert.Equal(start, manager.StartTime);
        Assert.Equal(start, manager.CurrentTime);
        Assert.Equal(TimeSpan.Zero, manager.Elapsed);
    }

    [Fact]
    public void Advance_IncrementsCurrentTimeAndRaisesEvent()
    {
        var start = new DateTime(2024, 1, 1, 6, 0, 0, DateTimeKind.Utc);
        var manager = new TimeManager(start);
        var delta = TimeSpan.FromMinutes(15);
        var eventTriggered = false;

        manager.TimeAdvanced += (_, d) =>
        {
            eventTriggered = true;
            Assert.Equal(delta, d);
        };

        manager.Advance(delta);

        Assert.Equal(start + delta, manager.CurrentTime);
        Assert.Equal(delta, manager.Elapsed);
        Assert.True(eventTriggered);
    }
}
