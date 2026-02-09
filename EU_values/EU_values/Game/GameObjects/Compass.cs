using EU_values.Game.BaseClasses;
using EU_values.Game.UI.Elements;
using System.Resources;

namespace EU_values.Game.GameObjects;

public class Compass: ComplexDrawableObject
{
    private Box _arrow;
    private Box _case;

    public float ArrowRotation { get => _arrow.Rotation; }

    private Image _normalArrow = Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_2\\Compass_arrow.png");
    private Image _questionArrow = Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_2\\Arrow_not_working.png");

    public Compass(float x, float y): base("Compass", x, y, width: 192, height: 192)
    {
        _case = new("Compas_case", x, y, width: 192, height: 192, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_2\\Compass_resized.png"));
        _arrow = new("Compas_arrow", x, y, width: 192, height: 192, _questionArrow);

        _arrow.IsRotatable = true;

        RenderList.AddObject(0, _case);
        RenderList.AddObject(0, _arrow);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }

    public void RotateArrow() => _arrow.Rotate(-1, false);

    public void SwitchToNormalArrow() => _arrow.ChangeBackground(_normalArrow);
    public void SwitchToQuestionArrow() => _arrow.ChangeBackground(_questionArrow);

    public void ChangePosition(float x, float y)
    {
        _case.Position = new(x, y);
        _arrow.Position = new(x, y);
        _arrow.Rotate(0, true);
        Position = new(x, y);
    }
}
