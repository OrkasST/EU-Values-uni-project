using EU_values.Game.Interfaces;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes;

public class MainMenuScene : Scene
{
    private GameUIText _gameName;
    private GameUIButton _startGameButton;
    private GameUIButton _quiteGameButton;
    private GameUIButton _goToSettingsButton;

    private Dictionary<string, Action> _actionList;
    private SettingsScene _settingsSubScene;

    private RenderLayerList _listBackup;
    private bool _isInSettingsSubScene = false;
    private bool _isGoToSettingsClicked = false;
    private bool _backToMainMenuClicked = false;

    public MainMenuScene()
    {
        _gameName = new GameUIText("MainMenu_GameName",
            x: 0, y: -200,
            text: "EU Values", fontFamily: "Times New Roman", fontSize: 46);
        _gameName.ChangeTextAlignment(TextPositioning.Center);
        _gameName.StickToWindowPart(WindowPart.Center);

        _startGameButton = new("MainMenu_startGameButton",
            x: -150, y: -100, width: 300, height: 46,
            text: "Start Game", textX: 150, textY: 10, textSize: 26);
        _startGameButton.ChangeTextAlignment(TextPositioning.Center);
        _startGameButton.StickToWindowPart(WindowPart.Center);

        _goToSettingsButton = new("MainMenu_goToSettingsButton",
            x: -150, y: -35, width: 300, height: 46,
            text: "Settings", textX: 150, textY: 10, textSize: 26);
        _goToSettingsButton.ChangeTextAlignment(TextPositioning.Center);
        _goToSettingsButton.StickToWindowPart(WindowPart.Center);

        _quiteGameButton = new("MainMenu_quiteGameButton",
            x: -150, y: 30, width: 300, height: 46,
            text: "Quite Game", textX: 150, textY: 10, textSize: 26);
        _quiteGameButton.ChangeTextAlignment(TextPositioning.Center);
        _quiteGameButton.StickToWindowPart(WindowPart.Center);

        RenderList.AddObject(0, _gameName);
        RenderList.AddObject(0, _startGameButton);
        RenderList.AddObject(0, _goToSettingsButton);
        RenderList.AddObject(0, _quiteGameButton);

        _listBackup = RenderList;

        _settingsSubScene = new(() =>
        {
            _backToMainMenuClicked = true;
        });

        _actionList = new Dictionary<string, Action>
        {
            [_startGameButton.Name] = () =>
            {
                ActionInjector.RequestAction(ActionType.LoadSave)();
                _nextState = States.InSaveChooseMenu;
            },
            [_quiteGameButton.Name] = () =>
            {
                ActionInjector.RequestAction(ActionType.QuiteGame)();
            },
            [_goToSettingsButton.Name] = () =>
            {
                _isGoToSettingsClicked = true;
            }
        };
    }

    public override void HandleUserInput()
    {
        if (!InputHandler.NoMouseEvents)
        {
            var coordinates = InputHandler.LastMouseEvent.Location;

            foreach (var layer in RenderList.Layers())
                foreach (var obj in layer)
                {
                    if (!(obj is IInteractive)) continue;

                    var iObj = (IInteractive)obj;

                    if (obj.Position.X <= coordinates.X && obj.Position.Y <= coordinates.Y
                        && obj.Position.X + obj.Size.Width >= coordinates.X && obj.Position.Y + obj.Size.Height >= coordinates.Y)
                    {
                        iObj.OnMouseEnter();
                        if (InputHandler.LastMouseEvent.IsLeftPressed) iObj.OnMouseDown();
                        else if (!InputHandler.LastMouseEvent.IsRightPressed) iObj.OnMouseUp(
                            _isInSettingsSubScene ? _settingsSubScene.ActionList[obj.Name] : _actionList[obj.Name] );
                    }
                    else if (iObj.HasMouseOver) iObj.OnMouseLeave();
                }
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        if (_isGoToSettingsClicked && !_isInSettingsSubScene)
        {
            RenderList = _settingsSubScene.RenderList;
            _isInSettingsSubScene = true;
            _isGoToSettingsClicked = false;
        }
        if (_backToMainMenuClicked && _isInSettingsSubScene)
        {
            RenderList = _listBackup;
            _isInSettingsSubScene = false;
            _backToMainMenuClicked = false;
        }

        if (_nextLevel != GameLevels.None) AwaitedLevel = _nextLevel;
    }
}
