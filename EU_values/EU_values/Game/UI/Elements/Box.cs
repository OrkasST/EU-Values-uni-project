using EU_values.Game.BaseClasses;

namespace EU_values.Game.UI.Elements;

public class Box : DrawableObject
{
    public bool IsFilled { get; private set; } = true;
    public Box(string name, float x, float y, float width, float height, Color color, bool? isFilled = true, bool? isCameraAffected = false) : base(name, x, y, width, height, color, isCameraAffected) {
        if (isFilled != null) IsFilled = isFilled.Value;
    }
    public Box(string name, float x, float y, int width, int height, Image image, bool? isCameraAffected = false) : base(name, x, y, width, height, image, isCameraAffected) { }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }
}
