using EU_values.Game.BaseClasses;
using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level1Scene: InGameScene
{
    public static string[,] MaskNamesAndDescriptions =
    {
        { "Silence", "Quiet people are weak." },
        { "Anger", "Angry people are dangerous." },
        { "Fear", "Fear means incompetence." },
        { "Joy", "Happy people don’t take things seriously." },
        { "Sadness", "Sadness means failure." },
        { "Neutral", "If they don’t react, they don’t care." },
    };

    private Box _doorShadow;
    private Box _tree;

    private List<Mask> _masks;

    private List<PointF> _maskDestinationPoints = [];

    private PlayerInventory _playerInventory;
    private GameWindow _inventoryHint;

    private Level1QuestionWindow _questionWindow;

    private float _hintApearTime = -1;

    public Level1Scene(GameLevels previousLevel) : base(previousLevel)
    {
        _background = new("Level_1_Background", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_Background.png"), true);
        _doorShadow = new("Level_1_DoorShadow", 0, 0, 2499, 1389, System.Drawing.Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Level_1_DoorShadow.png"), true);
        _tree = new("Level_1_tree", 0, 0, 2499, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_1\\Tree.png"), true);
        _playerInventory = new(name: "Level_1_PlayerInventory", x: 400, y: 120, inventorySize: MaskNamesAndDescriptions.GetLength(0));

        LastLevelHintPhrase = "\"One truth: every face hides the same infinite worth.\"";
        FinalPhraseText = "\"Look at the person, not the mask.\"";

        _questionWindow = new(question: "Accept the judgement or Question this judgement?");
        _actionList.Add(_questionWindow.AcceptButton.Name, () =>
        {
            _questionWindow.ToggleVisibility(false);
            ((Mask)_playerInventory.GetSelectedItem()).IsQuestioned = false;
        });
        _actionList.Add(_questionWindow.QuestionButton.Name, () =>
        {
            _questionWindow.ToggleVisibility(false);
            ((Mask)_playerInventory.GetSelectedItem()).IsQuestioned = true;
        });

        #region SettingUp Inventory Related Window
        _inventoryHint = new(name: "Level_1_InventoryHint", x: UIPositioner.CenterX - 200, y: UIPositioner.CenterY - 100, width: 400, height: 150,
            text: "Press \"E\" to select right item\nPress \"Q\" to select left item\nPress \"R\" to see items description",
            textX: 20, textY: 30, textSize: 26, isCameraAffected: true);
        _inventoryHint.ToggleVisibility();
        #endregion

        _masks = new();
        for (int i = 0; i < MaskNamesAndDescriptions.GetLength(0); i++)
            _masks.Add(new Mask(MaskNamesAndDescriptions[i,0], MaskNamesAndDescriptions[i,1], 1100, 1100, timeToFirstJump: 2000));

        #region Adding Objects to render list

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);
        RenderList.AddObject(2, _doorShadow);
        RenderList.AddObject(2, _tree);
        RenderList.AddObject(3, _playerInventory);
        RenderList.AddObject(3, _inventoryHint);
        RenderList.AddObject(3, _questionWindow);

        foreach (var mask in _masks) RenderList.AddObject(1, mask.Body);
        #endregion

        ReadData("..\\..\\..\\Resources\\Data\\Level_1.json");
        if (_tiledMap != null)
            foreach (var layer in _tiledMap.Layers)
                if (layer.Name == "MasksLocations") for (int i = 0; i < layer.Objects?.Count; i++)
                        _maskDestinationPoints.Add(new PointF(layer.Objects[i].X, layer.Objects[i].Y));

        var indexes = GameRandom.GetRandomIndexes(_maskDestinationPoints.Count);
        for (int i = 0; i < _masks.Count; i++) _masks[i].SetDestinationPoint(_maskDestinationPoints[indexes[i]]);

        SetupDoors(0);
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
            ShowQuestion();
        }
        else if (InputHandler.LastKeyboardEvent.Key == Keys.F4 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            foreach (var mask in _masks)
            {
                _playerInventory.StoreItem(mask);
                var newPosition = _playerInventory.GetCellPosition();
                mask.SetPositionByCenter(newPosition.X, newPosition.Y);
                mask.IsCollidable = false;
            }
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        foreach (var mask in _masks)
        {
            mask.Update(timeDelta, timeRemaining, timeDifference);

            if (mask.CurrentState == ActorState.Falling && Collider.DetectAccuratelyPartialEnterCollision(mask.Hitbox, Ground))
            {
                mask.SetPosition(Collider.lastPartialEnterTouchPosition);
                mask.OnGroundHit();
            }

            if (Collider.DetectAccuratelyPartialEnterCollision(mask.Hitbox, LevelBorders))
            {
                mask.SetPositionByCenter(
                    Collider.lastAccuratePartialEnteredHitbox.X < mask.Hitbox.X ?
                    Collider.lastAccuratePartialEnteredHitbox.X + Collider.lastAccuratePartialEnteredHitbox.Width + mask.Hitbox.Width / 2
                    : Collider.lastAccuratePartialEnteredHitbox.X - mask.Hitbox.Width / 2,
                    mask.Body.Position.Y + (mask.Body.Size.Height / 2));
                mask.OnLevelBorderHit();
            }

            if (mask.IsAtFinalLocation && Collider.DetectPartialEnterCollision(mask.Hitbox, _player.Hitbox))
            {
                _playerInventory.StoreItem(mask);
                var newPosition = _playerInventory.GetCellPosition();
                mask.SetPositionByCenter(newPosition.X, newPosition.Y);
                mask.IsCollidable = false;
            }
        }

        if (!_inventoryHint.Background.IsVisible && _hintApearTime == -1)
        {
            _inventoryHint.ToggleVisibility();
            _hintApearTime = timeRemaining;
        }
        if (_inventoryHint.IsVisible && timeRemaining - _hintApearTime >= 6000) _inventoryHint.ToggleVisibility(false);
        
    }

    private void ShowQuestion()
    {
        string description = _playerInventory.GetItemDescription();
        if (description == "") return;

        _questionWindow.SetDescription(description);
        _questionWindow.ToggleVisibility(true);
    }

    protected override bool IsLevelTaskFulfilled() => _masks.Find(mask => !mask.IsQuestioned) == null;
    protected override void OnLevelTaskFulfilled()
    {
        base.OnLevelTaskFulfilled();
        _finalPhrase.ToggleVisibility(true);
    }
}
