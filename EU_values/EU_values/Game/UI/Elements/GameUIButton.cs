using EU_values.Game.BaseClasses;
using EU_values.Game.Interfaces;

namespace EU_values.Game.UI.Elements;

internal class GameUIButton : ComplexDrawableObject, IInteractive
{
    private Box _defaultBox;
    private int _indBox;
    
    private Box _hoverBox;
    private Box _clickBox;

    private GameUIText _defaultText;
    private int _indText;

    private GameUIText _hoverText;
    private GameUIText _clickText;

    public bool HasMouseOver { get; set; } = false;
    public bool IsClickStarted { get; set; } = false;

    public GameUIButton (string name, int x, int y, string text) : base (name, x, y, 210, 60)
    {
        _defaultBox = new Box(name + "_default", x, y, 210, 60, Color.Gray);
        _hoverBox = new Box(name + "_hover", x-1, y-1, 210+2, 60+2, Color.Yellow);
        _clickBox = new Box(name + "_click", x, y, 210, 60, Color.Blue);

        _defaultText = new GameUIText(name+"_text", x, y, text, new Font("Times New Roman", 55), Color.Black);
        _hoverText = new GameUIText(name+"_text", x, y, text, new Font("Times New Roman", 55), Color.Black);
        _clickText = new GameUIText(name+"_text", x, y, text, new Font("Times New Roman", 55), Color.Yellow);
        AddObjects();
    }
    public GameUIButton(string name, int x, int y, string text, Color boxColor, Color textColor) : base(name, x, y, 30, 15)
    {
        _defaultBox = new Box(name+"_default", x, y, 30, 15, boxColor);
        _hoverBox = new Box(name + "_hover", x-1, y-1, 30+1, 15+1, boxColor);
        _clickBox = new Box(name + "_click", x, y, 30, 15, boxColor);

        _defaultText = new GameUIText(name + "_text", x, y, text, new Font("Times New Roman", 120), textColor);
        _hoverText = new GameUIText(name + "_text", x, y, text, new Font("Times New Roman", 120), textColor);
        _clickText = new GameUIText(name + "_text", x, y, text, new Font("Times New Roman", 120), textColor);
        AddObjects();
    }
    public GameUIButton(string name, int x, int y, string text, Size boxSize, int textSize) : base(name, x,y,boxSize)
    {
        _defaultBox = new Box(name + "_default", x, y, 30, 15, Color.Gray);
        _hoverBox = new Box(name + "_hover", x, y, 30, 15, Color.Gray);
        _clickBox = new Box(name + "_click", x, y, 30, 15, Color.Gray);

        _defaultText = new GameUIText(name + "_text", x, y, text, new Font("Times New Roman", 120), Color.Black);
        _hoverText = new GameUIText(name + "_text", x, y, text, new Font("Times New Roman", 120), Color.Black);
        _clickText = new GameUIText(name + "_text", x, y, text, new Font("Times New Roman", 120), Color.Black);
        AddObjects();
    }

    private void AddObjects()
    {
        _indBox = RenderList.AddObject(0, _defaultBox);
        _indText = RenderList.AddObject(1, _defaultText);
    }

    public override void Update(int timeDelta, int timeRemaining) { }

    // Interface Implementation
    public void OnMouseEnter() {
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
        IsClickStarted= true;
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
