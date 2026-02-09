using EU_values.Game.BaseClasses;
using EU_values.Game.Interfaces;
using EU_values.Utilities;

namespace EU_values.Game.UI.Elements;

public enum BoxType { Default, Hover, Click }

public class GameUIButton : ComplexDrawableObject, IInteractive
{
    private Box _defaultBox;
    private int _indBox;

    private Box _hoverBox;
    private Box _clickBox;

    private GameUIText _defaultText;
    private int _indText;

    private GameUIText _hoverText;
    private GameUIText _clickText;

    public string Text
    {
        get => _defaultText.Text;
        set
        {
            _defaultText.Text = value;
            _hoverText.Text = value;
            _clickText.Text = value;
        }
    }
    public bool HasMouseOver { get; set; } = false;
    public bool IsClickStarted { get; set; } = false;

    public GameUIButton(string name, int x, int y, string text, bool? isCameraAffected = false) : base(name, x, y, 210, 60)
    {
        _defaultBox = new Box(name + "_default", x, y, 210, 60, Color.Gray);
        _hoverBox = new Box(name + "_hover", x - 1, y - 1, 210 + 2, 60 + 2, Color.Yellow);
        _clickBox = new Box(name + "_click", x, y, 210, 60, Color.Blue);

        _defaultText = new GameUIText(name + "_text", x, y, text, "Times New Roman", 55, Color.Black);
        _hoverText = new GameUIText(name + "_text", x, y, text, "Times New Roman", 55, Color.Black);
        _clickText = new GameUIText(name + "_text", x, y, text, "Times New Roman", 55, Color.Yellow);
        AddObjects();
    }
    public GameUIButton(string name, int x, int y, string text, Color boxColor, Color textColor, bool? isCameraAffected = false) : base(name, x, y, 30, 15)
    {
        _defaultBox = new Box(name + "_default", x, y, 30, 15, boxColor);
        _hoverBox = new Box(name + "_hover", x - 1, y - 1, 30 + 1, 15 + 1, boxColor);
        _clickBox = new Box(name + "_click", x, y, 30, 15, boxColor);

        _defaultText = new GameUIText(name + "_text", x, y, text, "Times New Roman", 120, textColor);
        _hoverText = new GameUIText(name + "_text", x, y, text, "Times New Roman", 120, textColor);
        _clickText = new GameUIText(name + "_text", x, y, text, "Times New Roman", 120, textColor);
        AddObjects();
    }
    public GameUIButton(string name, int x, int y, int width, int height, string text, int textX, int textY, int textSize, bool? isCameraAffected = false)
        : base(name, x, y, width, height)
    {
        _defaultBox = new Box(name + "_default", x, y, width, height, Color.Gray);
        _hoverBox = new Box(name + "_hover", x - 1, y - 1, width + 2, height + 2, Color.Yellow);
        _clickBox = new Box(name + "_click", x, y, width, height, Color.Blue);

        _defaultText = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.Black);
        _hoverText = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.Black);
        _clickText = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.Yellow);
        AddObjects();
    }
    public GameUIButton(string name, float x, float y, float width, float height, string text, float textX, float textY, int textSize, bool? isCameraAffected = false)
        : base(name, x, y, width, height)
    {
        _defaultBox = new Box(name + "_default", x, y, width, height, Color.Gray);
        _hoverBox = new Box(name + "_hover", x - 1, y - 1, width + 2, height + 2, Color.Yellow);
        _clickBox = new Box(name + "_click", x, y, width, height, Color.Blue);

        _defaultText = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.Black);
        _hoverText = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.Black);
        _clickText = new GameUIText(name + "_text", x + textX, y + textY, text, "Times New Roman", textSize, Color.Yellow);
        AddObjects();
    }

    public void ChangeBoxBackground(BoxType boxType, Color color)
    {
        switch (boxType)
        {
            case BoxType.Default: _defaultBox.BackgroundColor = color; break;
            case BoxType.Hover: _hoverBox.BackgroundColor = color; break;
            case BoxType.Click: _clickBox.BackgroundColor = color; break;
            default: break;
        }
    }

    public void ChangeBoxBackground(BoxType boxType, Image image)
    {
        switch (boxType)
        {
            case BoxType.Default: _defaultBox.BackgroundImage = image; break;
            case BoxType.Hover: _hoverBox.BackgroundImage = image; break;
            case BoxType.Click: _clickBox.BackgroundImage = image; break;
            default: break;
        }
    }

    public void ChangeTextAlignment(TextPositioning textPositioning)
    {
        _defaultText.ChangeTextAlignment(textPositioning);
        _hoverText.ChangeTextAlignment(textPositioning);
        _clickText.ChangeTextAlignment(textPositioning);
    }

    private void AddObjects()
    {
        _indBox = RenderList.AddObject(0, _defaultBox);
        _indText = RenderList.AddObject(1, _defaultText);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference) { }

    public override void StickToWindowPart(WindowPart part)
    {
        base.StickToWindowPart(part);

        _hoverBox.StickToWindowPart(part);
        _hoverText.StickToWindowPart(part);

        _clickBox.StickToWindowPart(part);
        _clickText.StickToWindowPart(part);
    }
    public override void OnScreenResize()  
    {
        base.OnScreenResize();

        _hoverBox.OnScreenResize();
        _hoverText.OnScreenResize();

        _clickBox.OnScreenResize();
        _clickText.OnScreenResize();
    }

    // Interface Implementation
    public void OnMouseEnter()
    {
        if (HasMouseOver || IsClickStarted) return;
        HasMouseOver = true;
        RenderList.ChangeObject(0, _indBox, _hoverBox);
        RenderList.ChangeObject(1, _indText, _hoverText);
    }
    public void OnMouseLeave()
    {
        if (!HasMouseOver) return;
        HasMouseOver = false;
        IsClickStarted = false;
        RenderList.ChangeObject(0, _indBox, _defaultBox);
        RenderList.ChangeObject(1, _indText, _defaultText);
    }
    void IInteractive.OnMouseDown()
    {
        if (IsClickStarted) return;
        IsClickStarted = true;
        RenderList.ChangeObject(0, _indBox, _clickBox);
        RenderList.ChangeObject(1, _indText, _clickText);
    }
    public void OnMouseUp(Action action)
    {
        if (!IsClickStarted) return;
        IsClickStarted = false;
        RenderList.ChangeObject(0, _indBox, _hoverBox);
        RenderList.ChangeObject(1, _indText, _hoverText);
        action();
    }

    public void OnMouseEnter(Action action) { }
    public void OnMouseLeave(Action action) { }
    void IInteractive.OnMouseDown(Action action) { }
    public void OnMouseUp() { }
    public void OnKeyDown() { }
    public void OnKeyDown(Action action) { }
    public void OnKeyUp() { }
    public void OnKeyUp(Action action) { }
}
