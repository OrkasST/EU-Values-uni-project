namespace EU_values.Game.MainLoopUtilities.UIUtilities;

public class Camera
{
    public int OffsetX { get; private set; }
    public int OffsetY { get; private set; }

    public Camera(int centerX, int centerY, int focusObjectX, int focusObjectY)
    {
        SetOffset(centerX, centerY, focusObjectX, focusObjectY);
    }
    public Camera() : this(0,0,0,0) { }

    public void UpdateOffset(int x, int y)
    {
        OffsetX -= x;
        OffsetY -= y;
    }

    public void SetOffset(int x, int y)
    {
        OffsetX = x;
        OffsetY = y;
    }
    public void SetOffset(int centerX, int centerY, int focusObjectX, int focusObjectY)
    {
        OffsetX = centerX - focusObjectX;
        OffsetY = centerY - focusObjectY;
    }
}
