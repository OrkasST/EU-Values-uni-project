using EU_values.Game.BaseClasses;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.SceneDataUtils;

namespace EU_values.Game.GameObjects;

public enum AnimationType { LeftIdle, RightIdle, LeftWalk, RightWalk, BackWalk, LeftJump, RightJump, LeftFalling, RightFalling, LeftLanding, RightLanding, LeftLadderClimbing, RightLadderClimbing }
public enum AnimationDirection { Left = -1, Right = 1, Back }

public enum ActorState { Standing, Moving, Jumping, Falling, Landing, LadderClimbing }
public enum LookDirection { Left = -1, Right = 1, Back = 0, None=2 }

public abstract class Actor
{
    #region States Declaration

    public AnimationType CurrentAnimation { get; private set; }
    public AnimationType PreviousAnimation { get; private set; }

    public ActorState CurrentState { get; private set; }
    public ActorState PreviousState { get; private set; }
    public LookDirection Direction { get; private set; }
    public LookDirection PreviousDirection { get; private set; }
    #endregion


    #region Parameters Declaration

    public PointF Speed { get; protected set; }
    public PointF CurrentSpeed { get; protected set; }
    public float JumpSpeed { get; protected set; }
    public float _currentJumpSpeed; /// set to private

    protected Dictionary<AnimationType, AnimationParametersContainer> AnimationParameters;
    #endregion

    public AnimatedObject Body { get; private set; }
    public Hitbox Hitbox { get; private set; }

    #region Flags Declaration

    public bool IsCollidable { get => Hitbox.IsCollidable; set => Hitbox.IsCollidable = value; }
    public bool IsInFrontOfDoor { get; private set; } = false;
    public bool _isOnTheDoorWay = false;
    public bool IsJumping { get; protected set; } = false;
    protected bool _isStateChanged;
    public bool _isInAir = false;

    public bool IsCameraAffected { get => Body.IsCameraAffected; set => Body.IsCameraAffected = value; }
    #endregion

    public Actor(string name, float x, float y, AnimatedObject frameset, PointF speed, float jumpSpeed,
        float hitboxOffsetX, float hitboxOffsetY, float hitboxWidth, float hitboxHeight)
    {
        Body = frameset;
        this.Hitbox = new Hitbox(Body.Position.X, Body.Position.Y, hitboxOffsetX, hitboxOffsetY, hitboxWidth, hitboxHeight, $"{name}_Hitbox");

        CurrentAnimation = AnimationType.LeftIdle;
        PreviousAnimation = AnimationType.LeftIdle;

        Speed = speed;
        CurrentSpeed = new PointF(0, 0);
        JumpSpeed = jumpSpeed;

        CurrentState = ActorState.Standing;
        PreviousState = ActorState.Standing;

        Direction = LookDirection.Left;
        PreviousDirection = LookDirection.Left;

        AnimationParameters = new() { };
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
    public void SetCurrentSpeed(float x, float y)
    {
        CurrentSpeed = new PointF(x, y);
    }

    protected void ChangeState(ActorState nextState)
    {
        PreviousState = CurrentState;
        CurrentState = nextState;
        _isStateChanged = true;

        if (CurrentState == ActorState.Moving && CurrentSpeed.X == 0) CurrentSpeed = Speed;
        if (CurrentState == ActorState.Jumping && PreviousState == ActorState.Moving) CurrentSpeed = Speed;
        if (CurrentState == ActorState.Standing) CurrentSpeed = new PointF(0, 0);
        if (CurrentState == ActorState.Falling && !_isInAir) _isInAir = true;
    }
    protected void ResetStateChange() => _isStateChanged = false;
    protected void ChangeLookDirection(LookDirection direction)
    {
        PreviousDirection = Direction;
        Direction = direction;
    }
    protected void ChangeAnimationType(AnimationType nextAnimation)
    {
        PreviousAnimation = CurrentAnimation;
        CurrentAnimation = nextAnimation;
    }

    public virtual void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        if (_isStateChanged)
        {
            Body.StopAnimation();
            AdjustAnimationToState();
            _isStateChanged = false;
        }

        if (!Body.IsAnimationStarted) Body.StartAnimation(AnimationParameters[CurrentAnimation], timeRemaining);

        if (_isInAir)
        {
            SetPosition(new PointF(Body.Position.X + ((int)Direction * CurrentSpeed.X * 1.5f * timeDelta), Body.Position.Y + ((IsJumping ? -1 : 1) * (_currentJumpSpeed * timeDelta))));
            var jumpSpeedModifier = (IsJumping ? -1 : 1.6f) * 450f * timeDelta;
            _currentJumpSpeed = _currentJumpSpeed + jumpSpeedModifier;

            if (CurrentState == ActorState.Jumping && _currentJumpSpeed <= 0)
            {
                IsJumping = false;
                ChangeState(ActorState.Falling);
            }

        }
        else
        {
            if (CurrentState == ActorState.Moving)
            {
                SetPosition(new PointF(((int)Direction * CurrentSpeed.X * timeDelta) + Body.Position.X, Body.Position.Y));
            }
            if (IsInFrontOfDoor && Direction == LookDirection.Back)
            {
                _isOnTheDoorWay = true;
                SetPosition(new PointF(Body.Position.X, Body.Position.Y - (Speed.Y * timeDelta)));
            }
            else if (IsInFrontOfDoor) _isOnTheDoorWay = false;
        }

        if (Body.CurrentFrame >= Body.CurrentLastFrame)
        {
            if (CurrentState == ActorState.Landing) ChangeState(ActorState.Standing);
        }
    }

    public void OnGroundHit()
    {
        ChangeState(ActorState.Landing);
        _isInAir = false;
    }
    public void OnGroundLeave() { if (!IsJumping) ChangeState(ActorState.Falling); }

    public void OnLevelBorderHit()
    {
        CurrentSpeed = new PointF(0, Speed.Y);
    }

    protected void AdjustAnimationToState()
    {
        switch (CurrentState)
        {
            case ActorState.Standing:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftIdle);
                else ChangeAnimationType(AnimationType.RightIdle); break;

            case ActorState.Moving:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftWalk);
                else if (Direction == LookDirection.Back) ChangeAnimationType(AnimationType.BackWalk);
                else ChangeAnimationType(AnimationType.RightWalk); break;

            case ActorState.Jumping:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftJump);
                else ChangeAnimationType(AnimationType.RightJump); break;

            case ActorState.Falling:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftFalling);
                else ChangeAnimationType(AnimationType.RightFalling); break;

            case ActorState.Landing:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftLanding);
                else ChangeAnimationType(AnimationType.RightLanding); break;

            case ActorState.LadderClimbing:
                if (Direction == LookDirection.Left) ChangeAnimationType(AnimationType.LeftLadderClimbing);
                else ChangeAnimationType(AnimationType.RightLadderClimbing); break;

            default: break;
        }
    }

    public void Jump()
    {
        if (Direction == LookDirection.Back) ChangeLookDirection(PreviousDirection);
        _isInAir = true;
        IsJumping = true;
        _currentJumpSpeed = JumpSpeed;
    }

    public void OnDoorWayEnter() => IsInFrontOfDoor = true;
    public void OnDoorWayLeave() => IsInFrontOfDoor = false;

    //public void OnDoorEnterStart() => IsEnteringTheDoor = true;
    public void OnDoorEnterFail() {
        ChangeLookDirection(PreviousDirection);
        ChangeState(ActorState.Standing);
    }

    public void ChangeCameraRelation(bool isAffected) => IsCameraAffected = isAffected;
}
