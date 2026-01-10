namespace EU_values.Utilities.Events;

public class GameMouseEvent: GameUserEvent
{
    public Point Location { get; private set; }
    public bool IsLeftPressed { get; private set; } = false;
    public bool IsRightPressed { get; private set; } = false;

    public GameMouseEvent(GameUserEventType type, Point location, MouseButtons button) : base(type)
    {
        Location = location;
        if (button == MouseButtons.Left) IsLeftPressed = true;
        else if (button == MouseButtons.Right) IsRightPressed = true;
    }
}
