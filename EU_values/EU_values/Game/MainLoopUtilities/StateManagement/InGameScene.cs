using EU_values.Game.BaseClasses;
using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.Interfaces;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;
using System.Text.Json;

namespace EU_values.Game.MainLoopUtilities.StateManagement;

public abstract class InGameScene : Scene
{
    protected Player _player { get; set; }
    protected Box _background;

    protected string LastLevelHintPhrase = "";
    protected HintProposalWindow HintForFinalLevelWindow { get; private set; }

    #region Level UI Declaration

    protected GameUIText _playerData { get; set; }
    protected GameUIText _playerData1 { get; set; }
    protected Box _playerInfoBg { get; set; }
    private GameUIText _playerHitboxPosText;
    private Box _playerHitboxPosTextBG;
    private Box _playerHitboxVisualization;

    protected GameWindow _finalPhrase;
    protected string FinalPhraseText = "Look at the person, not the mask.";
    private int _finalPhraseAppearTime = -1;

    private GameWindow _failedQuestionNotification;
    #endregion

    private float _failWindowAppearTime = -1;

    #region Level Imported Data Declaration

    protected TiledMap? _tiledMap { get; set; }
    protected List<TiledLayer> _dataLayers { get; set; } = new();
    protected List<Hitbox> DoorWays { get; set; } = [];
    protected List<Door> Doors { get; set; } = [];
    protected List<Hitbox> Ground { get; set; } = [];
    protected List<Hitbox> LevelBorders { get; set; } = [];
    protected Point PlayerSpawnPoint { get; set; }

    protected List<DrawableObject> LevelHitboxes { get; set; } = [];
    #endregion

    #region Level Flags Declaration

    protected bool _isInSettings = false;
    protected bool IsSceneReady { get; private set; } = false;
    protected bool IsDataRead { get; private set; } = false;
    protected bool IsPlayerOnTheGround { get; set; }

    protected bool _showServiceData = false;

    protected bool _isRightAnswerGiven = false;

    protected bool _isInDeveloperMode = false;
    private bool _isRightsGrantProcedureStarted = false;
    private bool _isLevelChooseProcedureStarted = false;
    #endregion

    public static Dictionary<string, GameLevels> DoorDictionary = new()
    {
        ["Level_1"] = GameLevels.Level_1,
        ["Level_2"] = GameLevels.Level_2,
        ["Level_3"] = GameLevels.Level_3,
        ["Level_4"] = GameLevels.Level_4,
        ["Level_5"] = GameLevels.Level_5,
        ["Level_6"] = GameLevels.Level_6,
        ["Level_7"] = GameLevels.Level_7
    };

    protected Dictionary<string, Action> _actionList;

    protected GameLevels PreviousLevel { get; private set; }

    protected InGameMenu gameMenu { get; private set; }
    protected bool _isPlayerCanMove = true;


