using GameProject.Game;
using GameProject.Scenes;

var scenes = new List<IScene>
{
    new MainMenuScene(),
    new GameplayScene()
};

var game = new GameLoop(scenes);
game.Run();
