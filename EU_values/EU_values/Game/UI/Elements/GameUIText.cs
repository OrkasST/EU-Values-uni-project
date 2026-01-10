using EU_values.Game.BaseClasses;

namespace EU_values.Game.UI.Elements;

public class GameUIText : DrawableObject
{
    public string Text { get; set; }
    public Font Font { get; private set; }
    public Color TextColor { get; private set; }

    public GameUIText(string name, int x, int y, string text, Font font) : base(name, x, y) {
        Text = text;
        Font = new Font(font.FontFamily, font.Size, GraphicsUnit.Pixel);
        TextColor = Color.White;
    }
    public GameUIText(string name, int x, int y, string text, Font font, Color color) : this(name, x, y, text, font)
    {
        TextColor = color;
    }

    public override void Update(int timeDelta, int timeRemaining) { }
    public void Update(Action action) => action();
}
