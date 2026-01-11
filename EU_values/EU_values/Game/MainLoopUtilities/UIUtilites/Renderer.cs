using EU_values.Game.BaseClasses;
using EU_values.Game.MainLoopUtilities.StateManagement;
using EU_values.Game.UI.Elements;
using static System.Net.Mime.MediaTypeNames;

namespace EU_values.Game.MainLoopUtilities.UIUtilities;

public class Renderer
{
    public Color ClearColor { get; private set; } = Color.Black;
    private Size _cameraOffset = Size.Empty;

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
        if (obj.GetType() == typeof(GameUIText))
        {
            GameUIText textObj = (GameUIText)obj;
            RenderDrawable(g, textObj);
        }
        else if (obj.GetType() == typeof(Box))
        {
            Box box = (Box)obj;
            RenderDrawable(g, box);
        }
        else if (obj is ComplexDrawableObject)
        {
            if (obj.GetType() == typeof(AnimatedObject))
            {
                AnimatedObject animObj = (AnimatedObject)obj;
                RenderDrawable(g, animObj);
                return;
            }

            ComplexDrawableObject complexObj = (ComplexDrawableObject)obj;
            Render(g, complexObj.RenderList);
        }
    }

    private void RenderDrawable(Graphics g, GameUIText obj)
    {
        if (obj.IsCameraAffected) g.DrawString(obj.Text, obj.Font, new SolidBrush(obj.TextColor), obj.Position+_cameraOffset);
        else g.DrawString(obj.Text, obj.Font, new SolidBrush(obj.TextColor), obj.Position);
    }
    private void RenderDrawable(Graphics g, Box obj)
    {
        var brush = new SolidBrush(obj.BackgroundColor);
        var destRect = new Rectangle(obj.IsCameraAffected ? obj.Position + _cameraOffset : obj.Position, obj.Size);

        if(!obj.HasImage) g.FillRectangle(brush, destRect);
        else g.DrawImage(obj.BackgroundImage, destRect);
    }
    private void RenderDrawable(Graphics g, AnimatedObject obj)
    {
        var destRect = new Rectangle(obj.IsCameraAffected ? obj.Position + _cameraOffset : obj.Position, obj.Size);

        g.DrawImage(obj.BackgroundImage, destRect, obj.SourceRectangle, GraphicsUnit.Pixel);
    }

    public void ApplyCameraOffset(int offsetX, int offsetY)
    {
        _cameraOffset = new Size(offsetX, offsetY);
    }
}
