using System.Globalization;
using System.Linq;
using GameProject.Entities;
using GameProject.Game;

namespace GameProject.Scenes;

/// <summary>
/// Represents the main event loop (18:00-24:00) where security operations unfold in real time.
/// </summary>
public sealed class EventScene : IScene
{
    private readonly TimeManager _timeManager;
    private readonly GateManager _gateManager;
    private readonly GuestFlowManager _guestFlowManager;
    private readonly IncidentManager _incidentManager;
    private readonly ReputationManager _reputationManager;
    private GameState? _lastGameState;
    private bool _hasAnnouncedGameOver;
    private bool _hasAnnouncedCompletion;

    public EventScene(
        TimeManager timeManager,
        GateManager gateManager,
        GuestFlowManager guestFlowManager,
        IncidentManager incidentManager,
        ReputationManager reputationManager)
    {
        _timeManager = timeManager ?? throw new ArgumentNullException(nameof(timeManager));
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
        _guestFlowManager = guestFlowManager ?? throw new ArgumentNullException(nameof(guestFlowManager));
        _incidentManager = incidentManager ?? throw new ArgumentNullException(nameof(incidentManager));
        _reputationManager = reputationManager ?? throw new ArgumentNullException(nameof(reputationManager));
    }

    public string Name => "Event Execution";

    /// <summary>
    /// Sets up the event simulation by ensuring the gates are open and guests are generated.
    /// </summary>
    public void LoadContent()
    {
        Console.WriteLine("Preparing for the main event...");
        _gateManager.OpenAllGates();
        _guestFlowManager.GenerateGuests(1200);
        _hasAnnouncedGameOver = false;
        _hasAnnouncedCompletion = false;
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        if (deltaTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");
        }

        _lastGameState = gameState ?? throw new ArgumentNullException(nameof(gameState));

        if (!ReferenceEquals(gameState.TimeManager, _timeManager))
        {
            throw new InvalidOperationException("The supplied GameState must share the same TimeManager instance as the scene.");
        }

        var scaledDelta = TimeSpan.FromSeconds(deltaTime.TotalSeconds * 6);

        if (scaledDelta > TimeSpan.Zero)
        {
            _timeManager.Advance(scaledDelta);
            _guestFlowManager.Update(_timeManager.CurrentTime.TimeOfDay, scaledDelta);
            _incidentManager.Update(scaledDelta);

            var hoursElapsed = (float)scaledDelta.TotalHours;

            foreach (var gate in _gateManager.Gates)
            {
                foreach (var staff in gate.AssignedStaff)
                {
                    staff.UpdateFocus(hoursElapsed);
                }
            }
        }

        if (_reputationManager.Score <= 0 && !_hasAnnouncedGameOver)
        {
            Console.WriteLine("\n\uD83D\uDC80 GAME OVER: You've been fired!");
            _hasAnnouncedGameOver = true;
            return;
        }

        var currentTimeOfDay = _timeManager.CurrentTime.TimeOfDay;
        if (!_hasAnnouncedCompletion &&
            (currentTimeOfDay >= TimeSpan.FromHours(24) || _timeManager.CurrentTime.Date > _timeManager.StartTime.Date))
        {
            Console.WriteLine("\n\u2713 Event complete!");
            _hasAnnouncedCompletion = true;
        }
    }

    public void Render()
    {
        Console.Clear();

        var currentTime = _timeManager.CurrentTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        Console.WriteLine($"Time: {currentTime}");
        Console.WriteLine($"Reputation: {_reputationManager.Score}");
        Console.WriteLine($"Total Queue: {_gateManager.TotalQueueLength}");

        Console.WriteLine("\nGates:");
        foreach (var gate in _gateManager.Gates)
        {
            var focus = gate.AssignedStaff.Count == 0
                ? 0
                : (int)Math.Round(gate.AssignedStaff.Average(staff => staff.CurrentFocus));
            var status = gate.IsOpen ? "OPEN" : "CLOSED";

            Console.WriteLine($"  Gate {gate.Id}: {status} | Queue: {gate.QueueLength} | Avg Focus: {focus}%");
        }

        if (_incidentManager.RecentIncidents.Count > 0)
        {
            Console.WriteLine("\nRecent Incidents:");
            foreach (var incident in _incidentManager.RecentIncidents.TakeLast(5))
            {
                Console.WriteLine($"  [{incident.Timestamp:HH:mm}] Gate {incident.GateId}: {incident.Description} (Severity {incident.Severity})");
            }
        }

        if (_lastGameState?.Player is { } player)
        {
            Console.WriteLine($"\nPlayer: {player.Name} | Position: {player.Position}");
        }
    }
}
