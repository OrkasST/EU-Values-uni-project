namespace EU_values.Game.BaseClasses;

public abstract class DrawableObject
{
    public string Name { get; private set; }
    public Point Position { get; set; } = Point.Empty;
    public Size Size { get; set; } = new Size(10, 10);

    public Color BackgroundColor { get; set; } = Color.Red;
    public Image BackgroundImage { get; set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\No-Image.bmp");
    public bool HasImage { get; private set; } = false;
    public bool IsCameraAffected { get; private set; }

    public DrawableObject(string name, int x, int y, bool? isCameraAffected = false) {
        Name = name;
        Position = new Point(x, y);
        IsCameraAffected = isCameraAffected.HasValue;
    }
    public DrawableObject(string name, int x, int y, Color color, bool? isCameraAffected = false) : this (name, x, y, isCameraAffected) {
        BackgroundColor = color;
    }
    public DrawableObject(string name, int x, int y, int width, int height, bool? isCameraAffected = false) : this(name, x, y, isCameraAffected) {
        this.Size = new Size(width, height);
    }
    public DrawableObject(string name, int x, int y, Size size, bool? isCameraAffected = false) : this(name, x, y, isCameraAffected)
    {
        this.Size = size;
    }
    public DrawableObject(string name, int x, int y, int width, int height, Image image, bool? isCameraAffected = false) : this(name, x, y, width, height, isCameraAffected) {
        BackgroundImage = image;
        HasImage = true;
    }
    public DrawableObject(string name, int x, int y, int width, int height, Color color, bool? isCameraAffected = false) : this(name, x, y, width, height, isCameraAffected)
    {
        BackgroundColor = color;
    }

    public abstract void Update(int timeDelta, int timeRemaining);
}
