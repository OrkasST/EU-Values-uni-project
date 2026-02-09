using EU_values.Game.BaseClasses;
using EU_values.Game.GameObjects;
using EU_values.Game.GameObjects.Actors;
using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level7Scene : InGameScene
{
    private Ring _ring;
    private WordEnterWindow _finalQuiz;
    private string _preparedWord;

    private GameWindow _hint;
    private float _hintApearTime = -1;

    private static Dictionary<string, string> _hints = new () {
        ["WORTH"] = "\"One truth: every face hides the same infinite worth.\"",
        ["DIRECTION"] = "\"Only a free heart knows which way is forward.\"",
        ["VOICE"] = "\"Your voice shapes the path we walk together.\"",
        ["BALANCE"] = "\"Only when all sides stand level can truth stand tall.\"",
        ["EQUAL LAW"] = "\"Strength lies in rules that bend for no one.\"",
        ["LIGHT"] = "\"What is denied in silence must be spoken into light.\"",
    };
    private static Dictionary<string, string> _entered = new()
    {
        ["WORTH"] = "",
        ["DIRECTION"] = "",
        ["EQUAL LAW"] = "",
        ["VOICE"] = "",
        ["BALANCE"] = "",
        ["LIGHT"] = "",
    };

    private LimitLessWordEnterWindow _levelLastTaskWindow;

    private GameWindow _lastWords;
    private int _lastWordsApearTime = -1;

    public Level7Scene(GameLevels previousLevel) : base(previousLevel)
    {
        _background = new Box("Level_7_Background", 0, 0, 4000, 2000, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_7\\Level_7_Background.png"), true);
        _ring = new Ring(x: 1494, y: 424);
        _finalQuiz = new("FinalQuiz", x: 1494, y: 1450, width: 1000, text: "Guess The Word");
        _finalQuiz.Text.ToggleVisibility(true);

        _levelLastTaskWindow = new("\"Combine these into the value that binds them all.\"\n\nPress \"Tab\" to enter/leave edit mode");
        _levelLastTaskWindow.ToggleVisibility(false);

        #region Last Words Setup

        _lastWords = new GameWindow("Last Words", x: 0, y: 0, width: UIPositioner.FormWidth, height: UIPositioner.FormWidth,
            text: "", textX: UIPositioner.CenterX, textY: 200, textSize: 50);

        _lastWords.Text.ChangeTextAlignment(TextPositioning.Center);
        _lastWords.Background.ChangeBackground(Color.FromArgb(200, Color.Gold));
        _lastWords.Text.ChangeColor(Color.Black);
        _lastWords.Text.Text = "Thank You!\nYou had successfully finished the game, which is made to make people\nmore familiar with the values of European Union." +
            "\n\n\nGame is made by \"Mission Possible\" team." +
            "\n\nMain Idea      Ana Gigashwili" +
            "\nAnimations\t     Juliet Wdowiak" +
            "\nLevel Design\t   Oleksii Strokov" +
            "\nProgramming\t    Yahor Kaskevich";

        _lastWords.ToggleVisibility(false);
        #endregion

        _hint = new(name: "Level_7_hint", x: UIPositioner.CenterX - 400, y: UIPositioner.CenterY, width: 800, height: 150,
            text: "Press \"Q\" to rotate the Ring Clockwise\nPress \"E\" to rotate the Ring CounterClockwise",
            textX: 50, textY: 40, textSize: 26);

        #region Addin objects to RenderList

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _ring);
        RenderList.AddObject(1, _finalQuiz);
        RenderList.AddObject(2, _player.Body);
        RenderList.AddObject(3, _hint);
        RenderList.AddObject(3, _levelLastTaskWindow);
        RenderList.AddObject(-1, _lastWords);
        #endregion

        LastLevelHintPhrase = "\"Only a free heart knows which way is forward.\"";

        ReadData("..\\..\\..\\Resources\\Data\\Level_7.json");

        _preparedWord = _ring.GetCurrentWord();
        _finalQuiz.PrepareWord(_preparedWord);
        _finalQuiz.Text.Text += $"\n{_hints[_preparedWord]}";
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        for (int i = 0; i < _finalQuiz.LetterWindows.Count; i++)
        {
            if (Collider.DetectFullEnterCollision(_player.Hitbox, _finalQuiz.LetterWindowsHitboxes[i]))
                _finalQuiz.SelectLetterWindow(i);
            else if (_finalQuiz.LetterWindows[i].Background.BackgroundColor == Color.Yellow)
                _finalQuiz.DeselectLetterWindow(i);
        }

        if (_finalQuiz.IsLetterActivated) _finalQuiz.BlinkActive(timeRemaining);

        if (_finalQuiz.EnteredWord == _preparedWord && _preparedWord != "")
        {
            _ring.DisplayCurrent();
            _preparedWord = "";
            _finalQuiz.PrepareWord(_preparedWord);
        }

        if (_hint.IsVisible && _hintApearTime == -1)
        {
            _hintApearTime = timeRemaining;
        }
        if (_hint.IsVisible && timeRemaining - _hintApearTime >= 6000) _hint.ToggleVisibility(false);

        if (_lastWords.IsVisible && _lastWordsApearTime == -1)
        {
            _lastWordsApearTime = timeRemaining;
        }
        if (_lastWords.IsVisible && timeRemaining - _lastWordsApearTime >= 10000)
        {
            _nextState = States.InMainMenu;
        }

        if (_ring.TotalGuessed == _hints.Count)
        {
            _levelLastTaskWindow.ToggleVisibility(true);
            _finalQuiz.ToggleVisibility(false);
        }
    }

    public override void HandleUserInput()
    {
        if (InputHandler.NoKeyboardEvents) return;

        if (_finalQuiz.IsLetterSelected && InputHandler.LastKeyboardEvent.Key == Keys.T && !InputHandler.LastKeyboardEvent.IsHandled
          && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp && !_finalQuiz.IsLetterActivated)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _finalQuiz.ActivateSelected();
            _isPlayerCanMove = false;
        }
        if (_finalQuiz.IsLetterActivated && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            if (!(InputHandler.LastKeyboardEvent.IsHandled && InputHandler.LastKeyboardEvent.Key == Keys.T))
            {
                InputHandler.LastKeyboardEvent.IsHandled = true;
                _finalQuiz.ChangeLetter(InputHandler.LastKeyboardEvent.Key);
                _entered[_preparedWord] = _finalQuiz.EnteredWord;
                _isPlayerCanMove = true;
            }
        }

        if (_levelLastTaskWindow.IsVisible && InputHandler.LastKeyboardEvent.Key == Keys.Tab 
            && !InputHandler.LastKeyboardEvent.IsHandled && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            if (_levelLastTaskWindow.IsInEditMode)
            {
                _levelLastTaskWindow.StopEditingText();
                _isPlayerCanMove = true;
            }
            else
            {
                _levelLastTaskWindow.StartEditingText();
                _isPlayerCanMove = false;
            }
        }

        if (_levelLastTaskWindow.IsInEditMode && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp && !InputHandler.LastKeyboardEvent.IsHandled)
        {
            if (InputHandler.IsKeyAChar(InputHandler.LastKeyboardEvent.Key)) InputHandler.LastKeyboardEvent.IsHandled = true;
            _levelLastTaskWindow.AddLetter(InputHandler.LastKeyboardEvent.Key);
        }
        
        
        if (_levelLastTaskWindow.IsInEditMode && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp
            && !InputHandler.LastKeyboardEvent.IsHandled && InputHandler.LastKeyboardEvent.Key == Keys.Back)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _levelLastTaskWindow.RemoveLetter();
        }

        base.HandleUserInput();

        if (InputHandler.LastKeyboardEvent.IsHandled) return;

        if (InputHandler.LastKeyboardEvent.Key == Keys.E && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp
            && !_finalQuiz.IsLetterActivated && !_levelLastTaskWindow.IsInEditMode)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            OnRingRotated(_ring.RotateToAnotherSection(RotateDirection.Counterclockwise));
        }
        else if (InputHandler.LastKeyboardEvent.Key == Keys.Q && InputHandler.LastKeyboardEvent.EventType == GameUserEventType.KeyUp
            && !_finalQuiz.IsLetterActivated && !_levelLastTaskWindow.IsInEditMode)
        {
            InputHandler.LastKeyboardEvent.IsHandled = true;
            OnRingRotated(_ring.RotateToAnotherSection(RotateDirection.Clockwise));
        }
    }

    private void OnRingRotated(string word)
    {
        if (_preparedWord == word) return;

        _preparedWord = word;
        _finalQuiz.PrepareWord(_preparedWord);
        _finalQuiz.Text.Text = $"Guess The Word\n{_hints[_preparedWord]}";

        if (_entered[_preparedWord].Length > 0) _finalQuiz.UpdateStateToEntered(_entered[_preparedWord]);
    }

    protected override bool IsLevelTaskFulfilled()
    {
        return !_levelLastTaskWindow.IsInEditMode &&
            ( _levelLastTaskWindow.FieldText == "UNITY" ||
            _levelLastTaskWindow.FieldText == "RESPECT" ||
            _levelLastTaskWindow.FieldText == "HUMANITY" );
    }

    protected override void OnLevelTaskFulfilled()
    {
        _lastWords.ToggleVisibility(true);
    }
}
