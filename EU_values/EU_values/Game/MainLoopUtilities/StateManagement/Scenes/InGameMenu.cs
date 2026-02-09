using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes;

public class InGameMenu
{
    public GameUIText Title;
    public GameUIButton SaveGameButton;
    public GameUIButton GoToSettingsButton;
    public GameUIButton QuiteGameButton;

    public Box MenuBackground;

    public Dictionary<string, Action> ActionList { get; private set; }

    public InGameMenu()
    {
        MenuBackground = new("InGameMenuBackground", x: -300, y: -460, width: 600, height: 620, color: Color.FromArgb(120, Color.Black));
        MenuBackground.StickToWindowPart(WindowPart.Center);

        Title = new("InGameMenu_Title",
            x: 0, y: -430,
            text: "EU Values", fontFamily: "Times New Roman", fontSize: 36);
        Title.ChangeTextAlignment(TextPositioning.Center);
        Title.StickToWindowPart(WindowPart.Center);

        SaveGameButton = new("InGameMenu_SaveGameButton",
            x: -150, y: -320, width: 300, height: 46,
            text: "Save Game", textX: 150, textY: 10, textSize: 26);
        SaveGameButton.ChangeTextAlignment(TextPositioning.Center);
        SaveGameButton.StickToWindowPart(WindowPart.Center);

        GoToSettingsButton = new("InGameMenu_GoToSettingsButton",
            x: -150, y: -250, width: 300, height: 46,
            text: "Settings", textX: 150, textY: 10, textSize: 26);
        GoToSettingsButton.ChangeTextAlignment(TextPositioning.Center);
        GoToSettingsButton.StickToWindowPart(WindowPart.Center);

        QuiteGameButton = new("InGameMenu_ExitGameButton",
            x: -150, y: -180, width: 300, height: 46,
            text: "Quite Game", textX: 150, textY: 10, textSize: 26);
        QuiteGameButton.ChangeTextAlignment(TextPositioning.Center);
        QuiteGameButton.StickToWindowPart(WindowPart.Center);

        ActionList = new()
        {
            [SaveGameButton.Name] = () => { ActionInjector.RequestAction(ActionType.SaveGame)(); },
            [GoToSettingsButton.Name] = () => { },
            [QuiteGameButton.Name] = () => { ActionInjector.RequestAction(ActionType.QuiteGame)(); },
        };

        ToggleVisibility();
    }

    internal void ToggleVisibility()
    {
        MenuBackground.ToggleVisibility();
        Title.ToggleVisibility();
        SaveGameButton.ToggleVisibility();
        GoToSettingsButton.ToggleVisibility();
        QuiteGameButton.ToggleVisibility();
    }
}
