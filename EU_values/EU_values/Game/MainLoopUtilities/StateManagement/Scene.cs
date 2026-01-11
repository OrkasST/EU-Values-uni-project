namespace EU_values.Game.MainLoopUtilities.StateManagement;

public abstract class Scene
{
    public RenderLayerList RenderList { get; protected set; } = new RenderLayerList();

    public States AwaitedGameState { get; protected set; } = States.None;
    protected States _nextState { get; set; } = States.None;

    public GameLevels AwaitedLevel { get; protected set; } = GameLevels.None;
    protected GameLevels _nextLevel { get; set; } = GameLevels.None;

    public virtual void Update(int timeDelta, int timeRemaining)
    {
        for (int layer = 0; layer < RenderList.Layers.Count; layer++)
            for (int i = 0; i < RenderList.Layers[layer].Count; i++)
                RenderList.Layers[layer][i].Update(timeDelta, timeRemaining);
    }

    public virtual void HandleUserInput() { }
}
