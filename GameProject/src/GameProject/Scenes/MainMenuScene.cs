using GameProject.Game;

namespace GameProject.Scenes;

public class MainMenuScene : IScene
{
    public string Name => "Main Menu";

    public void LoadContent()
    {
        Console.WriteLine("Loading menu assets...");
    }

    public void Update(TimeSpan deltaTime, GameState gameState)
    {
        Console.WriteLine("Welcome to the sample game!");
        Console.WriteLine("Pretending to wait for player input to start the game...");
        gameState.SharedData["HasStarted"] = true;
    }

    public void Render()
    {
        Console.WriteLine("[Main Menu UI Rendered]");
    }
}
