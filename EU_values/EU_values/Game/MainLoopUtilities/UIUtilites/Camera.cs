using EU_values.Game.BaseClasses;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.UIUtilities;

public class Camera
{
    public float OffsetX { get; private set; }
    public float OffsetY { get; private set; }

    public float FocuseObjectX { get; private set; }
    public float FocuseObjectY { get; private set; }

    public float BorderX1 { get; private set; } = 0;
    public float BorderY1 { get; private set; } = 0;
    public float BorderX2 { get; private set; } = -1;
    public float BorderY2 { get; private set; } = -1;

    public float ScreenX { get; private set; } = 0;
    public float ScreenY { get; private set; } = 0;
    public float ScreenWidth { get; private set; } = 0;
    public float ScreenHeight { get; private set; } = 0;

    private bool _isScreenHeightBigger = false;
    private bool _isScreenWidthBigger = false;


    public Camera(int centerX, int centerY, int focusObjectX, int focusObjectY)
    {
        SetOffset(centerX, centerY, focusObjectX, focusObjectY, -1, -1);
    }
    public Camera() : this(0,0,0,0) { }

    public void UpdateOffset(float x, float y)
    {
        OffsetX -= x;
        OffsetY -= y;
        CheckBorders();
    }

    public void UpdateOffsetByDifference(float newFocusX, float newFocusY)
    {
        OffsetX -= newFocusX - FocuseObjectX;
        OffsetY -= newFocusY - FocuseObjectY;

        FocuseObjectX = newFocusX;
        FocuseObjectY = newFocusY;

        UpdateScreenPosition();
        CheckBorders();
    }
    public void SetOffset(float x, float y)
    {
        OffsetX = x;
        OffsetY = y;
        CheckBorders();
    }
    public void SetOffset(float centerX, float centerY, float focusObjectX, float focusObjectY, float borderX2, float borderY2)
    {
        OffsetX = centerX - focusObjectX;
        OffsetY = centerY - focusObjectY;

        FocuseObjectX = focusObjectX;
        FocuseObjectY = focusObjectY;

        BorderX2 = borderX2;
        BorderY2 = borderY2;

        _isScreenWidthBigger = false;
        _isScreenHeightBigger = false;
        UpdateScreenSize();
        UpdateScreenPosition();
        CheckBorders();
    }

    public bool CheckBorders()
    {
        bool isOut = false;

        if (BorderX2 < ScreenWidth)
        {
            OffsetX = UIPositioner.CenterX - BorderX2 / 2;
            _isScreenWidthBigger = true;
        }
        if (BorderY2 < ScreenHeight)
        {
            OffsetY = UIPositioner.CenterY - BorderY2 / 2;
            _isScreenHeightBigger = true;
        }

        if (!_isScreenWidthBigger && ScreenX < BorderX1 )
        {
            isOut = true;
            OffsetX = 0;
        }
        if (!_isScreenHeightBigger && ScreenY < BorderY1)
        {
            isOut = true;
            OffsetY = 0;
        }
        if (!_isScreenWidthBigger && ScreenX + ScreenWidth > BorderX2)
        {
            isOut = true;
            OffsetX = BorderX1 - (BorderX2 - ScreenWidth);
        }
        if (!_isScreenHeightBigger && ScreenY + ScreenHeight > BorderY2)
        {
            isOut=true;
            OffsetY = BorderY1 - (BorderY2 - ScreenHeight) ;
        }
        return isOut;
    }

    private void UpdateScreenPosition()
    {
        ScreenX = FocuseObjectX - ScreenWidth / 2;
        ScreenY = FocuseObjectY - ScreenHeight / 2;
    }
    public void UpdateScreenSize()
    {
        ScreenWidth = UIPositioner.FormWidth;
        ScreenHeight = UIPositioner.FormHeight;
    }
}
