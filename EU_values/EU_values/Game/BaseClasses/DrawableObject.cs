using EU_values.Utilities;

namespace EU_values.Game.BaseClasses;

public abstract class DrawableObject
{
    public string Name { get; private set; }
    public PointF Position { get; set; } = Point.Empty;
    public SizeF Size { get; set; } = new SizeF(10, 10);

    public bool IsRotatable { get; set; } = false;
    public float Rotation { get; set; } = 0;
    public List<Vector2> PointsVectors { get; private set; } = [];
    public PointF[] RenderPoints { get; private set; } = new PointF[3];

    public Color BackgroundColor { get; set; } = Color.Red;
    public Image BackgroundImage { get; set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\No-Image.bmp");
    public Image RenderImage { get; set; } = Image.FromFile("..\\..\\..\\Resources\\Images\\No-Image.bmp");
    public bool HasImage { get; private set; } = false;
    public bool IsCameraAffected { get; set; } = false;
    public bool IsVisible { get; private set; } = true;

    private WindowPart _stickPart = WindowPart.Left_Top;
    private PointF _zeropoint = new PointF(0, 0);
    private bool _isSticked = false;

    public DrawableObject(string name, float x, float y, bool? isCameraAffected = false)
    {
        Name = name;
        Position = new PointF(x, y);
        IsCameraAffected = isCameraAffected.HasValue ? isCameraAffected.Value : false;
    }
    public DrawableObject(string name, int x, int y, bool? isCameraAffected = false)
    {
        Name = name;
        Position = new PointF(x, y);
        IsCameraAffected = isCameraAffected.HasValue ? isCameraAffected.Value : false;
    }
    public DrawableObject(string name, int x, int y, Color color, bool? isCameraAffected = false) : this(name, x, y, isCameraAffected)
    {
        BackgroundColor = color;
    }
    public DrawableObject(string name, int x, int y, int width, int height, bool? isCameraAffected = false) : this(name, x, y, isCameraAffected)
    {
        this.Size = new SizeF(width, height);
        FillPointsVectors();
    }
    public DrawableObject(string name, float x, float y, float width, float height, bool? isCameraAffected = false) : this(name, x, y, isCameraAffected)
    {
        this.Size = new SizeF(width, height);
        FillPointsVectors();
    }
    public DrawableObject(string name, int x, int y, Size size, bool? isCameraAffected = false) : this(name, x, y, isCameraAffected)
    {
        this.Size = size;
        FillPointsVectors();
    }
    public DrawableObject(string name, int x, int y, int width, int height, Image image, bool? isCameraAffected = false) : this(name, x, y, width, height, isCameraAffected)
    {
        BackgroundImage = image;
        int diagonal = (int)Math.Pow(width * width + height * height, 0.5);
        RenderImage = new Bitmap(diagonal, diagonal);
        HasImage = true;
    }
    public DrawableObject(string name, float x, float y, float width, float height, Image image, bool? isCameraAffected = false) : this(name, x, y, width, height, isCameraAffected)
    {
        BackgroundImage = image;
        int diagonal = (int)Math.Pow(width * width + height * height, 0.5);
        RenderImage = new Bitmap(diagonal, diagonal);
        HasImage = true;
    }
    public DrawableObject(string name, int x, int y, int width, int height, Color color, bool? isCameraAffected = false) : this(name, x, y, width, height, isCameraAffected)
    {
        BackgroundColor = color;
    }
    public DrawableObject(string name, float x, float y, float width, float height, Color color, bool? isCameraAffected = false) : this(name, x, y, width, height, isCameraAffected)
    {
        BackgroundColor = color;
    }

    private void FillPointsVectors()
    {
        PointsVectors.Add(new Vector2(-Size.Width / 2, -Size.Height / 2));
        PointsVectors.Add(new Vector2(Size.Width / 2, -Size.Height/2));
        PointsVectors.Add(new Vector2(-Size.Width / 2, Size.Height/2));

        ApplyRotationMatrix(0);
    }

    public void Rotate(float angle, bool setNew)
    {
        ApplyRotationMatrix(setNew ? angle - Rotation : angle);

        if (!setNew) Rotation += angle;
        else Rotation = angle;

        //if (angle >= 360) angle = 0 + angle - 360;
        //else if (angle < 0) angle = 360 + angle;
    }

    private void ApplyRotationMatrix(double angle)
    {
        angle *= Math.PI / 180;

        for (int i = 0; i < PointsVectors.Count; i++)
        {
            float length = PointsVectors[i].Length;
            PointsVectors[i].ChangeCoordinates(
                (float)( PointsVectors[i].X * Math.Cos(angle) - PointsVectors[i].Y * Math.Sin(angle) ),
                (float)( PointsVectors[i].X * Math.Sin(angle) + PointsVectors[i].Y * Math.Cos(angle) )
                );

            RenderPoints[i].X = PointsVectors[i].X + Position.X + Size.Width / 2;
            RenderPoints[i].Y = PointsVectors[i].Y + Position.Y + Size.Height / 2;
        }
    }

    public abstract void Update(float timeDelta, int timeRemaining, int timeDifference);

    public virtual bool ToggleVisibility() => IsVisible = !IsVisible;
    public virtual bool ToggleVisibility(bool visibility) => IsVisible = visibility;

    public virtual void StickToWindowPart(WindowPart part) {
        _stickPart = part;
        _isSticked = true;
        OnScreenResize();
    }
    public virtual void OnScreenResize()
    {
        if (!_isSticked) return;

        switch(_stickPart)
        {
            case WindowPart.Left_Top: break;
            case WindowPart.Left_Center: Position = new PointF(Position.X, UIPositioner.CenterY + Position.Y - _zeropoint.Y);
                _zeropoint = new PointF(0, UIPositioner.CenterY);
                break;
            case WindowPart.Left_Bottom: Position = new PointF(Position.X, UIPositioner.FormHeight + Position.Y - _zeropoint.Y);
                _zeropoint = new PointF(0, UIPositioner.FormHeight);
                break;

            case WindowPart.Right_Top: Position = new PointF(UIPositioner.FormWidth + Position.X - _zeropoint.X, Position.Y);
                _zeropoint = new PointF(UIPositioner.FormWidth, 0);
                break;
            case WindowPart.Right_Center: Position = new PointF(UIPositioner.FormWidth + Position.X - _zeropoint.X, UIPositioner.CenterY + Position.Y - _zeropoint.Y);
                _zeropoint = new PointF(UIPositioner.FormWidth, UIPositioner.CenterY);
                break;
            case WindowPart.Right_Bottom: Position = new PointF(UIPositioner.FormWidth + Position.X - _zeropoint.X, UIPositioner.FormHeight + Position.Y - _zeropoint.Y);
                _zeropoint = new PointF(UIPositioner.FormWidth, UIPositioner.FormHeight);
                break;

            case WindowPart.Center: Position = new PointF(UIPositioner.CenterX + Position.X - _zeropoint.X, UIPositioner.CenterY + Position.Y - _zeropoint.Y);
                _zeropoint = new PointF(UIPositioner.CenterX, UIPositioner.CenterY);
                break;
            default: break;
        }
    }

    public virtual void ChangeBackground(Color color)
    {
        if (HasImage) HasImage = false;
        BackgroundColor = color;
    }

    public virtual void ChangeBackground(Image image)
    {
        if (!HasImage) HasImage = true;
        BackgroundImage = image;
    }

    public virtual void ChangeCameraRelation(bool isAffected)
    {
        IsCameraAffected = isAffected;
    }
}
