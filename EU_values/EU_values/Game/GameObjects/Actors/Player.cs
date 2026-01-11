using EU_values.Game.BaseClasses;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.GameObjects.Actors;

public enum AnimationType { LeftIdle, RightIdle, LeftWalk, RightWalk, BackWalk, LeftJump, RightJump, LeftFalling, RightFalling, LeftLanding, RightLanding }
enum AnimationDirection { Left, Right, Back }

public class Player
{
    public AnimationType CurrentAnimation { get; private set; }
    public AnimationType PreviousAnimation { get; private set; }

    private bool _isInAir = false;
    public bool IsInFrontOfDoor { get; private set; } = false;
    private bool _isOnTheDoorWay = false;
    public bool IsEnteringTheDoor { get; private set; } = false;

    public Point Speed { get; private set; }

    public AnimatedObject Body { get; private set; }
    public Player(string name, int x, int y)
    {   
        Body = new AnimatedObject(name, x, y, 128, 128, Image.FromFile("..\\..\\..\\Resources\\Images\\PlayerSpritesheet\\PlayerSpriteSheet.png"), 4, 11);
        CurrentAnimation = AnimationType.LeftIdle;
        PreviousAnimation = AnimationType.LeftIdle;
        Speed = new Point(2, 1);
    }

    public void HandleUserInput()
    {
        if (InputHandler.NoKeyboardEvents) return;
        if (InputHandler.LastKeyboardEvent.IsHandled) return;
        if (_isInAir || _isOnTheDoorWay) return;

        var nextAnimation = CurrentAnimation;
        switch (InputHandler.LastKeyboardEvent.Key)
        {
            case Keys.D: nextAnimation = GetNextAnimation(InputHandler.LastKeyboardEvent, AnimationType.RightIdle, AnimationType.RightWalk); break;
            case Keys.A: nextAnimation = GetNextAnimation(InputHandler.LastKeyboardEvent, AnimationType.LeftIdle, AnimationType.LeftWalk); break;
            case Keys.W: nextAnimation = GetNextAnimation(InputHandler.LastKeyboardEvent, PreviousAnimation, AnimationType.BackWalk); break;
            case Keys.Space: nextAnimation = GetNextAnimation(InputHandler.LastKeyboardEvent, GetAnimationDirection() == AnimationDirection.Left ?
                AnimationType.LeftJump : AnimationType.RightJump, CurrentAnimation);
                _isInAir = ( nextAnimation == AnimationType.RightJump || nextAnimation == AnimationType.LeftJump ); break;

            default: break;
        }
        if (CurrentAnimation != nextAnimation)
        {
            PreviousAnimation = CurrentAnimation;
            CurrentAnimation = nextAnimation;
            Body.StopAnimation();
        }
    }

    private AnimationType GetNextAnimation(GameKeyboardEvent e, AnimationType anim1, AnimationType anim2)
    {
        e.IsHandled = true;
        return e.EventType == GameUserEventType.KeyUp ? anim1 : anim2;
    }
    private AnimationDirection GetAnimationDirection()
    {
        if (CurrentAnimation == AnimationType.LeftIdle || CurrentAnimation == AnimationType.LeftWalk
            || CurrentAnimation == AnimationType.LeftJump || CurrentAnimation == AnimationType.LeftFalling
            || CurrentAnimation == AnimationType.LeftLanding) return AnimationDirection.Left;
        if (CurrentAnimation == AnimationType.RightIdle || CurrentAnimation == AnimationType.RightWalk
            || CurrentAnimation == AnimationType.RightJump || CurrentAnimation == AnimationType.RightFalling
            || CurrentAnimation == AnimationType.RightLanding) return AnimationDirection.Right;
        return AnimationDirection.Back;
    }

    private bool IsAnimationAirborne()
    {
        return CurrentAnimation == AnimationType.LeftJump || CurrentAnimation == AnimationType.RightJump ||
            CurrentAnimation == AnimationType.LeftLanding || CurrentAnimation == AnimationType.RightLanding ||
            CurrentAnimation == AnimationType.LeftFalling || CurrentAnimation == AnimationType.RightFalling;
    }

    public void Update(int timeDelta, int timeRemaining)
    {
        if (!Body.IsAnimationStarted)
        {
            Body.StartAnimation(8000, 0, 1*(int)CurrentAnimation, (int)CurrentAnimation <= 4 ? 3 : (int)CurrentAnimation <= 6 ? 2 : 1, (int)CurrentAnimation,
                !IsAnimationAirborne(), timeRemaining);
        } else
        {
            if (_isInAir && Body.CurrentFrame == Body.CurrentLastFrame)// && Body.IsReadyToSwitchFrame(timeRemaining))
            {
                switch (CurrentAnimation)
                {
                    case AnimationType.LeftJump: CurrentAnimation = AnimationType.LeftFalling; Body.StopAnimation(); break;
                    case AnimationType.RightJump: CurrentAnimation = AnimationType.RightFalling; Body.StopAnimation(); break;

                    case AnimationType.LeftFalling: CurrentAnimation = AnimationType.LeftLanding; Body.StopAnimation(); break;
                    case AnimationType.RightFalling: CurrentAnimation = AnimationType.RightLanding; Body.StopAnimation(); break;

                    case AnimationType.LeftLanding: CurrentAnimation = AnimationType.LeftIdle; _isInAir = false; Body.StopAnimation(); break;
                    case AnimationType.RightLanding: CurrentAnimation = AnimationType.RightIdle; _isInAir = false; Body.StopAnimation(); break;
                    default: break;
                }
            }

            if (CurrentAnimation == AnimationType.LeftWalk) Body.Position = new Point(Body.Position.X - Speed.X, Body.Position.Y);
            if (CurrentAnimation == AnimationType.RightWalk) Body.Position = new Point(Body.Position.X + Speed.X, Body.Position.Y);
            if (IsInFrontOfDoor && CurrentAnimation == AnimationType.BackWalk)
            {
                _isOnTheDoorWay = true;
                Body.Position = new Point(Body.Position.X, Body.Position.Y - Speed.Y);
            }
        } 
    }

    public void OnDoorWayEnter() => IsInFrontOfDoor = true;
    public void OnDoorWayLeave() => IsInFrontOfDoor = false;
    public void OnDoorEnterStart() => IsEnteringTheDoor = true;
}
