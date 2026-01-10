namespace EU_values.Game.MainLoopUtilities.StateManagement;

public abstract class Scene
{
    public RenderLayerList RenderList { get; protected set; } = new RenderLayerList();

    public virtual void Update(int timeDelta, int timeRemaining)
    {
        for (int layer = 0; layer < RenderList.Layers.Count; layer++)
            for (int i = 0; i < RenderList.Layers[layer].Count; i++)
                RenderList.Layers[layer][i].Update(timeDelta, timeRemaining);
    }

    public virtual void HandleUserInput() { }
}
