using EU_values.Game.GameObjects;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level2Scene : InGameScene
{
    private WordEnterWindow _wordWindow;

    private Compass _compass;

    private int compassRotation = 360;

    private string[] _wordForGuessing = { "LIBERTY", "IDENTITY", "VOICE", "DELIVERANCE", "BELIEF", "EMANCIPATION", "INDEPENDENCE", "RELEASE" };
    private string _preparedWord = "";
    private List<int> _wordsOrder = [];
    private int _currentWordIndex = 0;
    private int _numberWordToGuess = 4;
    private int _angleToRotate = 0;
    public Level2Scene(GameLevels previousLevel) : base(previousLevel)
    {
        _background = new Box("Level_2_Background", 0, 0, 2318, 1389, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_2\\Level_2_Background.png"), true);
        _wordWindow = new("Level2WordEnterWindow", 0, 0, 300, "Enter a word, which represents freadom\nSelect letter position and press \"T\" to fill or change the letter");
        _compass = new(1000, 500);

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _compass);
        RenderList.AddObject(2, _player.Body);
        RenderList.AddObject(1, _wordWindow);

        LastLevelHintPhrase = "\"Only a free heart knows which way is forward.\"";
        FinalPhraseText = "\"Freedom means choosing your own path,\nnot following someone else’s zigzag!\"";

        _angleToRotate = 360 / _numberWordToGuess;
        _wordsOrder = GameRandom.GetRandomIndexes(_numberWordToGuess);
        _preparedWord = _wordForGuessing[_wordsOrder[_currentWordIndex]];

        ReadData("..\\..\\..\\Resources\\Data\\Level_2.json");
        if (_tiledMap != null)
            foreach (var layer in _tiledMap.Layers)
            {
                if (layer.Objects != null && layer.Name == "WordWindow")
                    _wordWindow.ChangePosition(layer.Objects[0].X, layer.Objects[0].Y);

                else if (layer.Objects != null && layer.Name == "Compass")
                    _compass.ChangePosition(layer.Objects[0].X, layer.Objects[0].Y);
            }

        _wordWindow.PrepareWord(_preparedWord);
        SetupDoors(0);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        for (int i = 0; i < _wordWindow.LetterWindows.Count; i++)
        {
            if (Collider.DetectFullEnterCollision(_player.Hitbox, _wordWindow.LetterWindowsHitboxes[i]))
                _wordWindow.SelectLetterWindow(i);
            else if (_wordWindow.LetterWindows[i].Background.BackgroundColor == Color.Yellow)
                _wordWindow.DeselectLetterWindow(i);
        }

        if (_wordWindow.IsLetterActivated) _wordWindow.BlinkActive(timeRemaining);

        if (_wordWindow.EnteredWord == _preparedWord && compassRotation > 0)
        {
            if (!_wordWindow.IsWordFinished)
            {
                _wordWindow.OnCorrectWord();
                _compass.SwitchToNormalArrow();
            }
            compassRotation--;
            _compass.RotateArrow();
        }
        if (compassRotation % _angleToRotate == 0 && _wordWindow.EnteredWord == _preparedWord)
        {
            _currentWordIndex++;
            if (_currentWordIndex < _numberWordToGuess)
            {
                _preparedWord = _wordForGuessing[_wordsOrder[_currentWordIndex]];
                _wordWindow.PrepareWord(_preparedWord);
                _compass.SwitchToQuestionArrow();
            } else
            {
                _wordWindow.ToggleVisibility(false);
            }
        }

        _playerData1.Text = $"{_playerData1.Text}" +
            $"\n - prepared word: {_preparedWord}" +
            $"\n - Entered word: {_wordWindow.EnteredWord}" +
            $"\n - compass rotation: {compassRotation}";
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
    }

    protected override bool IsLevelTaskFulfilled() => _currentWordIndex == _numberWordToGuess;
}