    public InGameScene(GameLevels previousLevel)
    {
        _player = new("Player", 1200, 1100);
        _background = new("",0,0,0,0,Color.White,isCameraAffected:true);

        #region SettingUp Hint Proposal Window

        HintForFinalLevelWindow = new();

        _actionList = new()
        {
            [HintForFinalLevelWindow.YesHintButton.Name] = () =>
            {
                HintForFinalLevelWindow.YesHintButton.ToggleVisibility(false);
                HintForFinalLevelWindow.NoHintButton.Text = "Close hint";
                HintForFinalLevelWindow.Text.Text = LastLevelHintPhrase;
            },
            [HintForFinalLevelWindow.NoHintButton.Name] = () => {
                HintForFinalLevelWindow.ToggleVisibility(false);
            }
        };

        #endregion

        #region SettingUp Final Phrase
        _finalPhrase = new(name: "Level_1_FinalPhrase", x: UIPositioner.CenterX - 500, y: UIPositioner.CenterY - 200, width: 1000, height: 250,
            text: FinalPhraseText,
            textX: 500, textY: 100, textSize: 40, isCameraAffected: false);
        _finalPhrase.Text.ChangeTextAlignment(TextPositioning.Center);
        _finalPhrase.Text.ChangeColor(Color.Gold);
        _finalPhrase.Background.ChangeBackground(Color.FromArgb(230, 41, 21, 46));
        _finalPhrase.ToggleVisibility(false);
        #endregion

        _failedQuestionNotification = new(name: "Level_1_inventoryItemDescription", x: UIPositioner.CenterX - 400, y: UIPositioner.CenterY - 100, width: 800, height: 180,
            text: "You should think about the question a bit more...",
            textX: 400, textY: 30, textSize: 30, isCameraAffected: false);
        _failedQuestionNotification.ToggleVisibility();
        _failedQuestionNotification.Text.ChangeTextAlignment(TextPositioning.Center);

        RenderList.AddObject(-1, _failedQuestionNotification);
        RenderList.AddObject(-1, HintForFinalLevelWindow);
        RenderList.AddObject(-1, _finalPhrase);

        GameCamera = new();

        #region Add Service Data

        _playerData = new("PlayerData", 450, 50, "PlayerData", "Times New Roman", 26, Color.White);
        _playerData.ToggleVisibility();
        _playerData1 = new("PlayerData", 850, 50, "PlayerData", "Times New Roman", 26, Color.White);
        _playerData1.ToggleVisibility();

        _playerInfoBg = new("PlayerInfoBg", 420, 0, 800, UIPositioner.FormHeight, Color.FromArgb(80, Color.Blue));
        _playerInfoBg.ToggleVisibility();

        _playerHitboxPosText = new("playerHitboxPosition", _player.Hitbox.X - 5, _player.Hitbox.Y - 35, "X: 0, Y: 0", "Times New Roman", 18, true);
        _playerHitboxPosText.ToggleVisibility();
        _playerHitboxPosTextBG = new("PlayerInfoBg", _player.Hitbox.X - 10, _player.Hitbox.Y - 40, 170, 45, Color.FromArgb(170, Color.Black));
        _playerHitboxPosTextBG.ToggleVisibility();
        _playerHitboxVisualization = new("playerHitbox", _player.Hitbox.X, _player.Hitbox.Y, _player.Hitbox.Width, _player.Hitbox.Height, Color.AliceBlue, false, true);
        _playerHitboxVisualization.ToggleVisibility();

        RenderList.AddObject(-1, _playerInfoBg);
        RenderList.AddObject(-1, _playerData);
        RenderList.AddObject(-1, _playerData1);
        RenderList.AddObject(-1, _playerHitboxVisualization);
        RenderList.AddObject(-1, _playerHitboxPosTextBG);
        RenderList.AddObject(-1, _playerHitboxPosText);
        #endregion

        #region Add Ingame Menu

        gameMenu = new();
        RenderList.AddObject(-1, gameMenu.MenuBackground);
        RenderList.AddObject(-1, gameMenu.Title);
        RenderList.AddObject(-1, gameMenu.SaveGameButton);
        RenderList.AddObject(-1, gameMenu.GoToSettingsButton);
        RenderList.AddObject(-1, gameMenu.QuiteGameButton);

        foreach (var action in gameMenu.ActionList) _actionList.Add(action.Key, action.Value);
        #endregion

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
        GameCamera.SetOffset( centerX: UIPositioner.CenterX, centerY: UIPositioner.CenterY,
            focusObjectX: _player.Body.Position.X+_player.Body.Size.Width/2, focusObjectY: _player.Body.Position.Y + _player.Body.Size.Height / 2,
            borderX2: _background.Size.Width, borderY2: _background.Size.Height);
    }

    protected void SetupDoors(int layer)
    {
        foreach (var door in Doors)
        {
            if (door.Level == PreviousLevel) door.Open();
            RenderList.AddObject(layer, door.Body);
        }
    }
    protected void GetSceneHitboxes()
    {
        if (!IsDataRead) return;
        foreach (var layer in _dataLayers)
            for (int j = 0; j < layer.Objects?.Count; j++)
                switch (layer.Name)
                {
                    case "Door": Doors.Add(new Door(DoorDictionary[layer.Objects[j].Name],
                        layer.Objects[j].X, layer.Objects[j].Y,
                        Collider.CreateHitbox(layer.Objects[j]))); break;

                    case "DoorWay": DoorWays.Add(Collider.CreateHitbox(layer.Objects[j])); break;
                    case "Ground": Ground.Add(Collider.CreateHitbox(layer.Objects[j])); AddHitboxGraphicalRepresentation(layer.Objects[j]);
                        RenderList.AddObject(0, new Ground(layer.Objects[j].X, layer.Objects[j].Y, layer.Objects[j].Width, layer.Objects[j].Height)); break;
                    case "LevelBorders": LevelBorders.Add(Collider.CreateHitbox(layer.Objects[j])); AddHitboxGraphicalRepresentation(layer.Objects[j]); break;
                    case "PlayerSpawnPoint": PlayerSpawnPoint = new Point((int)layer.Objects[j].X, (int)layer.Objects[j].Y); break;
                    default: break;
                }
    }

