namespace EU_values.Utilities;

public enum WindowPart { Left_Top, Left_Center, Left_Bottom, Center, Right_Top, Right_Center, Right_Bottom }

public static class UIPositioner
{
    public static int FormWidth { get; private set; }
    public static int FormHeight { get; private set; }

    public static void UpdateFormDimensions(int width, int height) 
    {
        FormWidth = width;
        FormHeight = height;
    }

    public static int CenterX { get => FormWidth / 2; }
    public static int CenterY { get => FormHeight / 2; }
}
