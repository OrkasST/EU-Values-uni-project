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
    protected TiledMap? _tiledMap { get; set; }
    protected List<TiledLayer> _dataLayers { get; set; } = new();

    protected Action<int, int, int, int> SetCameraOffset { get; set; }

    protected Player _player { get; set; }

    protected GameUIText _playerData { get; set; }
    protected Box _playerInfoBg { get; set; }
    private GameUIText _playerHitboxPosText;
    private Box _playerHitboxPosTextBG;
    private Box _playerHitboxVisualization;

    protected List<Hitbox> DoorWays { get; set; } = [];
    protected List<Hitbox> Doors { get; set; } = [];
    protected List<Hitbox> Ground { get; set; } = [];
    protected List<Hitbox> LevelBorders { get; set; } = [];
    protected Point PlayerSpawnPoint { get; set; }

    protected bool IsSceneReady { get; private set; } = false;
    protected bool IsDataRead { get; private set; } = false;
    protected bool IsPlayerOnTheGround { get; set; }

    public static Dictionary<string, GameLevels> DoorDictionary = new()
    {
        ["Level_1"] = GameLevels.Level_1,
        ["Level_2"] = GameLevels.Level_2
    };

    protected GameLevels PreviousLevel { get; private set; }

    public InGameScene(GameLevels previousLevel, Action<int, int, int, int> setCameraOffsset)
    {
        _player = new("Player", 1200, 1100);

        #region Add Service Data
        _playerData = new("PlayerData", 450, 50, "PlayerData", "Times New Roman", 26, Color.White);
        _playerData.ToggleVisibility();

        _playerInfoBg = new("PlayerInfoBg", 420, 20, 400, 380, Color.FromArgb(80, Color.Blue));
        _playerInfoBg.ToggleVisibility();

        _playerHitboxPosText = new("playerHitboxPosition", _player.Hitbox.X - 5, _player.Hitbox.Y - 35, "X: 0, Y: 0", "Times New Roman", 18, true);
        _playerHitboxPosTextBG = new("PlayerInfoBg", _player.Hitbox.X - 10, _player.Hitbox.Y - 40, 170, 45, Color.FromArgb(170, Color.Black));
        _playerHitboxVisualization = new("playerHitbox", _player.Hitbox.X, _player.Hitbox.Y, _player.Hitbox.Width, _player.Hitbox.Height, Color.AliceBlue, false, true);

        RenderList.AddObject(-1, _playerInfoBg);
        RenderList.AddObject(-1, _playerData);
        RenderList.AddObject(-1, _playerHitboxVisualization);
        RenderList.AddObject(-1, _playerHitboxPosTextBG);
        RenderList.AddObject(-1, _playerHitboxPosText);
        #endregion

        SetCameraOffset = setCameraOffsset;

        PreviousLevel = previousLevel;
    }

    protected void ReadData(string filePath)
    {
        if (String.IsNullOrWhiteSpace(filePath)) return;
        if (!File.Exists(filePath)) return;

        var source = File.ReadAllText(filePath);
        _tiledMap = JsonSerializer.Deserialize<TiledMap>(source);

        if (_tiledMap == null) return;

        for (int i = 0; i < _tiledMap.Layers.Count; i++)
            if (_tiledMap.Layers[i].Type == "objectgroup")
                _dataLayers.Add(_tiledMap.Layers[i]);

        IsDataRead = true;

        GetSceneHitboxes();
        LocatePlayerInLevel();
    }

    protected void GetSceneHitboxes()
    {
        if (!IsDataRead) return;
        foreach (var layer in _dataLayers)
            for (int j = 0; j < layer.Objects?.Count; j++)
                switch (layer.Name)
                {
                    case "Door": Doors.Add(Collider.CreateHitbox(layer.Objects[j])); break;
                    case "DoorWay": DoorWays.Add(Collider.CreateHitbox(layer.Objects[j])); break;
                    case "Ground": Ground.Add(Collider.CreateHitbox(layer.Objects[j])); AddHitboxGraphicalRepresentation(layer.Objects[j]); break;
                    case "LevelBorders": LevelBorders.Add(Collider.CreateHitbox(layer.Objects[j])); AddHitboxGraphicalRepresentation(layer.Objects[j]); break;
                    case "PlayerSpawnPoint": PlayerSpawnPoint = new Point((int)layer.Objects[j].X, (int)layer.Objects[j].Y); break;
                    default: break;
                }
    }

    private void AddHitboxGraphicalRepresentation(TiledObject obj)
    {
        RenderList.AddObject(0, new Box(obj.Name, obj.X, obj.Y, obj.Width, obj.Height, Color.Black, isFilled: false, isCameraAffected: true));
        RenderList.AddObject(0, new GameUIText("HitboxPosition", obj.X - 5, obj.Y - 35, $"X: {obj.X}, Y: {obj.Y}", "Times New Roman", 18, true));

    }
    protected void LocatePlayerInLevel()
    {
        int playerX = PlayerSpawnPoint.X;
        int playerY = PlayerSpawnPoint.Y;

        if (PreviousLevel != GameLevels.None)
            foreach (var door in Doors) if (DoorDictionary[door.Id] == PreviousLevel)
                {
                    _player.SetPositionByCenter((door.X + door.Width / 2) - _player.Body.Size.Width / 2, playerY);
                    return;
                }

        _player.SetPositionByCenter(playerX, playerY);
    }


    public override void HandleUserInput()
    {
        base.HandleUserInput();
        _player.HandleUserInput(InputHandler.LastKeyboardEvent);

        if (InputHandler.NoKeyboardEvents) return;

        if (InputHandler.LastKeyboardEvent.Key == Keys.Oem3
            && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp && !InputHandler.LastKeyboardEvent.IsHandled)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _playerData.ToggleVisibility();
            _playerInfoBg.ToggleVisibility();
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);
        _player.Update(timeDelta, timeRemaining, timeDifference);

        if (_player.CurrentState != PlayerState.Falling && _player.CurrentState != PlayerState.Jumping)
        {
            if (Collider.DetectFullEnterCollision(_player.Hitbox, DoorWays)) _player.OnDoorWayEnter();
            else if (_player.IsInFrontOfDoor) _player.OnDoorWayLeave();
            if (!_player.IsEnteringTheDoor && Collider.DetectPartialEnterCollision(_player.Hitbox, Doors)) _player.OnDoorEnterStart();
            if (_player.IsEnteringTheDoor && Collider.DetectFullEnterCollision(_player.Hitbox, Doors))
            {
                _nextState = States.InGameActive;
                _nextLevel = DoorDictionary[Collider.LastFullEnterCollisionId];
            }
        }
        if (_player.CurrentState == PlayerState.Falling && Collider.DetectAccuratelyPartialEnterCollision(_player.Hitbox, Ground))
        {
            IsPlayerOnTheGround = true;
            _player.SetPosition(Collider.lastPartialEnterTouchPosition);
            _player.OnGroundHit();
        }
        else if (!_player.IsInFrontOfDoor && _player.CurrentState != PlayerState.Falling && !Collider.DetectPartialEnterCollision(_player.Hitbox, Ground))
        {
            IsPlayerOnTheGround = false;
            _player.OnGroundLeave();
        }

        if (Collider.DetectAccuratelyPartialEnterCollision(_player.Hitbox, LevelBorders))
        {
            _player.SetPositionByCenter(
                Collider.lastAccuratePartialEnteredHitbox.X < _player.Hitbox.X ?
                Collider.lastAccuratePartialEnteredHitbox.X + Collider.lastAccuratePartialEnteredHitbox.Width + _player.Hitbox.Width / 2
                : Collider.lastAccuratePartialEnteredHitbox.X - _player.Hitbox.Width/2,
                _player.Body.Position.Y + (_player.Body.Size.Height / 2));
            _player.OnLevelBorderHit();
        }

        if (_nextLevel != GameLevels.None) AwaitedLevel = _nextLevel;

        ServiceDataUpdate(timeDelta);
    }

    private void ServiceDataUpdate(float timeDelta)
    {
        _playerData.Text = $"Current Animation: {_player.CurrentAnimation}\n - IsStarted: {_player.Body.IsAnimationStarted}" +
            $"\n - IsInfinite: {_player.Body.IsAnimationInfinite}" +
            $"\n - Current Frame: {_player.Body.CurrentFrame}" +
            $"\n - Current X Frame: {_player.Body.CurrentXFrame}" +
            $"\n - Current Y Frame: {_player.Body.CurrentYFrame}" +
            $"\n - Player Position: {_player.Body.Position.X}, {_player.Body.Position.Y}" +
            $"\n - Player IsInFrontOfDoor: {_player.IsInFrontOfDoor}" +
            $"\n - Player is entering the door: {_player.IsEnteringTheDoor}" +
            $"\n - Next Level: {_nextLevel}" +
            $"\n - TimeDelta: {timeDelta}" +
            $"\n - Player Is on Ground: {IsPlayerOnTheGround}";
        _playerHitboxVisualization.Position = new PointF(_player.Hitbox.X, _player.Hitbox.Y);
        _playerHitboxPosText.Position = new PointF(_player.Hitbox.X - 5, _player.Hitbox.Y - 40);
        _playerHitboxPosTextBG.Position = new PointF(_player.Hitbox.X - 10, _player.Hitbox.Y - 40);
        _playerHitboxPosText.Text = $"X: {_player.Hitbox.X}\nY: {_player.Hitbox.Y}";
    }
}
