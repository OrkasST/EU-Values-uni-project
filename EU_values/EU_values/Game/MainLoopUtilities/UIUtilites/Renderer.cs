using EU_values.Game.BaseClasses;
using EU_values.Game.MainLoopUtilities.StateManagement;
using EU_values.Game.UI.Elements;
using static System.Net.Mime.MediaTypeNames;

namespace EU_values.Game.MainLoopUtilities.UIUtilities;

public class Renderer
{
    public Color ClearColor { get; private set; } = Color.Black;

    public Renderer(Color clearColor)
    {
        ClearColor = clearColor;
    }

    public void ClearScreen(Graphics g)
    {
        g.Clear(ClearColor);
    }

    public void Render(Graphics g, RenderLayerList list) {
        for (int layer = 0; layer < list.Layers.Count; layer++)
        {
            for (int i = 0; i < list.Layers[layer].Count; i++)
                DrawByType(g, list.Layers[layer][i]);
        }
    }

    private void DrawByType(Graphics g, DrawableObject obj)
    {
        if (obj is GameUIText)
        {
            GameUIText textObj = (GameUIText)obj;
            RenderDrawable(g, textObj);
        }
        else if (obj is ComplexDrawableObject)
        {
            ComplexDrawableObject complexObj = (ComplexDrawableObject)obj;
            Render(g, complexObj.RenderList);
        }
        else if (obj is Box)
        {
            Box box = (Box)obj;
            RenderDrawable(g, box);
        }
    }

    private void RenderDrawable(Graphics g, GameUIText obj)
    {
        g.DrawString(obj.Text, obj.Font, new SolidBrush(obj.TextColor), obj.Position);
    }
    private void RenderDrawable(Graphics g, Box obj)
    {
        var brush = new SolidBrush(obj.BackgroundColor);
        var destRect = new Rectangle(obj.Position, obj.Size);

        g.FillRectangle(brush, destRect);
    }
}
