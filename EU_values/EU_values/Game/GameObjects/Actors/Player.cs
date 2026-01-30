using EU_values.Game.BaseClasses;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.GameObjects.Actors;

public enum AnimationType { LeftIdle, RightIdle, LeftWalk, RightWalk, BackWalk, LeftJump, RightJump, LeftFalling, RightFalling, LeftLanding, RightLanding }
enum AnimationDirection { Left = -1, Right = 1, Back }

public enum PlayerState { Standing, Moving, Jumping, Falling, Landing }
public enum LookDirection { Left=-1, Right=1, Back=0 }

public class Player
{
    public AnimationType CurrentAnimation { get; private set; }
    public AnimationType PreviousAnimation { get; private set; }

    public bool _isInAir = false;
    public bool IsInFrontOfDoor { get; private set; } = false;
    private bool _isOnTheDoorWay = false;
    public bool IsEnteringTheDoor { get; private set; } = false;

    public bool IsJumping { get; private set; } = false;

    public PlayerState CurrentState { get; private set; }
    public PlayerState PreviousState { get; private set; }
    public LookDirection Direction { get; private set; }
    public LookDirection PreviousDirection { get; private set; }

    public float JumpSpeed { get; private set; }
    private float _currentJumpSpeed;

    private bool _isStateChanged;

    public PointF Speed { get; private set; }

    public AnimatedObject Body { get; private set; }
    public Hitbox Hitbox { get; private set; }
    public Player(string name, float x, float y)
    {
        Body = new (name, x, y, 128, 128, Image.FromFile("..\\..\\..\\Resources\\Images\\PlayerSpritesheet\\PlayerSpriteSheet.png"), 4, 11);
        this.Hitbox = new Hitbox(Body.Position.X, Body.Position.Y, 46, 0, 34, 128, "PlayerHitbox");

        CurrentAnimation = AnimationType.LeftIdle;
        PreviousAnimation = AnimationType.LeftIdle;

        Speed = new (90, 50);
        JumpSpeed = 450;

        CurrentState = PlayerState.Standing;
        PreviousState = PlayerState.Standing;

        Direction = LookDirection.Left;
        PreviousDirection = LookDirection.Left;
    }

    public void HandleUserInput(GameKeyboardEvent e)
    {
        if (InputHandler.NoKeyboardEvents) return;
        if (e.IsHandled) return;
        if (_isInAir || _isOnTheDoorWay) return;

        switch (e.Key)
        {
            case Keys.D: ChooseNextStateByEvent(e, PlayerState.Standing, PlayerState.Moving);
                         ChangeLookDirection(LookDirection.Right); break;

            case Keys.A: ChooseNextStateByEvent(e, PlayerState.Standing, PlayerState.Moving);
                         ChangeLookDirection(LookDirection.Left); break;

            case Keys.W: HandleFrontMovementButton(e); break;

            case Keys.Space: HandleJumpButton(e); break;

            default: break;
        }
    }

    private void HandleFrontMovementButton(GameKeyboardEvent e)
    {
        ChooseNextStateByEvent(e, PreviousState, PlayerState.Moving);
        if (e.EventType == GameUserEventType.KeyDown) ChangeLookDirection(LookDirection.Back);
        else ChangeLookDirection(PreviousDirection);
    }
    private void HandleJumpButton(GameKeyboardEvent e)
    {
        e.IsHandled = true;
        if (e.EventType == GameUserEventType.KeyDown) return;
        ChangeState(PlayerState.Jumping);
        if (Direction == LookDirection.Back) ChangeLookDirection(PreviousDirection);
        _isInAir = true;
        Jump();
    }

    public void SetPosition(PointF point)
    {
        this.Body.Position = point;
        this.Hitbox.Update(this.Body.Position.X, this.Body.Position.Y);
    }
    public void SetPosition(float x, float y) => SetPosition(new PointF(x, y));
    public void SetPositionByCenter(float x, float y)
    {
        SetPosition(new PointF(x - Body.Size.Width / 2, y - Body.Size.Height / 2));
        this.Hitbox.Set(Body.Position.X, Body.Position.Y);
    }

