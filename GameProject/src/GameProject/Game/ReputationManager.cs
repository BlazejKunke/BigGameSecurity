using System.Collections.ObjectModel;

namespace GameProject.Game;

/// <summary>
/// Tracks the park's reputation score and stores a log of events that impact it.
/// </summary>
public sealed class ReputationManager
{
    private readonly List<ReputationEvent> _events = new();

    public ReputationManager(int startingScore = 0)
    {
        if (startingScore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startingScore), "Starting score cannot be negative.");
        }

        Score = startingScore;
    }

    /// <summary>
    /// Current cumulative reputation score.
    /// </summary>
    public int Score { get; private set; }

    /// <summary>
    /// Historical log of adjustments.
    /// </summary>
    public IReadOnlyList<ReputationEvent> Events => new ReadOnlyCollection<ReputationEvent>(_events);

    public void ApplyPositiveEvent(string description, int amount)
    {
        ApplyDelta(description, amount, isPositive: true);
    }

    public void ApplyNegativeEvent(string description, int amount)
    {
        ApplyDelta(description, amount, isPositive: false);
    }

    private void ApplyDelta(string description, int amount, bool isPositive)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must be provided.", nameof(description));
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
        }

        var delta = isPositive ? amount : -amount;
        Score = Math.Max(0, Score + delta);

        _events.Add(new ReputationEvent(DateTime.UtcNow, description, delta, Score));
    }
}

/// <summary>
/// Immutable record describing a single reputation change.
/// </summary>
/// <param name="Timestamp">The time the change was recorded.</param>
/// <param name="Description">Short description of the event.</param>
/// <param name="Delta">How much the reputation changed (+/-).</param>
/// <param name="ResultingScore">The score after applying the change.</param>
public sealed record ReputationEvent(DateTime Timestamp, string Description, int Delta, int ResultingScore);
