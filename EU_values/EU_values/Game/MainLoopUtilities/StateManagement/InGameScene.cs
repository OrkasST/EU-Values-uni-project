using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;
using System.Text.Json;

namespace EU_values.Game.MainLoopUtilities.StateManagement;

public abstract class InGameScene : Scene
{
    protected TiledMap? _data;
    protected Action<int, int, int, int> SetCameraOffset;

    protected Player _player { get; set; }
    protected GameUIText _playerData;
    protected Box _playerInfoBg;

    protected List<Hitbox> DoorWays = [];
    protected List<Hitbox> Doors = [];

    protected bool IsSceneReady { get; private set; } = false;
    protected bool IsDataRead { get; private set; } = false;

    public static Dictionary<string, GameLevels> DoorDictionary = new ()
    {
        ["Level_1"] = GameLevels.Level_1,
        ["Level_2"] = GameLevels.Level_2
    };

    public InGameScene(GameLevels previousLevel, Action<int, int, int, int> setCameraOffsset)
    {
        _player = new("Player", 1200, 1100);

        _playerData = new("PlayerData", 450, 50, "PlayerData", "Times New Roman", 26, Color.White);
        _playerData.ToggleVisibility();

        _playerInfoBg = new("PlayerInfoBg", 420, 20, 400, 350, Color.FromArgb(80, Color.Blue));
        _playerInfoBg.ToggleVisibility();

        SetCameraOffset = setCameraOffsset;

        RenderList.AddObject(-1, _playerInfoBg);
        RenderList.AddObject(-1, _playerData);
    }

    protected void MovePlayerToTheDoor(GameLevels previousLevel)
    {
        if (previousLevel != GameLevels.None)
            foreach (var door in Doors) if (DoorDictionary[door.Id] == previousLevel)
                {
                    _player.SetPosition((door.X + door.Width / 2) - _player.Body.Size.Width / 2, _player.Body.Position.X);
                    return;
                }

    }

    protected void ReadData(string filePath)
    {
        if (String.IsNullOrWhiteSpace(filePath)) return;
        if (!File.Exists(filePath)) return;

        var source = File.ReadAllText(filePath);
        _data = JsonSerializer.Deserialize<TiledMap>(source);

        IsDataRead = true;
    }

    protected void GetSceneHitboxes()
    {
        if (!IsDataRead) return;

        for (int i = 0; i < _data?.Layers?.Count; i++)
            if (_data.Layers?[i].Type == "objectgroup")
                for (int j = 0; j < _data.Layers[i].Objects?.Count; j++)
                    if (_data.Layers[i].Name == "Door") Doors.Add(CreateHitboxRectangle(_data.Layers[i].Objects?[j]));
                    else if (_data.Layers[i].Name == "DoorWay") DoorWays.Add(CreateHitboxRectangle(_data.Layers[i].Objects?[j]));
    }

    protected Hitbox CreateHitboxRectangle(TiledObject? obj)
    {
        if (obj == null) return Hitbox.Empty;
        return new Hitbox((int)obj.X, (int)obj.Y, (int)obj.Width, (int)obj.Height, obj.Name);
    }

    public override void HandleUserInput()
    {
        base.HandleUserInput();
        _player.HandleUserInput();
        if ( !InputHandler.NoKeyboardEvents && InputHandler.LastKeyboardEvent.Key == Keys.Oem3
            && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp && !InputHandler.LastKeyboardEvent.IsHandled )
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _playerData.ToggleVisibility();
            _playerInfoBg.ToggleVisibility();
        }
    }

    public override void Update(int timeDelta, int timeRemaining)
    {
        base.Update(timeDelta, timeRemaining);
        _player.Update(timeDelta, timeRemaining);

        var playerHitbox = new Rectangle(_player.Body.Position, _player.Body.Size);

        if (Collider.DetectFullEnterCollision(playerHitbox, DoorWays)) _player.OnDoorWayEnter();
        else if (_player.IsInFrontOfDoor) _player.OnDoorWayLeave();
        if (!_player.IsEnteringTheDoor && Collider.DetectPartialEnterCollision(playerHitbox, Doors)) _player.OnDoorEnterStart();
        if (_player.IsEnteringTheDoor && Collider.DetectFullEnterCollision(playerHitbox, Doors))
        {
            _nextState = States.InGameActive;
            _nextLevel = DoorDictionary[Collider.LastFullEnterCollisionId];
        }

        if (_nextLevel != GameLevels.None) AwaitedLevel = _nextLevel;


        _playerData.Text = $"Current Animation: {_player.CurrentAnimation}\n - IsStarted: {_player.Body.IsAnimationStarted}" +
            $"\n - IsInfinite: {_player.Body.IsAnimationInfinite}" +
            $"\n - Current Frame: {_player.Body.CurrentFrame}" +
            $"\n - Current X Frame: {_player.Body.CurrentXFrame}" +
            $"\n - Current Y Frame: {_player.Body.CurrentYFrame}" +
            $"\n - Player Position: {_player.Body.Position.X}, {_player.Body.Position.Y}" +
            $"\n - Player IsInFrontOfDoor: {_player.IsInFrontOfDoor}" +
            $"\n - Player is entering the door: {_player.IsEnteringTheDoor}" +
            $"\n - Next Level: {_nextLevel}";
    }
}
