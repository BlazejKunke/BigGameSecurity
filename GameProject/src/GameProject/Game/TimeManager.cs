namespace GameProject.Game;

/// <summary>
/// Tracks the current in-game time and total elapsed time for the simulation.
/// </summary>
public sealed class TimeManager
{
    public TimeManager(DateTime startTime)
    {
        CurrentTime = startTime;
        StartTime = startTime;
    }

    /// <summary>
    /// Gets the absolute start time of the simulation.
    /// </summary>
    public DateTime StartTime { get; }

    /// <summary>
    /// Current time of day in the simulation.
    /// </summary>
    public DateTime CurrentTime { get; private set; }

    /// <summary>
    /// Total simulated time that has passed since the TimeManager was created.
    /// </summary>
    public TimeSpan Elapsed => CurrentTime - StartTime;

    /// <summary>
    /// Event that fires whenever time is advanced.
    /// </summary>
    public event EventHandler<TimeSpan>? TimeAdvanced;

    /// <summary>
    /// Advances the current time by the specified duration.
    /// </summary>
    public void Advance(TimeSpan delta)
    {
        if (delta < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Cannot advance time by a negative value.");
        }

        CurrentTime = CurrentTime.Add(delta);
        TimeAdvanced?.Invoke(this, delta);
    }
}
