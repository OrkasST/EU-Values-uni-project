using EU_values.Game.BaseClasses;

namespace EU_values.Game.UI.Elements;

public class Box : DrawableObject
{
    public Box(string name, int x, int y, int width, int height, Color color) : base(name, x, y, width, height, color) { }

    public override void Update(int timeDelta, int timeRemaining) { }
}
