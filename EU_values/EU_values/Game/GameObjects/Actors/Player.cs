using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.GameObjects.Actors;

public class Player : Actor
{
    private readonly static string _framesetPath = "..\\..\\..\\Resources\\Images\\PlayerSpritesheet\\PlayerSpriteSheet_02.png";

    public bool IsGoingThrough { get;  set; }

    public Player(string name, float x, float y) : base(name, x, y,
        frameset: new(name, x, y, width: 128, height: 128, frameset: Image.FromFile(_framesetPath), xFramesNumber: 4, yFramesNumber: 13, isCameraAffected: true),
        speed: new PointF(90, 50),
        jumpSpeed: 650,
        hitboxOffsetX: 46, hitboxOffsetY: 0, hitboxWidth: 34, hitboxHeight: 128)
    {
        AnimationParameters = new()
        {
            [AnimationType.LeftIdle] =      new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0,  endXFrame: 3, endYFrame: 0, isInfinite: true),
            [AnimationType.RightIdle] =     new(frameTimeGap: 100, startXFrame: 0, startYFrame: 1,  endXFrame: 3, endYFrame: 1, isInfinite: true),

            [AnimationType.LeftWalk] =      new(frameTimeGap: 100, startXFrame: 0, startYFrame: 2,  endXFrame: 3, endYFrame: 2, isInfinite: true),
            [AnimationType.RightWalk] =     new(frameTimeGap: 100, startXFrame: 0, startYFrame: 3,  endXFrame: 3, endYFrame: 3, isInfinite: true),
            [AnimationType.BackWalk] =      new(frameTimeGap: 100, startXFrame: 0, startYFrame: 4,  endXFrame: 3, endYFrame: 4, isInfinite: true),

            [AnimationType.LeftJump] =      new(frameTimeGap: 100, startXFrame: 0, startYFrame: 5,  endXFrame: 2, endYFrame: 5, isInfinite: true),
            [AnimationType.RightJump] =     new(frameTimeGap: 100, startXFrame: 0, startYFrame: 6,  endXFrame: 2, endYFrame: 6, isInfinite: true),

            [AnimationType.LeftFalling] =   new(frameTimeGap: 100, startXFrame: 0, startYFrame: 7,  endXFrame: 1, endYFrame: 7, isInfinite: true),
            [AnimationType.RightFalling] =  new(frameTimeGap: 100, startXFrame: 0, startYFrame: 8,  endXFrame: 1, endYFrame: 8, isInfinite: true),

            [AnimationType.LeftLanding] =   new(frameTimeGap: 100, startXFrame: 0, startYFrame: 9,  endXFrame: 1, endYFrame: 9, isInfinite: true),
            [AnimationType.RightLanding] =  new(frameTimeGap: 100, startXFrame: 0, startYFrame: 10, endXFrame: 1, endYFrame: 10, isInfinite: true),

            [AnimationType.LeftLadderClimbing] =  new(frameTimeGap: 100, startXFrame: 0, startYFrame: 11, endXFrame: 3, endYFrame: 11, isInfinite: true),
            [AnimationType.RightLadderClimbing] =  new(frameTimeGap: 100, startXFrame: 0, startYFrame: 12, endXFrame: 3, endYFrame: 12, isInfinite: true)
        };
    }

    public void HandleUserInput(GameKeyboardEvent e)
    {
        if (InputHandler.NoKeyboardEvents) return;
        if (e.IsHandled) return;
        if (_isInAir || _isOnTheDoorWay) return;

        switch (e.Key)
        {
            case Keys.D:
                ChooseNextStateByEvent(e, ActorState.Standing, ActorState.Moving);
                ChangeLookDirection(LookDirection.Right); break;

            case Keys.A:
                ChooseNextStateByEvent(e, ActorState.Standing, ActorState.Moving);
                ChangeLookDirection(LookDirection.Left); break;

            case Keys.W: HandleFrontMovementButton(e); break;

            case Keys.Space: HandleJumpButton(e); break;

            case Keys.S: HandleGoThroughButton(e); break;

            case Keys.F: HandleClimb(e); break;

            default: break;
        }
    }

    private void HandleClimb(GameKeyboardEvent e)
    {
        e.IsHandled = true;
        ChooseNextStateByEvent(e, PreviousState, ActorState.LadderClimbing);
    }

    private void HandleFrontMovementButton(GameKeyboardEvent e)
    {
        e.IsHandled = true;
        ChooseNextStateByEvent(e, PreviousState, ActorState.Moving);
        if (e.EventType == GameUserEventType.KeyDown) ChangeLookDirection(LookDirection.Back);
        else if (Direction == LookDirection.Back) ChangeLookDirection(PreviousDirection);
    }
    private void HandleJumpButton(GameKeyboardEvent e)
    {
        e.IsHandled = true;

        if (e.EventType == GameUserEventType.KeyDown) return;
        ChangeState(ActorState.Jumping);
        Jump();
    }
    private void ChooseNextStateByEvent(GameKeyboardEvent e, ActorState stateOnKeyUp, ActorState stateOnKeyDown)
    {
        e.IsHandled = true;

        ChangeState(e.EventType == GameUserEventType.KeyUp ? stateOnKeyUp : stateOnKeyDown);
    }
    private void HandleGoThroughButton(GameKeyboardEvent e)
    {
        e.IsHandled = true;
        IsGoingThrough = true;
        ChooseNextStateByEvent(e, CurrentState, ActorState.Falling);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);
    }

    internal void SetJumpSpeed(float speed)
    {
        JumpSpeed = speed;
    }
}
