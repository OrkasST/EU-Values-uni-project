using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class LimitLessWordEnterWindow : GameWindow
{
    private static float _width = 800;
    private static float _height = 200;

    public GameWindow TextField { get; private set; }

    public string FieldText { get; private set; } = "";
    public bool IsInEditMode { get; private set; } = false;

    public LimitLessWordEnterWindow(string text) : base("LimitLessWordEnterWindow", x: UIPositioner.CenterX - _width/2, y: UIPositioner.CenterY + 230,
        width: _width, height: _height, text, textX: _width / 2, textY: 30, textSize: 26, isCameraAffected: false)
    {
        Text.ChangeTextAlignment(TextPositioning.Center);

        TextField = new("TextField", x: Background.Position.X + 100, y: Background.Position.Y + 140, width: _width-200, height: 40,
            text: "", textX: 10, textY: 7, textSize: 26);
        TextField.Background.ChangeBackground(Color.Gray);
        TextField.Text.ChangeColor(Color.Black);

        RenderList.AddObject(1, TextField);
    }

    public void StartEditingText()
    {
        TextField.Background.ChangeBackground(Color.White);
        IsInEditMode = true;
    }
    public void StopEditingText()
    {
        TextField.Background.ChangeBackground(Color.Gray);
        IsInEditMode = false;
    }

    public void AddLetter(Keys key)
    {
        if (!InputHandler.IsKeyAChar(key) || !IsInEditMode) return;

        FieldText += $"{key}";
        TextField.Text.Text = FieldText;
    }
    public void RemoveLetter()
    {
        if (!IsInEditMode || FieldText.Length == 0) return;

        FieldText = FieldText.Substring(0, FieldText.Length - 1);
        TextField.Text.Text = FieldText;
    }
}
