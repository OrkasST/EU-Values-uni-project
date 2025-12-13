using EU_values.Game.MainLoopUtilities.Graphics;
using EU_values.Game.MainLoopUtilities.StateManagement;

namespace EU_values.Game;

internal class GameManager
{
    private Renderer _renderer;
    private GameState _state;

    public GameManager() { 
        _renderer = new Renderer();
        _state = new GameState();
    }

    public void Initialize()
    {

    }
    public void UpdateGameState()
    {

    }
    public void RenderGameObjects(Graphics g)
    {

    }
}
