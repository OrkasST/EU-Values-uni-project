using EU_values.Game.UI.Elements;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

internal class Level2Scene: InGameScene
{
    private Box _background;

    public Level2Scene(GameLevels previousLevel, Action <int, int, int, int> setCameraOffset) : base(previousLevel, setCameraOffset)
    {
        _background = new Box("Level_2_Background", 0, 0, 2318, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_2\\Level_2_Background.png"), true);

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);

        ReadData("..\\..\\..\\Resources\\Data\\Level_2.json");
        //GetSceneHitboxes();
    }
}
