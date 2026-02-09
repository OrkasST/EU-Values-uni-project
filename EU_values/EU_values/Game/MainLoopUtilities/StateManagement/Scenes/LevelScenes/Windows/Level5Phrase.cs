using EU_values.Game.UI.Elements;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class Level5Phrase
{
    public GameUIText Phrase;
    public bool IsExcluded = false;

    public Level5Phrase(float x, float y, string text, int fontSize)
    {
        Phrase = new(text, x, y, text, fontFamily: "Times New Roman", fontSize, isCameraAffected: true);
    }
}
