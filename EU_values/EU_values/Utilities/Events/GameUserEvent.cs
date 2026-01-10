namespace EU_values.Utilities.Events;
public enum GameUserEventType { MouseMove, MouseDown, MouseUp, KeyDown, KeyUp }

public abstract class GameUserEvent
{
    public GameUserEventType EventType;
    public GameUserEvent(GameUserEventType type)
    {
        EventType = type;
    }
}
