using System.Collections;

namespace EU_values.Utilities;
public enum ScreenMode { Full, Windowed }

public class GameSettings: IEnumerator<GameSettings>

{
    public ScreenMode ScreenMode = ScreenMode.Full;

    public GameSettings Current => throw new NotImplementedException();

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool MoveNext()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}
