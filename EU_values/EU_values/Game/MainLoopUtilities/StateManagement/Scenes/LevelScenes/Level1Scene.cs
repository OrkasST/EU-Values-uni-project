using EU_values.Game.UI.Elements;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level1Scene: InGameScene
{
    private Box _background;
    private Box _doorShadow;

    public Level1Scene(GameLevels previousLevel, Action<int, int, int, int> setCameraOffset) : base(previousLevel, setCameraOffset)
    {
        _background = new("Level_1_Background", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_Background.png"), true);
        _doorShadow = new("Level_1_DoorShadow", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_DoorShadow.png"), true);

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);
        RenderList.AddObject(2, _doorShadow);

        ReadData("..\\..\\..\\Resources\\Data\\Level_1.json");
        GetSceneHitboxes();

        MovePlayerToTheDoor(previousLevel);
    }

    public override void HandleUserInput()
    {
        base.HandleUserInput();
    }

    public override void Update(int timeDelta, int timeRemaining)
    {
        base.Update(timeDelta, timeRemaining);
    }
}
