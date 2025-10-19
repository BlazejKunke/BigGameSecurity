using GameProject.Entities;
using GameProject.Game;

namespace GameProject.Scenes;

public class GameplayScene : IScene
{
    public string Name => "Gameplay";

    public void LoadContent()
    {
        Console.WriteLine("Spawning enemies and preparing the arena...");
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        Console.WriteLine($"Player {gameState.Player.Name} is exploring the world.");
        gameState.Player.Move(new Position2D(1, 0));
        Console.WriteLine($"Player moved to {gameState.Player.Position}.");
    }

    public void Render()
    {
        Console.WriteLine("[Gameplay world rendered]");
    }
}