    private void ChooseNextStateByEvent(GameKeyboardEvent e, PlayerState stateOnKeyUp, PlayerState stateOnKeyDown)
    {
        e.IsHandled = true;
        ChangeState(e.EventType == GameUserEventType.KeyUp ? stateOnKeyUp : stateOnKeyDown);
    }

    private void ChangeState(PlayerState nextState)
    {
        PreviousState = CurrentState;
        CurrentState = nextState;
        _isStateChanged = true;
    }
    private void ChangeLookDirection(LookDirection direction)
    {
        PreviousDirection = Direction;
        Direction = direction;
    }
    private void ChangeAnimationType(AnimationType nextAnimation)
    {
        PreviousAnimation = CurrentAnimation;
        CurrentAnimation = nextAnimation;
    }

    public void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        if (_isStateChanged)
        {
            Body.StopAnimation();
            AdjustAnimationToState();
            _isStateChanged = false;
        }

        if (!Body.IsAnimationStarted)
        {
            Body.StartAnimation(100, 0, 1 * (int)CurrentAnimation, (int)CurrentAnimation <= 4 ? 3 : (int)CurrentAnimation <= 6 ? 2 : 1, (int)CurrentAnimation,
                !_isInAir, timeRemaining);
        }

        if (_isInAir)
        {
            SetPosition(new PointF(Body.Position.X + ((int)Direction * Speed.X * 1.5f * timeDelta), Body.Position.Y + ((IsJumping ? -1 : 1) * (_currentJumpSpeed*timeDelta))));
            _currentJumpSpeed = _currentJumpSpeed + (IsJumping ? -1 : 1.3f) * 10f;

            if (CurrentState == PlayerState.Jumping && _currentJumpSpeed == 0)
            {
                IsJumping = false;
                ChangeState(PlayerState.Falling);
            }

        } else
        {
            if (CurrentState == PlayerState.Moving)
            {
                var speed = Speed.X * timeDelta;
                var speedDir = (int)Direction * Speed.X * timeDelta;

                SetPosition(new PointF( ((int)Direction * Speed.X * timeDelta) + Body.Position.X, Body.Position.Y));
            }
            if (IsInFrontOfDoor && Direction == LookDirection.Back)
            {
                _isOnTheDoorWay = true;
                SetPosition(new PointF(Body.Position.X, Body.Position.Y - (Speed.Y * timeDelta) ));
            }
        }

        if (Body.CurrentFrame >= Body.CurrentLastFrame)
        {
            if (CurrentState == PlayerState.Landing) ChangeState(PlayerState.Standing);
        }
    }

    public void OnGroundHit()
    {
        ChangeState(PlayerState.Landing);
        _isInAir = false;
    }
    public void OnGroundLeave()
    {
        ChangeState(PlayerState.Falling);
        _isInAir = true;
    }

    public void OnLevelBorderHit()
    {
        ChangeState(PlayerState.Standing);
    }

    private void AdjustAnimationToState()
    {
        switch (CurrentState)
        {
            case PlayerState.Standing: if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftIdle);
                else ChangeAnimationType(AnimationType.RightIdle); break;

            case PlayerState.Moving:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftWalk);
                else if (Direction == LookDirection.Back) ChangeAnimationType(AnimationType.BackWalk);
                else ChangeAnimationType(AnimationType.RightWalk); break;

            case PlayerState.Jumping:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftJump);
                else ChangeAnimationType(AnimationType.RightJump); break;

            case PlayerState.Falling:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftFalling);
                else ChangeAnimationType(AnimationType.RightFalling); break;

            case PlayerState.Landing:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftLanding);
                else ChangeAnimationType(AnimationType.RightLanding); break;

            default: break;
        }
    }

    public void Jump()
    {
        IsJumping = true;
        _currentJumpSpeed = JumpSpeed;
    }

    public void OnDoorWayEnter() => IsInFrontOfDoor = true;
    public void OnDoorWayLeave() => IsInFrontOfDoor = false;
    public void OnDoorEnterStart() => IsEnteringTheDoor = true;
}
