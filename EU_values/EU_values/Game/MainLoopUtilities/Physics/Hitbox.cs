namespace EU_values.Game.MainLoopUtilities.Physics;

public class Hitbox
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string Id { get; private set; }

    public Hitbox (int x, int y, int width, int height, string id)
    {
        X = x; Y = y; Width = width; Height = height; Id = id;
    }
    public Hitbox (Point position, Size size, string id) : this(position.X, position.Y, size.Width, size.Height, id) { }
    public Hitbox (Rectangle rect, string id) : this (rect.X, rect.Y, rect.Width, rect.Height, id) { }
    public static Hitbox Empty => new Hitbox(0,0,0,0,"");
}
