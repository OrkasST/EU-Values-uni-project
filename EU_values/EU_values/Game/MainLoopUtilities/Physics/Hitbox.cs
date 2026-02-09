namespace EU_values.Game.MainLoopUtilities.Physics;

public class Hitbox
{
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public string Id { get; private set; }

    public float PreviousX { get; private set; }
    public float PreviousY { get; private set; }

    public float ModifierX { get; private set; }
    public float ModifierY { get; private set; }

    public bool IsCollidable { get; set; } = true;

    public Hitbox(float x, float y, float width, float height, string id)
    {
        X = x; Y = y; Width = width; Height = height; Id = id;
    }
    public Hitbox(Point position, Size size, string id) : this(position.X, position.Y, size.Width, size.Height, id) { }
    public Hitbox(Rectangle rect, string id) : this(rect.X, rect.Y, rect.Width, rect.Height, id) { }
    public Hitbox(float x, float y, float modifierX, float modifierY, float width, float height, string id) : this(x, y, width, height, id)
    {
        ModifierX = modifierX;
        ModifierY = modifierY;
    }
    public static Hitbox Empty => new Hitbox(0, 0, 0, 0, "");

    public void Update(float newX, float newY)
    {
        PreviousX = X;
        PreviousY = Y;

        X = newX + ModifierX;
        Y = newY + ModifierY;
    }

    public void Set(float x, float y)
    {
        X = x + ModifierX;
        Y = y + ModifierY;
        PreviousX = X;
        PreviousY = Y;
    }
}
