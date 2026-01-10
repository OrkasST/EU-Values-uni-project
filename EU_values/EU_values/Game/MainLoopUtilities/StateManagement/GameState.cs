namespace EU_values.Game.MainLoopUtilities.StateManagement;

public enum States { NotInitialized, InMainMenu, InGameActive, IsUpdating, InGamePaused }

public class GameState
{
    public States CurrentState { get; private set; } = States.NotInitialized;
    public States LastState { get; private set; } = States.NotInitialized;

    public GameState() {
        CurrentState = States.NotInitialized;
        LastState = CurrentState;
    }

    public void SetState(States State)
    {
        LastState = CurrentState;
        CurrentState = States.InGameActive;
    }
}
