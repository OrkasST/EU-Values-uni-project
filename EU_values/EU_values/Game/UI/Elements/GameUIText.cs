using EU_values.Game.BaseClasses;

namespace EU_values.Game.UI.Elements;

public class GameUIText : DrawableObject
{
    public string Text { get; set; }
    public Font Font { get; private set; }
    public Color TextColor { get; private set; }

    public GameUIText(string name, int x, int y, string text, string fontFamily, int fontSize, bool? isCameraAffected = false) : base(name, x, y, isCameraAffected) {
        Text = text;
        Font = new Font(fontFamily, fontSize, GraphicsUnit.Pixel);
        TextColor = Color.White;
    }
    public GameUIText(string name, int x, int y, string text, string fontFamily, int fontSize, Color color, bool? isCameraAffected = false) : this(name, x, y, text, fontFamily, fontSize, isCameraAffected)
    {
        TextColor = color;
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }
    public void Update(Action action) => action();
}
