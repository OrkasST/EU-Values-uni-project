using EU_values.Game.BaseClasses;

namespace EU_values.Game.MainLoopUtilities.StateManagement;

public class RenderLayerList
{
    private List<List<DrawableObject>> _bottomLayers = [];
    private List<List<DrawableObject>> _topLayers = [];

    public List<List<DrawableObject>> Layers()
    {
        var layers = new List<List<DrawableObject>>();

        foreach (var layer in _bottomLayers)
            layers.Add(layer);
        foreach (var layer in _topLayers)
            layers.Add(layer);

        return layers;
    }

    public int AddObject(int layer, DrawableObject obj)
    {
        if (layer >= _bottomLayers.Count)
        {
            _bottomLayers.Add(new List<DrawableObject> { obj });
            layer = _bottomLayers.Count - 1;
        }
        else if (layer < 0) return AddTopObject(-layer, obj);
        else _bottomLayers[layer].Add(obj);

        return _bottomLayers[layer].Count - 1;
    }

    private int AddTopObject(int layer, DrawableObject obj)
    {
        if (layer >= _topLayers.Count)
        {
            _topLayers.Add(new List<DrawableObject> { obj });
            layer = _topLayers.Count-1;
        }
        else _topLayers[layer].Add(obj);

        return _topLayers[layer].Count-1;
    }

    public void ChangeObject(int layer, int index, DrawableObject newObj)
    {
        if (layer >= 0) _bottomLayers[layer][index] = newObj;
        else _topLayers[(-layer) - 1][index] = newObj;
    }

    public void RemoveObject(int layer, int index)
    {
        if (layer >= 0) _bottomLayers[layer].RemoveAt(index);
        else _topLayers[(-layer) - 1].RemoveAt(index);
    }

    public void RemoveObjectRange(int layer, int index, int count)
    {
        for (int i = 0; i < count; i++) RemoveObject(layer, index);
    }
}
