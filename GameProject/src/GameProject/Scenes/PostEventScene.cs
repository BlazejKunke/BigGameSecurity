using System;
using System.Linq;
using GameProject.Entities;
using GameProject.Game;

namespace GameProject.Scenes;

/// <summary>
/// Displays the post-event performance report and allows the player to manage their staff roster
/// before proceeding to the next event.
/// </summary>
public sealed class PostEventScene : IScene
{
    private readonly ReputationManager _reputationManager;
    private readonly GateManager _gateManager;
    private readonly GuestFlowManager _guestFlowManager;
    private readonly HiringManager _hiringManager;

    public PostEventScene(
        ReputationManager reputationManager,
        GateManager gateManager,
        GuestFlowManager guestFlowManager,
        HiringManager hiringManager)
    {
        _reputationManager = reputationManager ?? throw new ArgumentNullException(nameof(reputationManager));
        _gateManager = gateManager ?? throw new ArgumentNullException(nameof(gateManager));
        _guestFlowManager = guestFlowManager ?? throw new ArgumentNullException(nameof(guestFlowManager));
        _hiringManager = hiringManager ?? throw new ArgumentNullException(nameof(hiringManager));
    }

    public string Name => "Post-Event Review";

    public void LoadContent()
    {
        // No resources to load
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        if (deltaTime < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaTime), "Delta time cannot be negative.");
        }

        if (gameState is null)
        {
            throw new ArgumentNullException(nameof(gameState));
        }

        // Display is handled in Render()
        // This scene is passive - just waiting for user input to continue
    }

    public void Render()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════╗");
        Console.WriteLine("║      EVENT PERFORMANCE REPORT              ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");
        Console.WriteLine();

        // Overall metrics
        Console.WriteLine("=== OVERALL PERFORMANCE ===");
        Console.WriteLine($"Final Reputation: {_reputationManager.Score}/100");
        Console.WriteLine($"Guests Processed: {_guestFlowManager.TotalGuestsProcessed}");
        Console.WriteLine($"Guests Denied/Arrested: {CountDeniedOrJailed()}");
        Console.WriteLine();

        // Gate-by-gate breakdown
        Console.WriteLine("=== GATE PERFORMANCE ===");
        foreach (var gate in _gateManager.Gates)
        {
            var totalIncidents = gate.IncidentsDetected + gate.IncidentsMissed;
            var detectionRate = totalIncidents > 0
                ? gate.IncidentsDetected * 100.0 / totalIncidents
                : 0.0;

            var assignedStaff = gate.AssignedStaff.Count > 0
                ? string.Join(", ", gate.AssignedStaff.Select(s => s.FirstName))
                : "None";

            Console.WriteLine($"Gate {gate.Id}:");
            Console.WriteLine($"  Processed: {gate.TotalProcessed} guests");
            Console.WriteLine($"  Threats Detected: {gate.IncidentsDetected}");
            Console.WriteLine($"  Threats Missed: {gate.IncidentsMissed}");
            Console.WriteLine($"  Detection Rate: {detectionRate:F1}%");
            Console.WriteLine($"  Assigned Staff: {assignedStaff}");
            Console.WriteLine();
        }

        // Reputation history (last 5 events)
        Console.WriteLine("=== RECENT EVENTS ===");
        var recentEvents = _reputationManager.Events.TakeLast(5).ToList();
        if (recentEvents.Count == 0)
        {
            Console.WriteLine("No significant events recorded.");
        }
        else
        {
            foreach (var evt in recentEvents)
            {
                var sign = evt.Delta >= 0 ? "+" : string.Empty;
                Console.WriteLine($"{sign}{evt.Delta} - {evt.Description}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("=== STAFF ROSTER ===");
        foreach (var staff in _hiringManager.HiredStaff)
        {
            var gateName = staff.AssignedGate is not null ? $"Gate {staff.AssignedGate.Id}" : "Unassigned";
            Console.WriteLine($"{staff.FirstName} {staff.LastName} - {gateName} | Focus ended at {staff.CurrentFocus}%");
        }

        Console.WriteLine();
        Console.WriteLine("────────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine("[C] Continue to next event");
        Console.WriteLine("[M] Manage staff (fire/review)");
        Console.WriteLine("[Q] Quit game");
        Console.WriteLine();

        var key = Console.ReadKey(true);

        switch (key.Key)
        {
            case ConsoleKey.C:
                Console.WriteLine("Preparing for next event...");
                break;
            case ConsoleKey.M:
                ManageStaff();
                break;
            case ConsoleKey.Q:
                Console.WriteLine("\nThank you for playing BigGameSecurity!");
                Environment.Exit(0);
                break;
        }
    }

    private void ManageStaff()
    {
        Console.Clear();
        Console.WriteLine("=== STAFF MANAGEMENT ===");
        Console.WriteLine();

        var staffList = _hiringManager.HiredStaff.ToList();

        if (staffList.Count == 0)
        {
            Console.WriteLine("No staff currently employed.");
            Console.WriteLine("\nPress any key to return...");
            Console.ReadKey();
            Render();
            return;
        }

        for (var i = 0; i < staffList.Count; i++)
        {
            var staff = staffList[i];
            Console.WriteLine($"{i + 1}. {staff.FirstName} {staff.LastName}");
            Console.WriteLine($"   STR: {staff.PhysicalStrength} | COM: {staff.Communication} | OBS: {staff.Observation}");
            Console.WriteLine($"   Reliability: {staff.Reliability}% | Focus: {staff.FocusSustainability}% | Quit Risk: {staff.QuitRisk}%");
            Console.WriteLine();
        }

        Console.WriteLine("Enter staff number to fire (or 0 to cancel): ");
        var input = Console.ReadLine();

        if (int.TryParse(input, out var staffNumber) && staffNumber > 0 && staffNumber <= staffList.Count)
        {
            var staff = staffList[staffNumber - 1];
            Console.WriteLine($"\nAre you sure you want to fire {staff.FirstName} {staff.LastName}? [Y/N]");
            var confirm = Console.ReadKey(true);

            if (confirm.Key == ConsoleKey.Y)
            {
                _hiringManager.FireStaff(staff);
                Console.WriteLine($"\n✗ {staff.FirstName} {staff.LastName} has been terminated.");
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        Render();
    }

    private int CountDeniedOrJailed()
    {
        return _guestFlowManager.AllGuests.Count(g =>
            g.CurrentState == GuestState.Denied || g.CurrentState == GuestState.AtJail);
    }
}
