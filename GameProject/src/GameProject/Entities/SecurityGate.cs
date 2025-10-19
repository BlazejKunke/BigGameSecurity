using System;
using System.Collections.Generic;
using System.Linq;

namespace GameProject.Entities;

/// <summary>
/// Represents a specialized security gate that tracks throughput and incident detection metrics.
/// </summary>
public sealed class SecurityGate : Entity
{
    public SecurityGate(int id, bool isOpen = true)
        : base($"Security Gate {id}")
    {
        if (id is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Gate id must be between 1 and 12.");
        }

        Id = id;
        IsOpen = isOpen;
        AssignedStaff = new List<SecurityStaff>();
        GuestQueue = new Queue<SecurityGuest>();
    }

    public int Id { get; }

    public bool IsOpen { get; set; }

    public List<SecurityStaff> AssignedStaff { get; }

    public Queue<SecurityGuest> GuestQueue { get; }

    public int TotalProcessed { get; private set; }

    public int IncidentsDetected { get; private set; }

    public int IncidentsMissed { get; private set; }

    public int QueueLength => GuestQueue.Count;

    public float AverageObservation
    {
        get
        {
            if (AssignedStaff.Count == 0)
            {
                return 0;
            }

            return AssignedStaff.Average(s => s.Observation);
        }
    }

    public SecurityProcessingResult? ProcessNextGuest()
    {
        if (GuestQueue.Count == 0 || AssignedStaff.Count == 0)
        {
            return null;
        }

        var guest = GuestQueue.Dequeue();
        TotalProcessed++;

        if (!guest.IsHighRisk)
        {
            guest.CurrentState = GuestState.WatchingGame;
            return new SecurityProcessingResult(guest, SecurityProcessingOutcome.None);
        }

        var detected = CheckDetection(guest);

        if (detected)
        {
            IncidentsDetected++;
            guest.CurrentState = guest.IsMTE ? GuestState.AtJail : GuestState.Denied;
            return new SecurityProcessingResult(guest, SecurityProcessingOutcome.ThreatDetected);
        }

        IncidentsMissed++;
        guest.CurrentState = GuestState.WatchingGame;
        return new SecurityProcessingResult(guest, SecurityProcessingOutcome.ThreatMissed);
    }

    private bool CheckDetection(SecurityGuest guest)
    {
        if (AssignedStaff.Count == 0)
        {
            return false;
        }

        var observationFactor = Math.Clamp(AverageObservation / 10f, 0f, 1f);
        var focusFactor = Math.Clamp(AssignedStaff.Average(s => s.CurrentFocus) / 100f, 0f, 1f);
        var detectionChance = observationFactor * focusFactor;

        return Random.Shared.NextDouble() < detectionChance;
    }
}

public enum SecurityProcessingOutcome
{
    None,
    ThreatDetected,
    ThreatMissed
}

public sealed record SecurityProcessingResult(SecurityGuest Guest, SecurityProcessingOutcome Outcome);
