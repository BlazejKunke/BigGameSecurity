using GameProject.Scenes;

namespace GameProject.Game;

/// <summary>
/// Very small example of a sequential scene-based game loop.
/// </summary>
public class GameLoop
{
    private readonly IReadOnlyList<IScene> _scenes;

    public GameLoop(IEnumerable<IScene> scenes)
    {
        _scenes = scenes.ToList();
        if (_scenes.Count == 0)
        {
            throw new InvalidOperationException("At least one scene must be provided.");
        }
    }

    public void Run()
    {
        var gameState = new GameState();

        foreach (var scene in _scenes)
        {
            Console.WriteLine($"\n--- {scene.Name} ---");

            scene.LoadContent();
            scene.Update(TimeSpan.FromMilliseconds(16), gameState);
            scene.Render();
        }

        Console.WriteLine("\nThank you for playing the sample!");
    }
}
