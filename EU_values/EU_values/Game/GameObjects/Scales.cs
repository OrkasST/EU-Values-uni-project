using EU_values.Game.BaseClasses;
using EU_values.Game.Interfaces;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;

namespace EU_values.Game.GameObjects;

public enum Cups { None, Left, Middle, Right }

public class Scales : ComplexDrawableObject
{
    private Box _rack;
    private Box _leftCup;
    private Box _rightCup;
    private Box _topCup;
    public Box _crossbar;

    public PointF LeftCupCenterPosition { get; private set; }
    public PointF RightCupCenterPosition { get; private set; }
    public PointF MiddleCupCenterPosition { get; private set; }

    private static float _balanceYCupsPosition = 63;

    public int ItemsRight { get; private set; } = 0;
    public int ItemsLeft { get; private set; } = 0;
    public int ItemsMiddle { get; private set; } = 0;

    private int _rotationDirection = 0;

    public Hitbox LeftCupHitbox;
    public Hitbox RightCupHitbox;
    public Hitbox MiddleCupHitbox;

    public Cups SelectedCup = Cups.None;

    private float _rotationSpeed = 0;

    public List<IStorable> LeftCupInventory = [];
    public List<IStorable> RightCupInventory = [];
    public List<IStorable> MiddleCupInventory = [];

    private string _resouseLockation = "..\\..\\..\\Resources\\Images\\Levels\\Level_4\\";
    public Scales(int x, int y, int width, int height) : base(name: "Level_4_scales", x, y, width, height, isCameraAffected: true)
    {
        _rack =new(name: "Scales_Rack", x, y, width, height, Image.FromFile($"{_resouseLockation}Scales_stand.png"), IsCameraAffected);
        _leftCup = new(name: "Scales_Rack", x, y, width, height, Image.FromFile($"{_resouseLockation}Scale_left_cup.png"), IsCameraAffected);
        _rightCup = new(name: "Scales_Rack", x, y, width, height, Image.FromFile($"{_resouseLockation}Scale_right_cup.png"), IsCameraAffected);
        _topCup = new(name: "Scales_Rack", x, y, width, height, Image.FromFile($"{_resouseLockation}Scale_top_cup.png"), IsCameraAffected);
        _crossbar = new(name: "Scales_Rack", x, y-_balanceYCupsPosition, width, height, Image.FromFile($"{_resouseLockation}Scales_handles.png"), IsCameraAffected);

        _crossbar.IsRotatable = true;
        _crossbar.Rotate(0, true);

        RenderList.AddObject(0, _crossbar);
        RenderList.AddObject(1, _rack);
        RenderList.AddObject(1, _leftCup);
        RenderList.AddObject(1, _rightCup);
        RenderList.AddObject(1, _topCup);

        LeftCupHitbox = new(x, y, 85, 256, "LeftCupHitbox");
        RightCupHitbox = new(x+171, y, 85, 256, "LeftCupHitbox");
        MiddleCupHitbox = new(x+85, y, 85, 256, "LeftCupHitbox");

        LeftCupCenterPosition = new PointF(Position.X + 51, Position.Y);
        RightCupCenterPosition = new PointF(Position.X + 194, Position.Y);
        MiddleCupCenterPosition = new PointF(Position.X + 123, Position.Y - 128);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        if (_rotationDirection != 0)
        {
            if (!(_crossbar.Rotation <= -50 && _rotationDirection == -1) && !(_crossbar.Rotation >= 50 && _rotationDirection == 1))
            {
                _rotationSpeed = (float)Math.Abs(ItemsLeft - ItemsRight) / (float)Math.Max(ItemsRight, ItemsLeft);
                if (_rotationSpeed == 0 || float.IsNaN(_rotationSpeed)) _rotationSpeed = 0.2f;

                _crossbar.Rotate(_rotationDirection * _rotationSpeed, false);
                if (_rotationDirection < 0)
                {
                    _leftCup.Position = new(_leftCup.Position.X, _leftCup.Position.Y + _rotationSpeed);
                    _rightCup.Position = new(_rightCup.Position.X, _rightCup.Position.Y - _rotationSpeed);
                }
                else if (_rotationDirection > 0)
                {
                    _leftCup.Position = new(_leftCup.Position.X, _leftCup.Position.Y - _rotationSpeed);
                    _rightCup.Position = new(_rightCup.Position.X, _rightCup.Position.Y + _rotationSpeed);
                }
            }
        }

        if (ItemsRight == ItemsLeft && _rotationDirection == 1 && _crossbar.Rotation >= 0)
        {
            _crossbar.Rotate(0, true);
            _rotationDirection = 0;
        }

        else if (ItemsRight == ItemsLeft && _rotationDirection == -1 && _crossbar.Rotation <= 0)
        {
            _crossbar.Rotate(0, true);
            _rotationDirection = 0;
        }
    }

    public bool LoadRightCup(IStorable? item)
    {
        if (item == null) return false;

        RightCupInventory.Add(item);

        ItemsRight++;
        if (ItemsRight > ItemsLeft) _rotationDirection = 1;
        CheckDirection();
        return true;
    }
    public bool LoadLeftCup(IStorable? item)
    {
        if (item == null) return false;

        LeftCupInventory.Add(item);

        ItemsLeft++;
        if (ItemsRight < ItemsLeft) _rotationDirection = -1;
        CheckDirection();
        return true;
    }
    public bool LoadMiddleCup(IStorable? item)
    {
        if (item == null) return false;

        MiddleCupInventory.Add(item);
        ItemsMiddle++;
        return true;
    }

    public IStorable ReleaseRightCup()
    {
        ItemsRight--;
        CheckDirection();
        var item = RightCupInventory.Last();
        RightCupInventory.RemoveAt(ItemsRight);
        return item;
    }
    public IStorable ReleaseLeftCup()
    {
        ItemsLeft--;
        CheckDirection();
        var item = LeftCupInventory.Last();
        LeftCupInventory.RemoveAt(ItemsLeft);
        return item;
    }
    public IStorable ReleaseMiddleCup()
    {
        ItemsMiddle--;
        CheckDirection();
        var item = MiddleCupInventory.Last();
        MiddleCupInventory.RemoveAt(ItemsMiddle);
        return item;
    }

    private void CheckDirection()
    {
        if (ItemsRight == ItemsLeft && _crossbar.Rotation < 0) _rotationDirection = 1;
        else if (ItemsRight == ItemsLeft && _crossbar.Rotation > 0) _rotationDirection = -1;
    }
}
