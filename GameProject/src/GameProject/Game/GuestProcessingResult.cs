using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Represents the outcome of processing a guest at a security gate.
/// </summary>
public sealed class GuestProcessingResult
{
    public ProcessingOutcome Result { get; init; }
    public SecurityGuest? Guest { get; init; }

    public static readonly GuestProcessingResult NoGuestsToProcess = new()
    {
        Result = ProcessingOutcome.NoGuestsInQueue,
        Guest = null
    };
}

public enum ProcessingOutcome
{
    NoGuestsInQueue,
    AllowedEntry,
    ThreatDetected,
    ThreatMissed
}
