using EU_values.Utilities.Events;

namespace EU_values.Utilities;

public static class InputHandler
{
    public static GameKeyboardEvent[] KeyboardEvents = new GameKeyboardEvent[3];
    public static GameMouseEvent[] MouseEvents = new GameMouseEvent[3];

    public static GameMouseEvent LastMouseEvent => MouseEvents[0];
    public static GameKeyboardEvent LastKeyboardEvent => KeyboardEvents[0];

    public static bool NoMouseEvents = true;
    public static bool NoKeyboardEvents = true;


    public static void AddEvent(GameKeyboardEvent e)
    {
        if (KeyboardEvents[0] != null && e.EventType == LastKeyboardEvent.EventType && e.Key == LastKeyboardEvent.Key) return;

        if (KeyboardEvents[0] == null) KeyboardEvents[0] = e;
        else if (KeyboardEvents[1] == null)
        {
            KeyboardEvents[1] = KeyboardEvents[0];
            KeyboardEvents[0] = e;
        }
        else
        {
            KeyboardEvents[2] = KeyboardEvents[1];
            KeyboardEvents[1] = KeyboardEvents[0];
            KeyboardEvents[0] = e;
        }
        NoKeyboardEvents = false;
    }
    public static void AddEvent(GameMouseEvent e)
    {
        if (MouseEvents[0] == null) MouseEvents[0] = e;
        else if (MouseEvents[1] == null)
        {
            MouseEvents[1] = MouseEvents[0];
            MouseEvents[0] = e;
        }
        else
        {
            MouseEvents[2] = MouseEvents[1];
            MouseEvents[1] = MouseEvents[0];
            MouseEvents[0] = e;
        }
        NoMouseEvents = false;
    }
}
