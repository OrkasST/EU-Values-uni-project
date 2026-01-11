using EU_values.Game.BaseClasses;

namespace EU_values.Game.UI.Elements;

public class Box : DrawableObject
{
    public Box(string name, int x, int y, int width, int height, Color color, bool? isCameraAffected = false) : base(name, x, y, width, height, color, isCameraAffected) { }
    public Box(string name, int x, int y, int width, int height, Image image, bool? isCameraAffected = false) : base(name, x, y, width, height, image, isCameraAffected) { }

    public override void Update(int timeDelta, int timeRemaining) { }
}
