using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes;

public class SettingsScene: Scene
{
    public GameUIText SceneName;
    public GameUIButton ToggleFullScreenButton;
    public GameUIButton ReturnButton;

    private Dictionary<string, Action> _actionList;
    public Dictionary<string, Action> ActionList { get => _actionList; }

    public SettingsScene(Action returnAction)
    {
        SceneName = new GameUIText("Settings_Name",
            x: 0, y: -200,
            text: "Game Settings", fontFamily: "Times New Roman", fontSize: 46);
        SceneName.ChangeTextAlignment(TextPositioning.Center);
        SceneName.StickToWindowPart(WindowPart.Center);

        ToggleFullScreenButton = new("Settings_toggleFullscreenButton",
            x: -190, y: -100, width: 380, height: 46,
            text: SettingsApplier.Settings.ScreenMode == 1 ?
                "Current Screen Mode: Fullscreen" : "Current Screen Mode: Windowed",
            textX: 190, textY: 10, textSize: 26);
        ToggleFullScreenButton.ChangeTextAlignment(TextPositioning.Center);
        ToggleFullScreenButton.StickToWindowPart(WindowPart.Center);

        ReturnButton = new("Settings_ReturnButton",
            x:-190, y: -30, width: 380, height: 46,
            text: "Back", textX: 190, textY: 10, textSize: 26);
        ReturnButton.ChangeTextAlignment(TextPositioning.Center);
        ReturnButton.StickToWindowPart(WindowPart.Center);

        RenderList.AddObject(0, SceneName);
        RenderList.AddObject(0, ToggleFullScreenButton);
        RenderList.AddObject(0, ReturnButton);

        _actionList = new()
        {
            [ ToggleFullScreenButton.Name ] = () =>
            {
                SettingsApplier.ToggleScereenMode();
                ToggleFullScreenButton.Text = SettingsApplier.Settings.ScreenMode == 1 ?
                    "Current Screen Mode: Fullscreen" : "Current Screen Mode: Windowed";
            },
            [ ReturnButton.Name ] = () => {
                returnAction();
            }
        };
    }
}
