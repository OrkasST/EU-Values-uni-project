using EU_values.Game.Interfaces;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes;

public class MainMenuScene : Scene
{
    private GameUIText label1;
    private GameUIButton button1;
    private GameUIText MouseCoordinates;

    private Dictionary<string, Action> _actionList;

    public MainMenuScene()
    {
        label1 = new GameUIText("MainMenu_Label1", 10, 10, "Hello My Game", "Times New Roman", 26);
        button1 = new GameUIButton("MainMenu_button1", 10, 80, "Button 1");
        MouseCoordinates = new GameUIText("MainMenu_MouseCoordinates", 10, 160, "X:0 Y:0", "Times New Roman", 20);

        RenderList.AddObject(0, label1);
        RenderList.AddObject(0, button1);
        RenderList.AddObject(0, MouseCoordinates);

        _actionList = new Dictionary<string, Action>
        {
            [button1.Name] = () =>
            {
                _nextState = States.InGameActive;
                _nextLevel = GameLevels.Level_1;
            }
        };
    }

    public override void HandleUserInput()
    {
        if (!InputHandler.NoMouseEvents)
        {
            var coordinates = InputHandler.LastMouseEvent.Location;
            MouseCoordinates.Text = $"X: {coordinates.X} | Y: {coordinates.Y}";

            foreach (var layer in RenderList.Layers())
                foreach (var obj in layer)
                {
                    if (!(obj is IInteractive)) continue;

                    var iObj = (IInteractive)obj;

                    MouseCoordinates.Text += $"\nO.X: {obj.Position.X} | O.Y: {obj.Position.Y}";
                    MouseCoordinates.Text += $"\nO.X2: {obj.Position.X + obj.Size.Width} | O.Y2: {obj.Position.Y + obj.Size.Height}";

                    if (obj.Position.X <= coordinates.X && obj.Position.Y <= coordinates.Y
                        && obj.Position.X + obj.Size.Width >= coordinates.X && obj.Position.Y + obj.Size.Height >= coordinates.Y)
                    {
                        iObj.OnMouseEnter();
                        if (InputHandler.LastMouseEvent.IsLeftPressed) iObj.OnMouseDown();
                        else if (!InputHandler.LastMouseEvent.IsRightPressed) iObj.OnMouseUp(_actionList[obj.Name]);
                    }
                    else if (iObj.HasMouseOver) iObj.OnMouseLeave();
                }
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        if (_nextLevel != GameLevels.None) AwaitedLevel = _nextLevel;
    }
}
