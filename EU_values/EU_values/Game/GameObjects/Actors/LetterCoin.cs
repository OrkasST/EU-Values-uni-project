using EU_values.Game.Interfaces;
using EU_values.Utilities;

namespace EU_values.Game.GameObjects.Actors;

public class LetterCoin : Actor, IStorable
{
    private readonly static string _framesetLocation = "..\\..\\..\\Resources\\Images\\Levels\\Level_3\\coin";
    public int _jumpsCount;

    private float _jumpTimerZeroPoint = -1;
    private int _timeToFirstJump;
    private int _timeToJump;

    private PointF _destinationPoint = PointF.Empty;
    private bool _isTripStarted = false;
    private float _tripDistance = 0;
    private float _doneDistance = 0;
    private PointF _tripStart = PointF.Empty;

    public bool IsQuestioned { get; set; } = false;

    public bool IsAtFinalLocation { get; private set; } = false;

    public string Description { get; private set; }
    public string Name { get; private set; }

    public LetterCoin(string name, string letter, float x, float y, int timeToFirstJump) : base(name, x, y,
        frameset: new(name, x, y, width: 64, height: 64, Image.FromFile($"{_framesetLocation}{letter}.png"), xFramesNumber: 1, yFramesNumber: 1, isCameraAffected: true),
        speed: PointF.Empty, jumpSpeed: 0,
        hitboxOffsetX: 0, hitboxOffsetY: 0, hitboxWidth: 64, hitboxHeight: 64)
    {
        AnimationParameters = new()
        {
            [AnimationType.LeftIdle] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.RightIdle] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.LeftWalk] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.RightWalk] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.LeftJump] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.RightJump] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.LeftFalling] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.RightFalling] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.LeftLanding] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
            [AnimationType.RightLanding] = new(frameTimeGap: 100, startXFrame: 0, startYFrame: 0, endXFrame: 0, endYFrame: 0, isInfinite: true),
        };

        Name = "Strange Coin";
        Description = $"A coin with a letter \"{letter}\" on it";

        ChangeState(ActorState.Jumping);
        _jumpsCount = 2;
        _timeToFirstJump = timeToFirstJump;
        RandomizeMovement();
        _timeToJump = _timeToFirstJump;
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        if (!_isTripStarted) base.Update(timeDelta, timeRemaining, timeDifference);

        if (_jumpTimerZeroPoint < 0) _jumpTimerZeroPoint = timeRemaining;
        if (_jumpsCount > 0 && timeRemaining - _jumpTimerZeroPoint > _timeToJump)
        {
            _jumpTimerZeroPoint = timeRemaining;
            _jumpsCount--;
            RandomizeMovement();
            Jump();
            ResetStateChange();
        }

        if (CurrentState == ActorState.Falling && _jumpsCount == 0)
        {
            PointF distance = new PointF(_destinationPoint.X - (Body.Position.X + Body.Size.Width / 2), _destinationPoint.Y - (Body.Position.Y + Body.Size.Height / 2));

            float xSpeed = JumpSpeed * (distance.X < 0 ? -1 : 1);
            float ySpeed = (distance.Y / distance.X) * xSpeed;

            Speed = new PointF(xSpeed, ySpeed);
            CurrentSpeed = Speed;

            OnGroundHit();
            ChangeState(ActorState.Moving);
            ResetStateChange();

            _isTripStarted = true;

            _tripDistance = (float)Math.Pow(distance.X * distance.X + distance.Y * distance.Y, 0.5);

            _tripStart = new PointF(Body.Position.X, Body.Position.Y);
        }
        if (_isTripStarted)
        {
            SetPosition(Body.Position.X + CurrentSpeed.X * timeDelta, Body.Position.Y + CurrentSpeed.Y * timeDelta);

            PointF distance = new PointF(_tripStart.X - Body.Position.X, _tripStart.Y - Body.Position.Y);
            _doneDistance = (float)Math.Pow(distance.X * distance.X + distance.Y * distance.Y, 0.5);

            if (_doneDistance >= _tripDistance)
            {
                SetPositionByCenter(_destinationPoint.X, _destinationPoint.Y);
                _isTripStarted = false;
                ChangeState(ActorState.Standing);
                ResetStateChange();
                IsAtFinalLocation = true;
            }
        }
    }

    public void SetDestinationPoint(PointF point) => _destinationPoint = point;

    private void RandomizeMovement()
    {
        JumpSpeed = GameRandom.Next(4, 9) * 100;
        Speed = new PointF(GameRandom.Next(2, 5) * 100, 0);
        CurrentSpeed = Speed;
        ChangeLookDirection(GameRandom.Next(1, 3) == 1 ? LookDirection.Left : LookDirection.Right);
        _timeToJump = GameRandom.Next(2, 6) * 200;
    }
}
