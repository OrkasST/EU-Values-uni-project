using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;
using System.Text.Json;

namespace EU_values.Game.MainLoopUtilities.StateManagement;

public abstract class InGameScene: Scene
{
    protected TiledMap? _data;
    protected Action<int, int, int, int> SetCameraOffset;
    public InGameScene(Action<int, int, int, int> setCameraOffsset)
    {
        SetCameraOffset = setCameraOffsset;
    }

    public void ReadData(string filePath)
    {
        if (!File.Exists(filePath)) return;

        var source = File.ReadAllText(filePath);
        _data = JsonSerializer.Deserialize<TiledMap>(source);
    }
}
