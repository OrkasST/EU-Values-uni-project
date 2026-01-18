using EU_values.Game.MainLoopUtilities.UIUtilities;
using EU_values.Game.MainLoopUtilities.StateManagement;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;
using EU_values.Utilities;

namespace EU_values.Game;

public enum GameLevels { None, Level_1, Level_2 }

internal class GameManager
{
    private Renderer _renderer;
    private FullScreen _fullScreen;
    private RenderLayerList _currentLayerList;

    private Scene _currentScene;

    public GameState State { get; private set; }
    public Clock Clock { get; private set; }

    public Camera GameCamera { get; private set; }

    public GameManager() { 
        _renderer = new Renderer(Color.Black);
        _fullScreen = new FullScreen();
        _currentLayerList = new RenderLayerList();

        _currentScene = new EmptyScene();

        State = new GameState();
        Clock = new Clock();
        GameCamera = new Camera();
    }

    public void Initialize()
    {
        //SettingsApplier.ReadSettings();

        //if (SettingsApplier.GameSettings)

        ChangeGameState(States.InMainMenu);
    }
    public void UpdateGameState(Form form)
    {
        if (State.CurrentState == States.NotInitialized)
        {
            Initialize();
            return;
        }
        if (State.CurrentState == States.IsUpdating) return;

        Clock.Tick();

        if (_currentScene.AwaitedGameState != States.None)
        {
            if (_currentScene.AwaitedGameState == States.InGameActive)
            {
                State.SetGameLevel(_currentScene.AwaitedLevel);
                ChangeGameState(States.InGameActive);
            }
        }

        State.SetState(States.IsUpdating);
        _currentScene.HandleUserInput();
        _currentScene.Update(Clock.TimeDelta, Clock.TimeRemaining, Clock.TimeDifference);

        State.SetState(State.LastState);
    }
    public void RenderGameObjects(Graphics g)
    {
        if (State.CurrentState == States.NotInitialized) return;
        if (State.CurrentState == States.IsUpdating) return;

        _renderer.ClearScreen(g);
        _renderer.Render(g, _currentLayerList);
    }

    private void ChangeGameState(States state) {
        State.SetState(state);
        switch (state)
        {
            case States.InMainMenu: LoadScene(new MainMenuScene()); break;

            case States.InGameActive: LoadLevel(); break;

            default: break;
        }
    }

    private void LoadLevel()
    {
        switch (State.CurrentGameLevel)
        {
            case GameLevels.Level_1: LoadScene(new Level1Scene(State.LastGameLevel, GameCamera.SetOffset)); break;
            case GameLevels.Level_2: LoadScene(new Level2Scene(State.LastGameLevel, GameCamera.SetOffset)); break;
            default: break;
        }
    }

    private void LoadScene(Scene scene)
    {
        _currentScene = scene;
        _currentLayerList = _currentScene.RenderList;
    }
}
