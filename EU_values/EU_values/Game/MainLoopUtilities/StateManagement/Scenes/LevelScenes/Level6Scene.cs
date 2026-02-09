using EU_values.Game.BaseClasses;
using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;
using System.Drawing;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level6Scene : InGameScene
{
    private static string[] _iconNames = { "Privacy", "Belief", "Identity", "Origin", "Gender", "Religion" };
    private PlayerInventory _playerInventory;
    private List<Level6AcceptorWindow> _aceptors = [];
    private bool _isAcceptorSelected = false;

    private Level6AcceptorWindow? _selectedAcceptor;
    private GameWindow _hint;

    public Level6Scene(GameLevels previousLevel) : base(previousLevel)
    {
        _background = new Box("Level_2_Background", 0, 0, 4500, 2000, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_6\\Level_6_Background.png"), true);
        _playerInventory = new(name: "Level_4_PlayerInventory", x: 400, y: 40, inventorySize: 6);

        _hint = new(name: "Action hint", x: 0, y: 0, width: 400, height: 60,
            text: "Press \"T\" to place item from inventory" +
            "\nPress \"Y\" to take item", textX: 200, textY: 10, textSize: 20, isCameraAffected: true);
        _hint.Text.ChangeTextAlignment(TextPositioning.Center);
        _hint.Background.ChangeBackground(Color.FromArgb(100, Color.Black));
        _hint.ToggleVisibility(false);

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);
        RenderList.AddObject(2, _hint);

        foreach (string s in _iconNames)
        {
            var icon = new Level6Icon(name: s, x: 0, y: 0,
            imageName: $"{s}.png");
            _playerInventory.StoreItem(icon);
            var position = _playerInventory.GetCellPosition();
            icon.SetPositionByCenter(position.X, position.Y);
            RenderList.AddObject(0, icon.Body);
        }

        RenderList.AddObject(3, _playerInventory);

        LastLevelHintPhrase = "\"What is denied in silence must be spoken into light.\"";
        FinalPhraseText = "\"Rights exist even when they are unheard.\"";

        ReadData("..\\..\\..\\Resources\\Data\\Level_6.json");

        if (_tiledMap != null)
            foreach (var layer in _tiledMap.Layers)
            {
                if (layer.Name == "IconAccepters")
                {
                    for (int i = 0; i < layer.Objects?.Count; i++)
                    {
                        _aceptors.Add(new(name: _iconNames[i], x: layer.Objects[i].X, y: layer.Objects[i].Y));
                        RenderList.AddObject(3, _aceptors.Last());
                    }
                }
            }

        SetupDoors(0);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        foreach (var obj in _aceptors)
        {
            if (Collider.DetectFullEnterCollision(new PointF(_player.Body.Position.X + _player.Body.Size.Width / 2, _player.Body.Position.Y + _player.Body.Size.Height / 2),
                obj.InteractiveArea))
            {
                if (!_hint.IsVisible)
                {
                    _hint.ChangePosition(obj.Position.X - 140, obj.Position.Y - 70);
                    _hint.ToggleVisibility(true);
                }

                obj.Select();
                _isAcceptorSelected = true;
                _selectedAcceptor = obj;
            }
            else if (obj.IsSelected)
            {
                obj.Deselect();
                _selectedAcceptor = null;
                _hint.ToggleVisibility(false);
            }
        }
    }

    public override void HandleUserInput()
    {
        base.HandleUserInput();

        if (InputHandler.NoKeyboardEvents) return;
        if (InputHandler.LastKeyboardEvent.IsHandled) return;

        if (InputHandler.LastKeyboardEvent.Key == Keys.E && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _playerInventory.MoveSelection(true);
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }
        else if (InputHandler.LastKeyboardEvent.Key == Keys.Q && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            _playerInventory.MoveSelection(false);
            InputHandler.LastKeyboardEvent.IsHandled = true;
        }


        if (InputHandler.LastKeyboardEvent.Key == Keys.T && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            if (_isAcceptorSelected && _selectedAcceptor != null)
            {
                _selectedAcceptor.StoreItem((Level6Icon?)_playerInventory.RemoveItem());
            }
        }
        if (InputHandler.LastKeyboardEvent.Key == Keys.Y && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            if (_isAcceptorSelected && _selectedAcceptor != null)
            {
                var item = _selectedAcceptor.RemoveItem();
                _playerInventory.StoreItem(item);

                var position = _playerInventory.GetCellPosition();
                if (item is Actor) (item as Actor)?.SetPositionByCenter(position.X, position.Y);
            }
        }

    }

    protected override bool IsLevelTaskFulfilled()
    {
        bool areFullfilled = true;
        foreach (var aceptor in _aceptors) if (!aceptor.IsFullfilled) areFullfilled = false;
        return areFullfilled;
    }
}
