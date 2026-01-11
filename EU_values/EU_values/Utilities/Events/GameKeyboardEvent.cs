using Microsoft.VisualBasic.Devices;

namespace EU_values.Utilities.Events;

public class GameKeyboardEvent : GameUserEvent
{
    public Keys Key { get; private set; }
    public bool IsHandled { get; set; } = false;

    public GameKeyboardEvent(GameUserEventType type, Keys key) : base(type)
    {
        Key = key;
    }
}
