using EU_values.Game.BaseClasses;
using EU_values.Game.Interfaces;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes;

public class SaveChooseScene : Scene
{
    public List<GameWindow> Saves = [];
    protected Dictionary<string, Action> _actionList = [];
    private Dictionary<string, int> _keys = [];

    public SaveChooseScene() : base()
    {
        for (int i = 0; i < GameManager.GameInfo.Fields.Count; i++)
        {
            var levelInfo = GameManager.GameInfo.Fields[i].Level > 0 ? $"Level: {GameManager.GameInfo.Fields[i].Level}" : "";

            var save = new GameWindow($"Save_{GameManager.GameInfo.Fields[i].Name}", x: -400, y: -600 + i * 400,
                width: 800, height: 150, text: $"{i + 1}. Save: {GameManager.GameInfo.Fields[i].Name}\n{levelInfo}",
                textX: 50, textY: 50, textSize: 30);
            save.StickToWindowPart(WindowPart.Center);
            save.Background.ChangeBackground(Color.FromArgb(50, Color.DarkGray));

            var chooseSaveBtn = new GameUIButton($"Save_{i}_btn", x: 200, y: -600 + i * 400 + 50, width: 180, height: 50, text: "Choose save", textX: 90, textY: 10, textSize: 30);
            chooseSaveBtn.StickToWindowPart(WindowPart.Center);
            chooseSaveBtn.ChangeTextAlignment(TextPositioning.Center);
            save.AddObject(chooseSaveBtn, 1);

            var resetSaveBtn = new GameUIButton($"Save_{i}_btn_", x: 0, y: -600 + i * 400 + 50, width: 180, height: 50, text: "Reset save", textX: 90, textY: 10, textSize: 30);
            resetSaveBtn.StickToWindowPart(WindowPart.Center);
            resetSaveBtn.ChangeTextAlignment(TextPositioning.Center);
            save.AddObject(resetSaveBtn, 1);

            _keys.Add(chooseSaveBtn.Name, i);

            _actionList.Add(chooseSaveBtn.Name, () =>
            {
                GameManager.ChosenSave = _keys[chooseSaveBtn.Name];
                _nextState = States.InGameActive;
                _nextLevel = GameManager.GameInfo.Fields[_keys[chooseSaveBtn.Name]].Level > 0 ?
                (GameLevels)GameManager.GameInfo.Fields[_keys[chooseSaveBtn.Name]].Level : GameLevels.Level_1;
            });
            _actionList.Add(resetSaveBtn.Name, () =>
            {
                GameManager.GameInfo.Fields[_keys[resetSaveBtn.Name.Substring(0, resetSaveBtn.Name.Length-1)]].Level = 0;
                ActionInjector.RequestAction(ActionType.SaveGame)();
                _nextState = States.InSaveChooseMenu;
            });

            Saves.Add(save);
            RenderList.AddObject(0, save);
        }
    }

    public override void HandleUserInput()
    {
        if (!InputHandler.NoMouseEvents)
        {
            HandleMouse(InputHandler.LastMouseEvent.Location, RenderList);
        }
    }

    private void HandleMouse(Point coordinates, RenderLayerList RenderList)
    {
        foreach (var layer in RenderList.Layers())
            foreach (var obj in layer)
            {
                if (obj is ComplexDrawableObject && !(obj is IInteractive))
                {
                    var complex = (ComplexDrawableObject)obj;
                    HandleMouse(coordinates, complex.RenderList);
                    continue;
                }

                if (!(obj is IInteractive)) continue;

                var iObj = (IInteractive)obj;

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


    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);
        if (_nextLevel != GameLevels.None) AwaitedLevel = _nextLevel;
    }
}
