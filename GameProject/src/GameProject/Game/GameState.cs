using GameProject.Entities;

namespace GameProject.Game;

/// <summary>
/// Stores globally accessible state that scenes can share.
/// </summary>
public class GameState
{
    public Player Player { get; } = new("Avery");

    /// <summary>
    /// Arbitrary key/value store for sharing small bits of data between scenes.
    /// </summary>
    public Dictionary<string, object> SharedData { get; } = new();
}
