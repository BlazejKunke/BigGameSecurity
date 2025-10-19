using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Stores globally accessible state that scenes or systems can share.
/// </summary>
public class GameState
{
    public GameState()
    {
        TimeManager = new TimeManager(DateTime.Today.AddHours(6));
        ReputationManager = new ReputationManager();
        GateManager = new GateManager(6);
        GuestFlowManager = new GuestFlowManager(GateManager);
        IncidentManager = new IncidentManager(GateManager, ReputationManager);
    }

    public Player Player { get; } = new("Avery");

    public TimeManager TimeManager { get; }

    public ReputationManager ReputationManager { get; }

    public GateManager GateManager { get; }

    public GuestFlowManager GuestFlowManager { get; }

    public IncidentManager IncidentManager { get; }

    /// <summary>
    /// Active gates registered in the park.
    /// </summary>
    public List<Gate> Gates { get; } = new();

    /// <summary>
    /// Staff that can be assigned to gates.
    /// </summary>
    public List<Staff> StaffMembers { get; } = new();

    /// <summary>
    /// Guests currently tracked by the game.
    /// </summary>
    public List<Guest> Guests { get; } = new();

    /// <summary>
    /// Arbitrary key/value store for sharing small bits of data between scenes.
    /// </summary>
    public Dictionary<string, object> SharedData { get; } = new();
}
