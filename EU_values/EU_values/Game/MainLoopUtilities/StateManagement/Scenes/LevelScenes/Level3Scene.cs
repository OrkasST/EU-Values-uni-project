using EU_values.Game.BaseClasses;
using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level3Scene: InGameScene
{
    private PlayerInventory _playerInventory;
    public static readonly string GuessedWord = "VOTE";

    private List<LetterCoin> _letters;
    private List<PointF> _letterDestinationPoints = [];

    private WordEnterWindow _wordWindow;
    private static string _preparedWord = "VOTE";

    public Level3Scene(GameLevels previousLevel) : base(previousLevel)
    {
        LastLevelHintPhrase = "\"Your voice shapes the path we walk together.\"";
        FinalPhraseText = "\"Democracy isn’t about picking the ‘right’ option…\nit’s about making the choice.\"";

        _background = new Box("Level_3_Background", 0, 0, 4000, 2252, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_3\\Level_3_Background.jpg"), true);

        _playerInventory = new(name: "Level_3_PlayerInventory", x: 400, y: 40, inventorySize: GuessedWord.Length);

        _wordWindow = new("Level3WordEnterWindow", 0, 0, 300, "Guess the word\nSelect letter position and press \"T\" to fill or change the letter");

        _letters = new();
        for (int i = 0; i < GuessedWord.Length; i++)
            _letters.Add(new LetterCoin(name: $"LetterCoin{GuessedWord[i]}", letter: $"{GuessedWord[i]}", 1100, 1100, timeToFirstJump: 2000));

        #region Adding Objects To Render List

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _player.Body);
        RenderList.AddObject(3, _playerInventory);
        RenderList.AddObject(0, _wordWindow);
        foreach (var letter in _letters) RenderList.AddObject(1, letter.Body);
        #endregion

        ReadData("..\\..\\..\\Resources\\Data\\Level_3.json");
        if (_tiledMap != null)
            foreach (var layer in _tiledMap.Layers)
            {
                if (layer.Name == "LettersLocations") for (int i = 0; i < layer.Objects?.Count; i++)
                        _letterDestinationPoints.Add(new PointF(layer.Objects[i].X, layer.Objects[i].Y));
                else if (layer.Objects != null && layer.Name == "WordWindow")
                {
                    _wordWindow.ChangePosition(layer.Objects[0].X, layer.Objects[0].Y);
                }
            }
        _wordWindow.PrepareWord(_preparedWord);
        _wordWindow.ToggleVisibility(false);

        var indexes = GameRandom.GetRandomIndexes(_letterDestinationPoints.Count);
        for (int i = 0; i < _letters.Count; i++) _letters[i].SetDestinationPoint(_letterDestinationPoints[indexes[i]]);

        SetupDoors(0);
    }
    public override void HandleUserInput()
    {
        if (InputHandler.NoKeyboardEvents) return;

        if (_wordWindow.IsLetterSelected && InputHandler.LastKeyboardEvent.Key == Keys.T && !InputHandler.LastKeyboardEvent.IsHandled
           && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp && !_wordWindow.IsLetterActivated)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _wordWindow.ActivateSelected();
            _isPlayerCanMove = false;
        }
        if (_wordWindow.IsLetterActivated && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            if (!(InputHandler.LastKeyboardEvent.IsHandled && InputHandler.LastKeyboardEvent.Key == Keys.T))
            {
                InputHandler.LastKeyboardEvent.IsHandled = true;
                _wordWindow.ChangeLetter(InputHandler.LastKeyboardEvent.Key);
                _isPlayerCanMove = true;
            }
        }

        base.HandleUserInput();

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
        else if (_isInDeveloperMode && InputHandler.LastKeyboardEvent.Key == Keys.F4 && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            foreach (var letter in _letters)
            {
                _playerInventory.StoreItem(letter);
                var newPosition = _playerInventory.GetCellPosition();
                letter.SetPositionByCenter(newPosition.X, newPosition.Y);
                letter.IsCollidable = false;
            }
        }
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        foreach (var letter in _letters)
        {
            letter.Update(timeDelta, timeRemaining, timeDifference);

            if (letter.CurrentState == ActorState.Falling && Collider.DetectAccuratelyPartialEnterCollision(letter.Hitbox, Ground))
            {
                letter.SetPosition(Collider.lastPartialEnterTouchPosition);
                letter.OnGroundHit();
            }

            if (Collider.DetectAccuratelyPartialEnterCollision(letter.Hitbox, LevelBorders))
            {
                letter.SetPositionByCenter(
                    Collider.lastAccuratePartialEnteredHitbox.X < letter.Hitbox.X ?
                    Collider.lastAccuratePartialEnteredHitbox.X + Collider.lastAccuratePartialEnteredHitbox.Width + letter.Hitbox.Width / 2
                    : Collider.lastAccuratePartialEnteredHitbox.X - letter.Hitbox.Width / 2,
                    letter.Body.Position.Y + (letter.Body.Size.Height / 2));
                letter.OnLevelBorderHit();
            }

            if (letter.IsAtFinalLocation && Collider.DetectPartialEnterCollision(letter.Hitbox, _player.Hitbox))
            {
                _playerInventory.StoreItem(letter);
                var newPosition = _playerInventory.GetCellPosition();
                letter.SetPositionByCenter(newPosition.X, newPosition.Y);
                letter.IsCollidable = false;
            }
        }

        if (_wordWindow.IsVisible)
        {
            for (int i = 0; i < _wordWindow.LetterWindows.Count; i++)
            {
                if (Collider.DetectFullEnterCollision(_player.Hitbox, _wordWindow.LetterWindowsHitboxes[i]))
                    _wordWindow.SelectLetterWindow(i);
                else if (_wordWindow.LetterWindows[i].Background.BackgroundColor == Color.Yellow)
                    _wordWindow.DeselectLetterWindow(i);
            }

            if (_wordWindow.IsLetterActivated) _wordWindow.BlinkActive(timeRemaining);
        }

        if (_playerInventory.InventoryItems.Count == _playerInventory.InventorySize) _wordWindow.ToggleVisibility(true);
    }

    protected override bool IsLevelTaskFulfilled() => _wordWindow.EnteredWord == _preparedWord;
}