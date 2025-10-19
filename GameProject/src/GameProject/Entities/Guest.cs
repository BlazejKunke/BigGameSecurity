namespace GameProject.Entities;

/// <summary>
/// Represents a guest visiting the stadium. Guests can queue at a gate and their patience can
/// decrease over time if they have to wait too long.
/// </summary>
public sealed class Guest : Entity
{
    public Guest(string name, TicketType ticketType, int partySize, TimeSpan patience) : base(name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Guests must have a name.", nameof(name));
        }

        if (partySize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partySize), "Party size must be at least one.");
        }

        if (patience <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(patience), "Patience must be a positive value.");
        }

        TicketType = ticketType;
        PartySize = partySize;
        PatienceRemaining = patience;
        InitialPatience = patience;
        Id = Guid.NewGuid();
        ArrivalTime = DateTime.UtcNow;
    }

    public Guid Id { get; }

    public TicketType TicketType { get; }

    public int PartySize { get; }

    /// <summary>
    /// Records the moment the guest object was created/arrived.
    /// </summary>
    public DateTime ArrivalTime { get; }

    /// <summary>
    /// Total patience allotted when the guest joined the queue.
    /// </summary>
    public TimeSpan InitialPatience { get; }

    /// <summary>
    /// Remaining patience before the guest becomes upset and leaves.
    /// </summary>
    public TimeSpan PatienceRemaining { get; private set; }

    /// <summary>
    /// Tracks the gate the guest is currently waiting for, if any.
    /// </summary>
    public Gate? CurrentGate { get; private set; }

    /// <summary>
    /// Whether the guest has lost all patience and left the queue.
    /// </summary>
    public bool HasLeftQueue => PatienceRemaining == TimeSpan.Zero;

    public void Wait(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), "Guests cannot wait a negative amount of time.");
        }

        var remaining = PatienceRemaining - duration;
        PatienceRemaining = remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
    }

    public void RefreshPatience()
    {
        PatienceRemaining = InitialPatience;
    }

    internal void AssignGate(Gate gate)
    {
        CurrentGate = gate;
    }

    internal void LeaveGate()
    {
        CurrentGate = null;
    }
}

public enum TicketType
{
    GeneralAdmission,
    Vip,
    SeasonPass,
    StaffGuest
}
