using EU_values.Game.BaseClasses;
using EU_values.Game.Interfaces;

namespace EU_values.Game.GameObjects.Actors;

public class Concept : Actor, IStorable
{
    private static string _framesetPath = "..\\..\\..\\Resources\\Images\\Levels\\Level_4\\";
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Concept(string name, string description, float x, float y, string imageName)
        : base(name, x, y,
            frameset: new(name, x, y, width: 64, height: 64, Image.FromFile(_framesetPath+imageName), xFramesNumber: 1, yFramesNumber: 1, isCameraAffected: true),
            speed: PointF.Empty, jumpSpeed: 0, hitboxOffsetX: 0, hitboxOffsetY: 0, hitboxWidth: 0, hitboxHeight: 0)
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

        Name = name;
        Description = description;
    }
}
