using EU_values.Game.MainLoopUtilities.UIUtilities;
using EU_values.Game.MainLoopUtilities.StateManagement;
using EU_values.Utilities;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes;

namespace EU_values.Game;

internal class GameManager
{
    private Renderer _renderer;
    private FullScreen _fullScreen;
    private RenderLayerList _currentLayerList;

    private Scene _currentScene;

    public GameState State { get; private set; }
    public Clock Clock { get; private set; }

    public GameManager() { 
        _renderer = new Renderer(Color.Black);
        _fullScreen = new FullScreen();
        _currentLayerList = new RenderLayerList();

        _currentScene = new EmptyScene();

        State = new GameState();
        Clock = new Clock();
    }

    public void Initialize(Form form)
    {
        //SettingsApplier.ReadSettings();

        //if (SettingsApplier.GameSettings)

        ChangeGameState(States.InMainMenu);
    }
    public void UpdateGameState()
    {
        if (State.CurrentState == States.NotInitialized) return;
        if (State.CurrentState == States.IsUpdating) return;

        Clock.Tick();
        State.SetState(States.IsUpdating);

        _currentScene.HandleUserInput();
        _currentScene.Update(Clock.TimeDelta, Clock.TimeRemaining);

        //InputHandler.ClearEvents();
    }
    public void RenderGameObjects(Graphics g)
    {
        if (State.CurrentState == States.NotInitialized) return;
        _renderer.ClearScreen(g);
        _renderer.Render(g, _currentLayerList);
    }

    private void ChangeGameState(States state) {
        State.SetState(state);
        switch (state)
        {
            case States.InMainMenu: LoadScene(new MainMenuScene()); break;

            default: break;
        }
    }

    private void LoadScene(Scene scene)
    {
        _currentScene = scene;
        _currentLayerList = _currentScene.RenderList;
    }
}
