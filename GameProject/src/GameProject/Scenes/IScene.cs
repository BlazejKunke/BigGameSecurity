using GameProject.Game;

namespace GameProject.Scenes;

/// <summary>
/// Describes a unit of gameplay that can load, update, and render.
/// </summary>
public interface IScene
{
    string Name { get; }

    void LoadContent();

    void Update(TimeSpan deltaTime, GameState gameState);

    void Render();
}
