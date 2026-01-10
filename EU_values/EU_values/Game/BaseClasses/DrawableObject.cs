namespace EU_values.Game.BaseClasses;

public abstract class DrawableObject
{
    public string Name { get; private set; }
    public Point Position { get; set; } = Point.Empty;
    public Size Size { get; set; } = new Size(10, 10);

    public Color BackgroundColor { get; set; } = Color.Red;
    public Image BackgroundImage { get; private set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\No-Image.bmp");
    public bool HasImage { get; private set; } = false;

    public DrawableObject(string name, int x, int y) {
        Name = name;
        Position = new Point(x, y);
    }
    public DrawableObject(string name, int x, int y, Color color) : this (name, x, y) {
        BackgroundColor = color;
    }
    public DrawableObject(string name, int x, int y, int width, int height) : this(name, x, y) {
        this.Size = new Size(width, height);
    }
    public DrawableObject(string name, int x, int y, Size size) : this(name, x, y)
    {
        this.Size = size;
    }
    public DrawableObject(string name, int x, int y, int width, int height, Image image) : this(name, x, y, width, height) {
        BackgroundImage = image;
        HasImage = true;
    }
    public DrawableObject(string name, int x, int y, int width, int height, Color color) : this(name, x, y, width, height)
    {
        BackgroundColor = color;
    }

    public abstract void Update(int timeDelta, int timeRemaining);
}
