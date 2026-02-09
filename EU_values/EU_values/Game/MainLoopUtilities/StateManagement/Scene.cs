using EU_values.Game.MainLoopUtilities.UIUtilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement;

public abstract class Scene
{
    public RenderLayerList RenderList { get; protected set; } = new RenderLayerList();
    public bool IsRenderListChanged = false;
    public Camera GameCamera = new();

    public States AwaitedGameState { get; protected set; } = States.None;
    protected States _nextState { get; set; } = States.None;

    public GameLevels AwaitedLevel { get; protected set; } = GameLevels.None;
    protected GameLevels _nextLevel { get; set; } = GameLevels.None;

    public virtual void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        var layers = this.RenderList.Layers();

        for (int layer = 0; layer < layers.Count; layer++)
            for (int i = 0; i < layers[layer].Count; i++)
                layers[layer][i].Update(timeDelta, timeRemaining, timeDifference);

        if (_nextState != States.None) AwaitedGameState = _nextState;
    }

    public virtual void HandleUserInput() { }
}
