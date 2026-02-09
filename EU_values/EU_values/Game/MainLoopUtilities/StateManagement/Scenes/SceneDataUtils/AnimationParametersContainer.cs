namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;

public class AnimationParametersContainer
{
    public AnimationParametersContainer(int frameTimeGap, int startXFrame, int startYFrame, int endXFrame, int endYFrame, bool isInfinite)
    {
        FrameTimeGap = frameTimeGap;
        StartXFrame = startXFrame;
        StartYFrame = startYFrame;
        EndXFrame = endXFrame;
        EndYFrame = endYFrame;
        IsInfinite = isInfinite;
    }

    public int FrameTimeGap { get; private set; }
    public int StartXFrame { get; private set; }
    public int StartYFrame { get; private set; }
    public int EndXFrame { get; private set; }
    public int EndYFrame { get; private set; }
    public bool IsInfinite { get; private set; }
}
