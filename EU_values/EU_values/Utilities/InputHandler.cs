using EU_values.Utilities.Events;

namespace EU_values.Utilities;

public static class InputHandler
{
    public static List<KeyEventArgs> KeyboardEvents = [];
    public static GameMouseEvent[] MouseEvents = new GameMouseEvent[3];

    public static GameMouseEvent LastMouseEvent => MouseEvents[0];
    public static KeyEventArgs LastKeyboardEvent => KeyboardEvents[0];

    public static bool NoMouseEvents = true;
    public static bool NoKeyboardEvents = true;


    public static void AddEvent(KeyEventArgs e)
    {
        KeyboardEvents.Insert(KeyboardEvents.Count, e);
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

    //public static void ClearEvents()
    //{
    //    KeyboardEvents.Clear();
    //    NoKeyboardEvents = true;
    //    NoMouseEvents = true;
    //}
}
