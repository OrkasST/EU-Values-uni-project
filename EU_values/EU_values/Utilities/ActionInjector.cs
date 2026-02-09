using EU_values.Game;

namespace EU_values.Utilities;

public class ActionInjector
{
    private static Dictionary<ActionType, Action> _actionList = [];

    public static void ApplyDictionary(Dictionary<ActionType, Action> dictionary) => _actionList = dictionary;

    public static Action RequestAction(ActionType actionType) => _actionList[actionType];
}
