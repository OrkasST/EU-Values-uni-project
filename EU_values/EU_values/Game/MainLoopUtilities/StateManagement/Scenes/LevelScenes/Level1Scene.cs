using EU_values.Game.BaseClasses;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level1Scene: InGameScene
{
    private Box _background;
    private Box _doorShadow;
    private Box _tree;

    private PlayerInventory _playerInventory;

    public Level1Scene(GameLevels previousLevel, Action<int, int, int, int> setCameraOffset) : base(previousLevel, setCameraOffset)
    {
        _background = new("Level_1_Background", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_Background.png"), true);
        _doorShadow = new("Level_1_DoorShadow", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_DoorShadow.png"), true);
        _tree = new("Level_1_tree", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Tree.png"), true);
        _playerInventory = new("Level_1_PlayerInventory", 400, 40, 8);

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);
        RenderList.AddObject(2, _doorShadow);
        RenderList.AddObject(2, _tree);
        RenderList.AddObject(3, _playerInventory);

        ReadData("..\\..\\..\\Resources\\Data\\Level_1.json");
    }

    public override void HandleUserInput()
    {
        base.HandleUserInput();
        if (InputHandler.NoKeyboardEvents) return;
        if (InputHandler.LastKeyboardEvent.IsHandled) return;

        if (InputHandler.LastKeyboardEvent.Key == Keys.E && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _playerInventory.MoveSelection(true);
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }
        else if (InputHandler.LastKeyboardEvent.Key == Keys.Q && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _playerInventory.MoveSelection(false);
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);
    }
}