    private void AddHitboxGraphicalRepresentation(TiledObject obj)
    {
        LevelHitboxes.Add(new Box(obj.Name, obj.X, obj.Y, obj.Width, obj.Height, Color.Black, isFilled: false, isCameraAffected: true));
        RenderList.AddObject(0, LevelHitboxes[LevelHitboxes.Count-1]);
        LevelHitboxes[LevelHitboxes.Count - 1].ToggleVisibility();

        LevelHitboxes.Add(new GameUIText("HitboxPosition", obj.X - 5, obj.Y - 35, $"X: {obj.X}, Y: {obj.Y}", "Times New Roman", 18, true));
        RenderList.AddObject(0, LevelHitboxes[LevelHitboxes.Count - 1]);
        LevelHitboxes[LevelHitboxes.Count - 1].ToggleVisibility();
    }
    protected void LocatePlayerInLevel()
    {
        int playerX = PlayerSpawnPoint.X;
        int playerY = PlayerSpawnPoint.Y;

        if (PreviousLevel != GameLevels.None)
            foreach (var door in Doors) if (door.Level == PreviousLevel)
                {
                    _player.SetPositionByCenter((door.Body.Position.X + door.Body.Size.Width / 2) - _player.Body.Size.Width / 2, playerY);
                    return;
                }

        _player.SetPositionByCenter(playerX, playerY);
    }


