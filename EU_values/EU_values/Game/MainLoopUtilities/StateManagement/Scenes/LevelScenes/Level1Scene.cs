using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using System.Text.Json;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level1Scene: InGameScene
{
    private Player _player = new ("Level_1_player", 1200, 1100);
    private Box _playerInfoBg = new ("Level_1_PlayerInfoBg", 420, 20, 400, 350, Color.FromArgb(80, Color.Blue));
    private GameUIText _playerData = new ("Level_1_PlayerData", 450, 50, "PlayerData", "Times New Roman", 26, Color.White);
    private Box _background = new("Level_1_Background", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_Background.png"), true);
    private Box _doorShadow = new("Level_1_DoorShadow", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_DoorShadow.png"), true);

    private List<Rectangle> DoorWays = [];
    private List<Rectangle> Doors = [];

    public Level1Scene(Action<int, int, int, int> setCameraOffset) : base(setCameraOffset)
    {
        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);
        RenderList.AddObject(1, _playerInfoBg);
        RenderList.AddObject(2, _doorShadow);
        RenderList.AddObject(2, _playerData);

        ReadData("..\\..\\..\\Resources\\Data\\Level_1.json");

        for (int i = 0; i < _data?.Layers?.Count; i++)
            if (_data.Layers?[i].Type == "objectgroup")
                for (int j = 0; j < _data.Layers[i].Objects?.Count; j++)
                    if (_data.Layers[i].Name == "Door") Doors.Add( CreateHitboxRectangle(_data.Layers[i].Objects?[j]) );
                    else if (_data.Layers[i].Name == "DoorWay") DoorWays.Add( CreateHitboxRectangle(_data.Layers[i].Objects?[j]) );
    }

    private Rectangle CreateHitboxRectangle(TiledObject? obj)
    {
        if (obj == null) return new Rectangle();
        return new Rectangle((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height);
    }

    public override void HandleUserInput()
    {
        _player.HandleUserInput();
        _playerData.Text = $"Current Animation: {_player.CurrentAnimation}\n - IsStarted: {_player.Body.IsAnimationStarted}" +
            $"\n - IsInfinite: {_player.Body.IsAnimationInfinite}" +
            $"\n - Current Frame: {_player.Body.CurrentFrame}" +
            $"\n - Current X Frame: {_player.Body.CurrentXFrame}" +
            $"\n - Current Y Frame: {_player.Body.CurrentYFrame}" +
            $"\n - Player Position: {_player.Body.Position.X}, {_player.Body.Position.Y}";
    }

    public override void Update(int timeDelta, int timeRemaining)
    {
        _player.Update(timeDelta, timeRemaining);


        var playerHitbox = new Rectangle(_player.Body.Position, _player.Body.Size);

        if ( Collider.DetectFullEnterCollision(playerHitbox, DoorWays) ) _player.OnDoorWayEnter();
        else if ( _player.IsInFrontOfDoor ) _player.OnDoorWayLeave();
        if ( !_player.IsEnteringTheDoor && Collider.DetectPartialEnterCollision(playerHitbox, Doors) ) _player.OnDoorEnterStart();
        if ( _player.IsEnteringTheDoor && Collider.DetectFullEnterCollision(playerHitbox, Doors)) { }

        _playerData.Text = _playerData.Text + $"\nPlayer IsInFrontOfDoor: {_player.IsInFrontOfDoor}\nPlayer is entering the door: {_player.IsEnteringTheDoor}";

        base.Update(timeDelta, timeRemaining);
    }
}
