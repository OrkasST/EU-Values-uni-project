namespace EU_values.Utilities;

public class Vector2
{
    private float _x;
    private float _y;
    public float X { get => _x; set { _x = value; CalculateLength(); } }
    public float Y { get => _y; set { _y = value; CalculateLength(); } }

    public float Length { get; private set; }

    public Vector2 (float x, float y)
    {
        _x = x;
        _y = y;
        CalculateLength();
    }

    public void ChangeCoordinates(float x, float y)
    {
        _x = x;
        _y = y;
        CalculateLength();
    }

    private void CalculateLength() { Length = (float)Math.Pow(_x * _x + _y * _y, 0.5); }
}
