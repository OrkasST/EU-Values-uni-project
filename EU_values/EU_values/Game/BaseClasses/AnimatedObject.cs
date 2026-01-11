using EU_values.Game.MainLoopUtilities.UIUtilities;
using System.Drawing;

namespace EU_values.Game.BaseClasses;

public class AnimatedObject : ComplexDrawableObject
{
    //var rectLocation = new Point(x, y);
    //var rectSize = new Size(size, size);
    //var destRect = new Rectangle(rectLocation, rectSize);

    //var srcRect = new Rectangle(0, 32, 32, 32);

    //g.DrawImage(PlayerImage, destRect, srcRect, GraphicsUnit.Pixel);
    public int CurrentFrame { get; private set; }

    public int CurrentXFrame {  get; private set; }
    public int CurrentYFrame {  get; private set; }

    public int XFramesNumber { get; private set; }
    public int YFramesNumber { get; private set; }

    public int CurrentLastFrame { get; private set; }

    protected int _startingYFrame;
    protected int _startingXFrame;
    protected int _endingYFrame;
    protected int _endingXFrame;

    public int AnimationDuration { get; private set; }
    protected int _stepDuration;

    protected int _stepStartTime;

    public bool IsAnimationStarted { get; private set; } = false;
    public bool IsAnimationInfinite { get; private set; } = false;

    public Rectangle SourceRectangle { get => new Rectangle(CurrentXFrame * Size.Width, CurrentYFrame * Size.Height, Size.Width, Size.Height); }

    public AnimatedObject(string name, int x, int y, int width, int height, Image frameset, int xFramesNumber, int yFramesNumber, bool? isCameraAffected = false) : base(name, x, y, width, height, frameset, isCameraAffected)
    {
        CurrentFrame = 0;
        CurrentXFrame = 0;
        CurrentYFrame = 0;

        XFramesNumber = xFramesNumber;
        YFramesNumber = yFramesNumber;
    }

    private int GetFrame(int x, int y) => y * XFramesNumber + x;

    public void SetFrame(int frame)
    {
        CurrentFrame = frame;
        CurrentYFrame = CurrentFrame/XFramesNumber;
        CurrentXFrame = CurrentFrame - CurrentYFrame * XFramesNumber;
    }
    public void SetFrame(int x, int y)
    {
        CurrentXFrame = x > XFramesNumber ? XFramesNumber-1 : x;
        CurrentYFrame = y > YFramesNumber ? YFramesNumber-1 : y;

        CurrentFrame = CurrentYFrame*XFramesNumber + CurrentXFrame;
    }

    public void StartAnimation(int frameTimeGap, int startXFrame, int startYFrame, int endXFrame, int endYFrame, bool isInfinite, int startTime)
    {
        _stepDuration = frameTimeGap;

        _startingXFrame = startXFrame;
        _startingYFrame = startYFrame;
        _endingXFrame = endXFrame;
        _endingYFrame = endYFrame;

        CurrentLastFrame = GetFrame(_endingXFrame, _endingYFrame);

        IsAnimationInfinite = isInfinite;
        
        _stepStartTime = startTime;
        SetFrame(startXFrame, startYFrame);

        AnimationDuration = (CurrentLastFrame - CurrentFrame) * _stepDuration;
        IsAnimationStarted = true;
    }

    public void StopAnimation()
    {
        IsAnimationStarted = false;
    }

    public bool IsReadyToSwitchFrame(int timeRemaining)
    {
        return _stepStartTime + _stepDuration < timeRemaining;
    }

    public void AnimationStep(int timeRemaining)
    {
        if (!IsReadyToSwitchFrame(timeRemaining)) return;

        var nextFrame = CurrentFrame + 1;

        if (nextFrame > CurrentLastFrame && IsAnimationInfinite) SetFrame(_startingXFrame, _startingYFrame);
        else if (nextFrame <= CurrentLastFrame) SetFrame(nextFrame);

        _stepStartTime = timeRemaining;
    }

    public override void Update(int timeDelta, int timeRemaining) {
        if (IsAnimationStarted)
        {
            AnimationStep(timeRemaining);
        }
    }
}
