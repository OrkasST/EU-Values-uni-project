using EU_values.Game.BaseClasses;

namespace EU_values.Game.MainLoopUtilities.StateManagement;

public class RenderLayerList
{
    public List<List<DrawableObject>> Layers { get; private set; } = [];

    public int AddObject(int layer, DrawableObject obj)
    {
        if (layer >= Layers.Count) Layers.Add(new List<DrawableObject> { obj });
        else Layers[layer].Add(obj);

        return Layers[layer].Count - 1;
    }

    public void ChangeObject(int layer, int index, DrawableObject newObj)
    {
        Layers[layer][index] = newObj;
    }
}
