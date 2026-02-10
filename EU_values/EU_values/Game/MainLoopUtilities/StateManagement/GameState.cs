namespace EU_values.Game.MainLoopUtilities.StateManagement;

public enum States { None, NotInitialized, InMainMenu, InGameActive, IsUpdating, InGamePaused, InSaveChooseMenu }

public class GameState
{
    public States CurrentState { get; private set; } = States.NotInitialized;
    public States LastState { get; private set; } = States.NotInitialized;

    public GameLevels CurrentGameLevel { get; private set; } = GameLevels.None;
    public GameLevels LastGameLevel { get; private set; } = GameLevels.None;

    public GameState() {
        CurrentState = States.NotInitialized;
        LastState = CurrentState;
    }

    public void SetState(States State)
    {
        LastState = CurrentState;
        CurrentState = State;
    }

    public void SetGameLevel(GameLevels level)
    {
        LastGameLevel = CurrentGameLevel;
        CurrentGameLevel = level;
    }
}
