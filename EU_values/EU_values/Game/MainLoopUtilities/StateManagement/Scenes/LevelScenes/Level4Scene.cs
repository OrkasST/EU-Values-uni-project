using EU_values.Game.BaseClasses;
using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level4Scene : InGameScene
{
    private Scales _scales;
    private Box _pointer;
    private PlayerInventory _playerInventory;

    private GameWindow _hint;

    private float _warningAppearTime = -1;

    private string[] _sliders = { "Gender", "Age", "Origin", "Identity" };

    public Level4Scene(GameLevels previousLevel) : base(previousLevel)
    {
        _background = new Box("Level_2_Background", 0, 0, 4000, 2240, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_4\\Level_4_Background.png"), true);
        _scales = new(x: 1919, y: 1949, width: 256, height: 256);
        _pointer = new(name: "pointer", x: _scales.Position.X, y: _scales.Position.Y, width: 10, height: 10, Color.Aqua, isCameraAffected: true);
        _pointer.ToggleVisibility(false);
        _playerInventory = new(name: "Level_4_PlayerInventory", x: 400, y: 40, inventorySize: 4);

        _hint = new(name: "Action hint", x: _scales.Position.X - 128, y: _scales.Position.Y - 80, width: 512, height: 60,
            text: "Press \"T\" to place item from inventory to scales cup" +
            "\nPress \"Y\" to take item", textX: 256, textY: 10, textSize: 20, isCameraAffected: true);
        _hint.Text.ChangeTextAlignment(TextPositioning.Center);
        _hint.ToggleVisibility(false);

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _scales);
        RenderList.AddObject(1, _pointer);
        RenderList.AddObject(2, _player.Body);
        RenderList.AddObject(3, _playerInventory);
        RenderList.AddObject(3, _hint);

        foreach (string s in _sliders)
        {
            var concept = new Concept(name: s, description: s, x: 0, y: 0,
            imageName: $"{s}.png");
            _playerInventory.StoreItem(concept);
            var position = _playerInventory.GetCellPosition();
            concept.SetPositionByCenter(position.X, position.Y);
            RenderList.AddObject(0, concept.Body);
        }

        LastLevelHintPhrase = "\"Only when all sides stand level can truth stand tall.\"";
        FinalPhraseText = "\"Equality is the soul of liberty.\nThere is, in fact, no liberty without it.\"";

        ReadData("..\\..\\..\\Resources\\Data\\Level_4.json");
        SetupDoors(0);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);
        if (Collider.DetectFullEnterCollision(
            new PointF(_player.Body.Position.X + _player.Body.Size.Width / 2, _player.Body.Position.Y + _player.Body.Size.Height / 2),
            _scales.LeftCupHitbox))
        {
            _pointer.ToggleVisibility(true);
            _pointer.Position = new PointF(_scales.LeftCupPosition.X + 51, _scales.LeftCupPosition.Y + 12);
            _scales.SelectedCup = Cups.Left;
            _hint.ToggleVisibility(true);
        }
        else if (Collider.DetectFullEnterCollision(
            new PointF(_player.Body.Position.X + _player.Body.Size.Width / 2, _player.Body.Position.Y + _player.Body.Size.Height / 2),
            _scales.RightCupHitbox))
        {
            _pointer.ToggleVisibility(true);
            _pointer.Position = new PointF(_scales.RightCupPosition.X + 194, _scales.RightCupPosition.Y + 12);
            _scales.SelectedCup = Cups.Right;
            _hint.ToggleVisibility(true);
        }
        else if (Collider.DetectFullEnterCollision(
            new PointF(_player.Body.Position.X + _player.Body.Size.Width / 2, _player.Body.Position.Y + _player.Body.Size.Height / 2),
            _scales.MiddleCupHitbox))
        {
            _pointer.ToggleVisibility(true);
            _pointer.Position = new PointF(_scales.Position.X + 123, _scales.Position.Y - 20);
            _scales.SelectedCup = Cups.Middle;
            _hint.ToggleVisibility(true);
        }
        else
        {
            _pointer.ToggleVisibility(false);
            _scales.SelectedCup = Cups.None;
            _hint.ToggleVisibility(false);
        }


        _playerData.Text += $"\n\n ScalesRotation: {_scales._crossbar.Rotation}" +
            $"\n Items Left: {_scales.ItemsLeft}" +
            $"\n Items Right: {_scales.ItemsRight}";
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
        else if (InputHandler.LastKeyboardEvent.Key == Keys.R && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            if (!_playerInventory.ItemDescriptionWindow.IsVisible) _playerInventory.ShowItemDescription("\nPress \"R\" to close");
            else _playerInventory.HideItemDescription();
        }

        if (InputHandler.LastKeyboardEvent.Key == Keys.T && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            if (_scales.SelectedCup == Cups.Right)
            {
                if (_scales.LoadRightCup(_playerInventory.RemoveItem()))
                {
                    var item = _scales.RightCupInventory.Last();
                    if (item is Actor) (item as Actor)?.SetPositionByCenter(_scales.RightCupCenterPosition.X, _scales.RightCupCenterPosition.Y);
                }
            }
            else if (_scales.SelectedCup == Cups.Left)
            {
                if (_scales.LoadLeftCup(_playerInventory.RemoveItem()))
                {
                    var item = _scales.LeftCupInventory.Last();
                    if (item is Actor) (item as Actor)?.SetPositionByCenter(_scales.LeftCupCenterPosition.X, _scales.LeftCupCenterPosition.Y);
                }
            }
            else if (_scales.SelectedCup == Cups.Middle)
            {
                if (_scales.LoadMiddleCup(_playerInventory.RemoveItem()))
                {
                    var item = _scales.MiddleCupInventory.Last();
                    if (item is Actor) (item as Actor)?.SetPositionByCenter(_scales.MiddleCupCenterPosition.X, _scales.MiddleCupCenterPosition.Y);
                }
            }
        }
        if (InputHandler.LastKeyboardEvent.Key == Keys.Y && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            if (_scales.SelectedCup == Cups.Right && _scales.RightCupInventory.Count > 0)
            {
                var item = _scales.ReleaseRightCup();
                _playerInventory.StoreItem(item);

                var position = _playerInventory.GetCellPosition();
                if (item is Actor) (item as Actor)?.SetPositionByCenter(position.X, position.Y);
            }
            else if (_scales.SelectedCup == Cups.Left && _scales.LeftCupInventory.Count > 0)
            {
                var item = _scales.ReleaseLeftCup();
                _playerInventory.StoreItem(item);

                var position = _playerInventory.GetCellPosition();
                if (item is Actor) (item as Actor)?.SetPositionByCenter(position.X, position.Y);
            }
            else if (_scales.SelectedCup == Cups.Middle && _scales.MiddleCupInventory.Count > 0)
            {
                var item = _scales.ReleaseMiddleCup();
                _playerInventory.StoreItem(item);

                var position = _playerInventory.GetCellPosition();
                if (item is Actor) (item as Actor)?.SetPositionByCenter(position.X, position.Y);
            }
        }
    }

    protected override bool IsLevelTaskFulfilled() => _scales.ItemsMiddle == _sliders.Length;
}