    public override void HandleUserInput()
    {
        base.HandleUserInput();
        if (!_isInSettings && _isPlayerCanMove) _player.HandleUserInput(InputHandler.LastKeyboardEvent);

        #region MouseEvent Handling

        var coordinates = InputHandler.LastMouseEvent.Location;
        HandleRenderedObjectsMouseEvents(coordinates, RenderList, _actionList);

        #endregion

        #region KeyboardEvent Handling

        if (InputHandler.NoKeyboardEvents) return;
        if (InputHandler.LastKeyboardEvent.IsHandled) return;

        if (_isRightsGrantProcedureStarted && InputHandler.LastKeyboardEvent.Key != Keys.F10)
        {
            _isRightsGrantProcedureStarted = false;
        }

        if (InputHandler.LastKeyboardEvent.Key == Keys.Escape && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _isInSettings = !_isInSettings;
            gameMenu.ToggleVisibility();
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }
        else if (_isInDeveloperMode && InputHandler.LastKeyboardEvent.Key == Keys.F2 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _showServiceData = !_showServiceData;
            _playerData.ToggleVisibility();
            _playerData1.ToggleVisibility();
            _playerInfoBg.ToggleVisibility();
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }
        else if (_isInDeveloperMode && InputHandler.LastKeyboardEvent.Key == Keys.F3 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _playerHitboxVisualization.ToggleVisibility();
            _playerHitboxPosText.ToggleVisibility();
            _playerHitboxPosTextBG.ToggleVisibility();
            foreach (var obj in LevelHitboxes) obj.ToggleVisibility();
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }
        else if (InputHandler.LastKeyboardEvent.Key == Keys.F9 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _isRightsGrantProcedureStarted = true;
        }
        else if (_isRightsGrantProcedureStarted && InputHandler.LastKeyboardEvent.Key == Keys.F10 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _isRightsGrantProcedureStarted = false;
            _isInDeveloperMode = !_isInDeveloperMode;
        }
        else if (_isInDeveloperMode && InputHandler.LastKeyboardEvent.Key == Keys.F11 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _isLevelChooseProcedureStarted = !_isLevelChooseProcedureStarted;
        }
        else if (_isInDeveloperMode && _isLevelChooseProcedureStarted && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            switch (InputHandler.LastKeyboardEvent.Key)
            {
                case Keys.D1: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_1; break;
                case Keys.D2: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_2; break;
                case Keys.D3: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_3; break;
                case Keys.D4: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_4; break;
                case Keys.D5: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_5; break;
                case Keys.D6: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_6; break;
                case Keys.D7: _nextState = States.InGameActive; _nextLevel = GameLevels.Level_7; break;
                default: break;
            }
        }

        #endregion
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        _player.Update(timeDelta, timeRemaining, timeDifference);
        if (_player.Body.Position.Y > _background.Size.Height) _player.SetPositionByCenter(PlayerSpawnPoint.X, PlayerSpawnPoint.Y-1);
        GameCamera.UpdateOffsetByDifference(_player.Body.Position.X, _player.Body.Position.Y);

        if (_player.CurrentState != ActorState.Falling && _player.CurrentState != ActorState.Jumping)
        {
            if (Collider.DetectFullEnterCollision(_player.Hitbox, DoorWays)) _player.OnDoorWayEnter();
            else if (_player.IsInFrontOfDoor) _player.OnDoorWayLeave();

            if (_isRightAnswerGiven && Collider.DetectPartialEnterCollision(_player.Hitbox, DoorWays) &&
                !HintForFinalLevelWindow.IsVisible && !HintForFinalLevelWindow.IsProposed)
            {
                HintForFinalLevelWindow.ToggleVisibility(true);
                HintForFinalLevelWindow.IsProposed = true;
            }
            else if (!Collider.DetectPartialEnterCollision(_player.Hitbox, DoorWays))
                HintForFinalLevelWindow.ToggleVisibility(false);


            foreach (var door in Doors)
            {
                if (_player.IsInFrontOfDoor && Collider.DetectFullEnterCollision(_player.Hitbox, door.Hitbox))
                {
                    if (_isRightAnswerGiven || door.Level == PreviousLevel)
                    {
                        _nextState = States.InGameActive;
                        _nextLevel = door.Level;
                    }
                    else
                    {
                        _failedQuestionNotification.ToggleVisibility();
                        _failWindowAppearTime = timeRemaining;

                        _player.SetPositionByCenter(_player.Body.Position.X + _player.Body.Size.Width / 2, PlayerSpawnPoint.Y);
                        _player.OnDoorEnterFail();
                    }
                }
            }
        }
        if (_player.CurrentState == ActorState.Falling && Collider.DetectAccuratelyPartialEnterCollision(_player.Hitbox, Ground) && !_player.IsGoingThrough)
        {
            IsPlayerOnTheGround = true;
            _player.SetPosition(Collider.lastPartialEnterTouchPosition);
            _player.OnGroundHit();
        }
        else if (_player.CurrentState != ActorState.Falling && !Collider.DetectPartialEnterCollision(_player.Hitbox, Ground) && _player.Direction != LookDirection.Back)
        {
            IsPlayerOnTheGround = false;
            _player.OnGroundLeave();
        }
        else if (_player.CurrentState == ActorState.Falling && !Collider.DetectPartialEnterCollision(_player.Hitbox, Collider.lastAccuratePartialEnteredHitbox) && _player.IsGoingThrough) _player.IsGoingThrough = false;

        if (Collider.DetectAccuratelyPartialEnterCollision(_player.Hitbox, LevelBorders))
        {
            _player.SetPositionByCenter(
                Collider.lastAccuratePartialEnteredHitbox.X < _player.Hitbox.X ?
                Collider.lastAccuratePartialEnteredHitbox.X + Collider.lastAccuratePartialEnteredHitbox.Width + _player.Hitbox.Width / 2
                : Collider.lastAccuratePartialEnteredHitbox.X - _player.Hitbox.Width / 2,
                _player.Body.Position.Y + (_player.Body.Size.Height / 2));
            _player.OnLevelBorderHit();
        }

        if (_failedQuestionNotification.Background.IsVisible && timeRemaining - _failWindowAppearTime >= 6000) _failedQuestionNotification.ToggleVisibility();

        if (_nextLevel != GameLevels.None) AwaitedLevel = _nextLevel;

        ServiceDataUpdate(timeDelta);
        if (IsLevelTaskFulfilled()) OnLevelTaskFulfilled();
        if (_finalPhrase.IsVisible)
        {
            if (_finalPhraseAppearTime < 0) _finalPhraseAppearTime = timeRemaining;
            else if (timeRemaining - _finalPhraseAppearTime > 8000)
            {
                _finalPhrase.ToggleVisibility(false);
            }
        }
    }

