using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;
using System.Text.Json;

namespace EU_values.Utilities;

public static class SettingsApplier
{
    public static GameSettings Settings = new();

    private static string _filePath = "..\\..\\..\\Docs\\Settings\\GameSettings.json";

    public static bool IsPropertyChanged = false;


    public static void ReadSettings()
    {
        if (!File.Exists(_filePath))
        {
            WriteSettings();
            return;
        }

        string source = File.ReadAllText(_filePath);
        var tempData = JsonSerializer.Deserialize<GameSettings>(source);
        if (tempData != null) Settings = tempData;
    }

    public static void WriteSettings()
    {
        string jsonString = JsonSerializer.Serialize(Settings);
        File.WriteAllText(_filePath, jsonString);
    }

    public static int ToggleScereenMode()
    {
        IsPropertyChanged = true;
        Settings.ScreenMode = Settings.ScreenMode == 0 ? 1 : 0;
        WriteSettings();
        return Settings.ScreenMode;
    }

    public static void OnChangesApplied() => IsPropertyChanged = false;
}
