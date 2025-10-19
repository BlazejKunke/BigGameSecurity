using System.Linq;
using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Calculates and applies reputation changes based on gate performance, queue times,
/// and security effectiveness during the event.
/// </summary>
public sealed class ReputationCalculator
{
    private readonly ReputationManager _reputationManager;
    private readonly GateManager _gateManager;

    private TimeSpan? _lastQueueCheckTime;
    private TimeSpan? _lastEfficiencyBonusTime;

    public ReputationCalculator(ReputationManager reputationManager, GateManager gateManager)
    {
        _reputationManager = reputationManager ?? throw new ArgumentNullException(nameof(reputationManager));
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
    }

    /// <summary>
    /// Should be called every update to apply continuous reputation effects like queue penalties.
    /// </summary>
    public void Update(TimeSpan currentGameTime)
    {
        if (ShouldTrigger(ref _lastQueueCheckTime, currentGameTime, TimeSpan.FromMinutes(5)))
        {
            ApplyQueuePenalties();
        }

        if (ShouldTrigger(ref _lastEfficiencyBonusTime, currentGameTime, TimeSpan.FromMinutes(15)))
        {
            ApplyEfficientOperations();
        }
    }

    /// <summary>
    /// Apply reputation bonus when staff successfully detects a threat.
    /// </summary>
    public void ApplyThreatDetected(SecurityGuest guest)
    {
        if (guest is null)
        {
            throw new ArgumentNullException(nameof(guest));
        }

        if (guest.IsMTE)
        {
            _reputationManager.ApplyPositiveEvent("Major threat detained before entry", 20);
        }
        else if (guest.HasFakeId || guest.HasFakeTicket)
        {
            _reputationManager.ApplyPositiveEvent("Fraudulent guest denied entry", 5);
        }
    }

    /// <summary>
    /// Apply reputation penalty when a threat slips through security.
    /// </summary>
    public void ApplyThreatMissed(SecurityGuest guest)
    {
        if (guest is null)
        {
            throw new ArgumentNullException(nameof(guest));
        }

        if (guest.IsMTE)
        {
            _reputationManager.ApplyNegativeEvent("CRITICAL: Major threat entered stadium!", 50);
        }
        else if (guest.HasFakeId)
        {
            _reputationManager.ApplyNegativeEvent("Guest with fake ID entered", 10);
        }
        else if (guest.HasFakeTicket)
        {
            _reputationManager.ApplyNegativeEvent("Guest with fake ticket entered", 8);
        }
    }

    /// <summary>
    /// Apply reputation bonus for efficient operations (high throughput, short queues).
    /// </summary>
    public void ApplyEfficientOperations()
    {
        if (_gateManager.Gates.Count == 0)
        {
            return;
        }

        var avgQueueLength = _gateManager.TotalQueueLength / (float)_gateManager.Gates.Count;

        if (avgQueueLength < 3)
        {
            _reputationManager.ApplyPositiveEvent("Excellent queue management", 2);
        }
    }

    /// <summary>
    /// Apply reputation penalty when gates are not opened/closed at appropriate times.
    /// </summary>
    public void ApplyGateMismanagement(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must be provided.", nameof(description));
        }

        _reputationManager.ApplyNegativeEvent(description, 10);
    }

    private static bool ShouldTrigger(ref TimeSpan? lastTrigger, TimeSpan currentTime, TimeSpan interval)
    {
        if (interval < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be non-negative.");
        }

        if (lastTrigger is null || currentTime < lastTrigger.Value || currentTime - lastTrigger.Value >= interval)
        {
            lastTrigger = currentTime;
            return true;
        }

        return false;
    }

    private void ApplyQueuePenalties()
    {
        var openGates = _gateManager.Gates.Where(g => g.IsOpen).ToList();

        if (openGates.Count == 0)
        {
            return;
        }

        var avgQueueLength = openGates.Average(g => g.QueueLength);

        // Penalties for long queues
        if (avgQueueLength > 50)
        {
            _reputationManager.ApplyNegativeEvent("Extremely long queues causing frustration", 5);
        }
        else if (avgQueueLength > 30)
        {
            _reputationManager.ApplyNegativeEvent("Long queue times", 2);
        }
        else if (avgQueueLength > 20)
        {
            _reputationManager.ApplyNegativeEvent("Queues building up", 1);
        }
    }
}
