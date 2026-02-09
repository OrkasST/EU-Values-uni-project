using EU_values.Game.BaseClasses;

namespace EU_values.Game.UI.Elements;

public class GameWindow: ComplexDrawableObject
{
    public Box Background { get; private set; }
    public GameUIText Text { get; private set; }

    private List<PointF> _objectsOffsets = [];

    public GameWindow(string name, float x, float y, float width, float height, string text, float textX, float textY, int textSize, bool? isCameraAffected = false)
        : base(name, x, y, width, height, isCameraAffected)
    {
        Background = new Box(name + "_default", x, y, width, height, Color.FromArgb(180, Color.Black), true, isCameraAffected);
        RenderList.AddObject(0, Background);
        _objectsOffsets.Add(new(0,0));

        Text = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.White, isCameraAffected);
        RenderList.AddObject(0, Text);

        _objectsOffsets.Add(new (textX, textY));
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }

    public virtual void ChangePosition(float x, float y)
    {
        Position = new(x,y);
        int count = 0;

        foreach (var layer in RenderList.Layers())
            foreach (var obj in layer)
            {
                obj.Position = new PointF(x + _objectsOffsets[count].X, y + _objectsOffsets[count].Y);
                count++;
            }
    }

    public void AddObject(DrawableObject obj, int layer)
    {
        RenderList.AddObject(layer < 1 ? 1 : layer, obj);
        _objectsOffsets.Add(new(obj.Position.X - Position.X, obj.Position.Y - Position.Y));
    }
}
