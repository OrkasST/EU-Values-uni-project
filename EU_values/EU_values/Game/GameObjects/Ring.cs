using EU_values.Game.BaseClasses;
using EU_values.Game.UI.Elements;

namespace EU_values.Game.GameObjects;

public enum RotateDirection { Clockwise, Counterclockwise }

public class Ring : ComplexDrawableObject
{
    private bool _isRotating;
    private float _angle = 60f;
    private float _currentAngle = 0f;

    private int _direction = 1;

    private static string _imagesLocation = "..\\..\\..\\Resources\\Images\\Levels\\Level_7\\";
    private static string[] _wordImageNames = { "WORTH", "DIRECTION", "EQUAL LAW", "VOICE", "BALANCE", "LIGHT" };
    private List<Box> _words = [];
    private bool[] _activatedWords;

    private int _currentSelection = 4;

    public int TotalGuessed { get; private set; } = 0;

    private Box _mainBg;

    public Ring(float x, float y) : base("Compass", x, y, width: 192, height: 192)
    {
        _mainBg = new("Ring", x, y, width: 1000, height: 1000,
            Image.FromFile($"{_imagesLocation}Ring.png"), isCameraAffected: true);
        _mainBg.IsRotatable = true;

        _activatedWords = new bool[6];

        for (int i = 0; i < _wordImageNames.Length; i++)
        {
            _words.Add(new Box(name: _wordImageNames[i], x, y, width: 1000, height: 1000,
                image: Image.FromFile($"{_imagesLocation}{_wordImageNames[i]}.png"), isCameraAffected: true));
            _words.Last().IsRotatable = true;
        }

        RenderList.AddObject(0, _mainBg);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        if (_isRotating && _currentAngle < _angle)
        {
            _currentAngle += 1;
            _mainBg.Rotate(_direction, false);
            foreach (var word in _words) word.Rotate(_direction, false);
        }
        else if (_isRotating) _isRotating = false;
    }

    public string RotateToAnotherSection(RotateDirection direction)
    {
        if (!_isRotating)
        {
            _isRotating = true;
            _currentAngle = 0f;
            _direction = direction == RotateDirection.Clockwise ? 1 : -1;

            _currentSelection -= _direction;

            if (_currentSelection < 0) _currentSelection = _words.Count - 1;
            else if (_currentSelection > _words.Count - 1) _currentSelection = 0;
        }

        return _wordImageNames[_currentSelection];
    }

    public string GetCurrentWord() => _wordImageNames[_currentSelection];

    public void DisplayCurrent()
    {
        if (_currentSelection < 0 || _currentSelection >= _words.Count
            || _activatedWords[_currentSelection] || _isRotating) return;

        RenderList.AddObject(1, _words[_currentSelection]);
        _activatedWords[_currentSelection] = true;
        TotalGuessed++;
    }
}