    protected void HandleRenderedObjectsMouseEvents(Point mouseCoordinates, RenderLayerList objects, Dictionary<string, Action> actionList)
    {
        foreach (var layer in objects.Layers())
            foreach (var obj in layer)
            {
                if (!obj.IsVisible) continue;
                if (obj is ComplexDrawableObject && !(obj is IInteractive))
                {
                    var complex = (ComplexDrawableObject)obj;
                    HandleRenderedObjectsMouseEvents(mouseCoordinates, complex.RenderList, actionList);
                    continue;
                }
                if (!(obj is IInteractive)) continue;

                var iObj = (IInteractive)obj;


                if (obj.Position.X <= mouseCoordinates.X && obj.Position.Y <= mouseCoordinates.Y
                    && obj.Position.X + obj.Size.Width >= mouseCoordinates.X && obj.Position.Y + obj.Size.Height >= mouseCoordinates.Y)
                {
                    iObj.OnMouseEnter();
                    if (InputHandler.LastMouseEvent.IsLeftPressed) iObj.OnMouseDown();
                    else if (!InputHandler.LastMouseEvent.IsRightPressed) iObj.OnMouseUp(actionList[obj.Name]);
                }
                else if (iObj.HasMouseOver) iObj.OnMouseLeave();
            }
    }

    protected virtual bool IsLevelTaskFulfilled() => false;
    protected virtual void OnLevelTaskFulfilled() {
        _isRightAnswerGiven = true;
        foreach (var door in Doors)
        {
            if (door.Level != PreviousLevel) door.Open();
        }
        _finalPhrase.Text.Text = FinalPhraseText;
        _finalPhrase.ToggleVisibility(true);
    }

    private void ServiceDataUpdate(float timeDelta)
    {
        if (!_showServiceData) return;

        _playerData.Text = $"Current Animation: {_player.CurrentAnimation}\n - IsStarted: {_player.Body.IsAnimationStarted}" +
            $"\n - IsInfinite: {_player.Body.IsAnimationInfinite}" +
            $"\n - Current Frame: {_player.Body.CurrentFrame}" +
            $"\n - Current X Frame: {_player.Body.CurrentXFrame}" +
            $"\n - Current Y Frame: {_player.Body.CurrentYFrame}\n" +
            $"\n - Current State: {_player.CurrentState}" +
            $"\n - Player Direction: {_player.Direction}" +
            $"\n - Player Position: {_player.Body.Position.X}, {_player.Body.Position.Y}" +
            $"\n - Is Player Going Through: {_player.IsGoingThrough}" +
            $"\n\n - Player IsInFrontOfDoor: {_player.IsInFrontOfDoor}" +
            $"\n - Player is on the doorway: {_player._isOnTheDoorWay}\n" +
            $"\n - Next Level: {_nextLevel}" +
            $"\n - TimeDelta: {timeDelta}" +
            $"\n - FPS: {1/timeDelta}" +
            $"\n - Player Is on Ground: {IsPlayerOnTheGround}" +
            $"\n - Is In Settings: {_isInSettings}" +
            $"\n - Camera offsetX: {GameCamera.OffsetX}" +
            $"\n - Camera offsetY: {GameCamera.OffsetY}" +
            $"\n - Current speed: ({_player.CurrentSpeed.X}) ({_player.CurrentSpeed.Y})" +
            $"\n - Current jump speed: {_player._currentJumpSpeed}" +
            $"\n - IsCollidable: {_player.IsCollidable}" +
            $"\n - Last Key Value: {InputHandler.LastKeyboardEvent.Key}";
        _playerData1.Text = $"In Lelvel choose mode: {_isLevelChooseProcedureStarted}" +
            $"\nIn Level choose mode press 1-7 to choose level";

        _playerHitboxVisualization.Position = new PointF(_player.Hitbox.X, _player.Hitbox.Y);
        _playerHitboxPosText.Position = new PointF(_player.Hitbox.X - 5, _player.Hitbox.Y - 40);
        _playerHitboxPosTextBG.Position = new PointF(_player.Hitbox.X - 10, _player.Hitbox.Y - 40);
        _playerHitboxPosText.Text = $"X: {_player.Hitbox.X}\nY: {_player.Hitbox.Y}";
    }

}

