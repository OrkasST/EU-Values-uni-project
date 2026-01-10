namespace EU_values.Game.Interfaces;

public interface IInteractive
{
    bool HasMouseOver { get; protected set; }
    bool IsClickStarted { get; protected set; }

    void OnMouseEnter();
    void OnMouseEnter(Action action);

    void OnMouseLeave();
    void OnMouseLeave(Action action);

    void OnMouseDown();
    void OnMouseDown(Action action);

    void OnMouseUp();
    void OnMouseUp(Action action);

    void OnKeyDown();
    void OnKeyDown(Action action);

    void OnKeyUp();
    void OnKeyUp(Action action);

}
