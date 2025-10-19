using System;
using System.Linq;
using GameProject.Entities;
using GameProject.Game;

namespace GameProject.Scenes;

/// <summary>
/// Drives the core evening event simulation where guests arrive, are processed through security,
/// and incidents can influence the park's reputation score.
/// </summary>
public sealed class EventScene : IScene
{
    private readonly TimeManager _timeManager;
    private readonly GateManager _gateManager;
    private readonly GuestFlowManager _guestFlowManager;
    private readonly IncidentManager _incidentManager;
    private readonly ReputationManager _reputationManager;
    private readonly int _defaultGuestCount;

    private bool _eventCompleted;

    public EventScene(
        TimeManager timeManager,
        GateManager gateManager,
        GuestFlowManager guestFlowManager,
        IncidentManager incidentManager,
        ReputationManager reputationManager,
        int defaultGuestCount = 600)
    {
        _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
        _guestFlowManager = guestFlowManager ?? throw new ArgumentNullException(nameof(guestFlowManager));
        _incidentManager = incidentManager ?? throw new ArgumentNullException(nameof(incidentManager));
        _reputationManager = reputationManager ?? throw new ArgumentNullException(nameof(reputationManager));

        if (defaultGuestCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(defaultGuestCount), "Guest count must be non-negative.");
        }

        _defaultGuestCount = defaultGuestCount;
    }

    public string Name => "Event Execution";

    public void LoadContent()
    {
        Console.WriteLine("Preparing for the main event (18:00 - 24:00)...");
        _gateManager.OpenAllGates();

        if (_guestFlowManager.AllGuests.Count == 0 && _defaultGuestCount > 0)
        {
            _guestFlowManager.GenerateGuests(_defaultGuestCount);
            Console.WriteLine($"Generated {_defaultGuestCount} guests for tonight's crowd.");
        }
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        if (deltaTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");
        }

        var acceleratedDelta = Multiply(deltaTime, 6);

        _timeManager.Advance(acceleratedDelta);
        _guestFlowManager.Update(_timeManager.CurrentTime.TimeOfDay, acceleratedDelta);
        _incidentManager.Update(acceleratedDelta);

        var hoursElapsed = (float)acceleratedDelta.TotalHours;

        if (hoursElapsed > 0)
        {
            foreach (var gate in _gateManager.Gates)
            {
                foreach (var staff in gate.AssignedStaff)
                {
                    staff.UpdateFocus(hoursElapsed);
                }
            }
        }

        if (_reputationManager.Score <= 0)
        {
            Console.WriteLine("\n💀 GAME OVER: You've been fired!");
            Environment.Exit(0);
        }

        if (!_eventCompleted && HasEventFinished())
        {
            _eventCompleted = true;
            Console.WriteLine("\n✓ Event complete!");
            // Transition to a post-event scene could occur here when the scene system supports it.
        }
    }

    public void Render()
    {
        Console.Clear();

        Console.WriteLine($"Time: {_timeManager.CurrentTime:HH\\:mm}");
        Console.WriteLine($"Reputation: {_reputationManager.Score}");
        Console.WriteLine($"Total Queue: {_gateManager.TotalQueueLength}");
        Console.WriteLine($"Guests processed: {_guestFlowManager.TotalGuestsProcessed}");
        Console.WriteLine($"Guests waiting: {_guestFlowManager.TotalGuestsWaiting}\n");

        Console.WriteLine("Gates:");
        foreach (var gate in _gateManager.Gates)
        {
            var avgFocus = gate.AssignedStaff.Count > 0
                ? gate.AssignedStaff.Average(s => s.CurrentFocus)
                : 0;

            Console.WriteLine(
                $"  Gate {gate.Id}: {(gate.IsOpen ? "OPEN" : "CLOSED")} | Queue: {gate.QueueLength} | Processed: {gate.TotalProcessed} | Avg Focus: {avgFocus:F0}%");
        }

        Console.WriteLine();
    }

    private bool HasEventFinished()
    {
        var eventEnd = _timeManager.StartTime.Date.AddDays(1);
        return _timeManager.CurrentTime >= eventEnd;
    }

    private static TimeSpan Multiply(TimeSpan duration, int factor)
    {
        if (factor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be non-negative.");
        }

        return TimeSpan.FromTicks(checked(duration.Ticks * factor));
    }
}
