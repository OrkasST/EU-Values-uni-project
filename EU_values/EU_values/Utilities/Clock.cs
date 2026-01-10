namespace EU_values.Utilities;

public class Clock
{
    public int LastUpdateTime { get; private set; } = DateTime.Now.Millisecond;
    public int TimeDelta { get; private set; }
    public int TimeRemaining { get; private set; }
    private void UpdateTimeDelta()
    {
        TimeDelta = DateTime.Now.Millisecond > LastUpdateTime
            ? DateTime.Now.Millisecond - LastUpdateTime
            : 1000 - LastUpdateTime + DateTime.Now.Millisecond;
    }
    private void UpdateTimeRemaining()
    {
        TimeRemaining += TimeDelta;
    }

    public void Tick()
    {
        LastUpdateTime = DateTime.Now.Millisecond;
        UpdateTimeDelta();
        UpdateTimeRemaining();
    }
}
