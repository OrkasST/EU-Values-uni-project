namespace EU_values.Utilities;

public class Clock
{
    public int LastUpdateTime { get; private set; } = DateTime.Now.Millisecond;
    public int TimeDifference { get; private set; }
    public int TimeRemaining { get; private set; }
    public float TimeDelta { get; private set; }
    private void UpdateTimeDelta()
    {
        TimeDifference = DateTime.Now.Millisecond > LastUpdateTime
            ? DateTime.Now.Millisecond - LastUpdateTime
            : 1000 - LastUpdateTime + DateTime.Now.Millisecond;
        TimeDelta = TimeDifference / 1000f;
    }
    private void UpdateTimeRemaining()
    {
        TimeRemaining += TimeDifference;
    }

    public void Tick()
    {
        UpdateTimeDelta();
        UpdateTimeRemaining();
        LastUpdateTime = DateTime.Now.Millisecond;
    }
}
