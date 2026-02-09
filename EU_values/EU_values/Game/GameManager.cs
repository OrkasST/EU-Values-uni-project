using EU_values.Game.MainLoopUtilities.StateManagement;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;
using EU_values.Game.MainLoopUtilities.UIUtilities;
using EU_values.Utilities;
using System.Text.Json;

namespace EU_values.Game;

public enum GameLevels { None, Level_1, Level_2, Level_3, Level_4, Level_5, Level_6, Level_7 }
public enum ActionType { QuiteGame, SaveGame }

public class GameManager
{
    private Renderer _renderer;
    private FullScreen _fullScreen;

    private Scene _currentScene;

    public GameState State { get; private set; }
    public Clock Clock { get; private set; }
    public Camera GameCamera { get; private set; }

    public static string SaveLocation = "..\\..\\..\\Docs\\Saves\\Save.json";
    public SaveStructure GameInfo = new();

    public GameManager() {
        _renderer = new Renderer(Color.Black);
        _fullScreen = new FullScreen();

        _currentScene = new EmptyScene();

        State = new GameState();
        Clock = new Clock();
        GameCamera = new Camera();
    }

    public void Initialize(Form form)
    {
        SettingsApplier.ReadSettings();

        ActionInjector.ApplyDictionary(new Dictionary<ActionType, Action>
        {
            [ActionType.QuiteGame] = () => { if (State.CurrentState == States.InGameActive) Save(); this.Quite(form); },
            [ActionType.SaveGame] = () => { Save(); }
        });

        if (SettingsApplier.Settings.ScreenMode == 1) _fullScreen.EnterFullScreenMode(form);

        ChangeGameState(States.InMainMenu);
    }

    public void UpdateGameState(Form form)
    {
        if (form.Width != UIPositioner.FormWidth || form.Height != UIPositioner.FormHeight)
        {
            UIPositioner.UpdateFormDimensions(form.Width, form.Height);
            foreach (var layer in _currentScene.RenderList.Layers())
                foreach (var obj in layer) obj.OnScreenResize();
        }

        if (SettingsApplier.IsPropertyChanged)
        {
            if (SettingsApplier.Settings.ScreenMode == 1) _fullScreen.EnterFullScreenMode(form);
            else _fullScreen.LeaveFullScreenMode(form);

            SettingsApplier.OnChangesApplied();
        }

        if (State.CurrentState == States.NotInitialized)
        {
            Initialize(form);
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
            } else if (_currentScene.AwaitedGameState == States.InMainMenu)
            {
                GameInfo.Level = 1;
                Save();
                ChangeGameState(States.InMainMenu);
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
        _renderer.Render(g, _currentScene.RenderList, new SizeF(_currentScene.GameCamera.OffsetX, _currentScene.GameCamera.OffsetY));
    }

    private void ChangeGameState(States state) {
        State.SetState(state);
        switch (state)
        {
            case States.InMainMenu: LoadScene(new MainMenuScene(LoadSave())); break;

            case States.InGameActive: LoadLevel(); break;

            default: break;
        }
    }

    private void LoadLevel()
    {
        switch (State.CurrentGameLevel)
        {
            case GameLevels.Level_1: GameInfo.Level = 1; LoadScene(new Level1Scene(GameLevels.None)); break;
            case GameLevels.Level_2: GameInfo.Level = 2; LoadScene(new Level2Scene(GameLevels.Level_1)); break;
            case GameLevels.Level_3: GameInfo.Level = 3; LoadScene(new Level3Scene(GameLevels.Level_2)); break;
            case GameLevels.Level_4: GameInfo.Level = 4; LoadScene(new Level4Scene(GameLevels.Level_3)); break;
            case GameLevels.Level_5: GameInfo.Level = 5; LoadScene(new Level5Scene(GameLevels.Level_4)); break;
            case GameLevels.Level_6: GameInfo.Level = 6; LoadScene(new Level6Scene(GameLevels.Level_5)); break;
            case GameLevels.Level_7: GameInfo.Level = 7; LoadScene(new Level7Scene(GameLevels.Level_6)); break;
            default: break;
        }
    }

    private void LoadScene(Scene scene)
    {
        _currentScene = scene;
    }

    private void Save()
    {
        string jsonString = JsonSerializer.Serialize(GameInfo);
        File.WriteAllText(SaveLocation, jsonString);
    }

    private GameLevels LoadSave()
    {
        if (!File.Exists(SaveLocation)) return GameLevels.Level_1;

        string source = File.ReadAllText(SaveLocation);
        var tempData = JsonSerializer.Deserialize<SaveStructure>(source);
        if (tempData != null) GameInfo = tempData;

        switch (GameInfo.Level)
        {
            case 2: return GameLevels.Level_2;
            case 3: return GameLevels.Level_3;
            case 4: return GameLevels.Level_4;
            case 5: return GameLevels.Level_5;
            case 6: return GameLevels.Level_6;
            case 7: return GameLevels.Level_7;
            default: return GameLevels.Level_1;
        }
    }

    private void Quite(Form form)
    {
        form.Close();
    }
}
