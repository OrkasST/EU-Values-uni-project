using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using System.Windows.Forms;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class WordEnterWindow : GameWindow
{
    public List<GameWindow> LetterWindows = [];
    public List<Hitbox> LetterWindowsHitboxes = [];

    private Color _letterDefaultTextColor = Color.White;
    private Color _letterDefaultBackgroundColor = Color.FromArgb(180, Color.Black);

    private int _selectedLetter = -1;
    public bool IsLetterActivated = false;
    public bool IsLetterSelected { get; private set; } = false;
    public bool IsWordFinished { get; private set; } = false;

    private float _lastToggleTime = -1;
    private float _blinkPeriod = 500;

    private string _normalText;

    public string EnteredWord = "";
    private Dictionary<int, bool> _letterStatus = [];
    private string _guessedWord = "";

    private static string _imagesLocation = "..\\..\\..\\Resources\\Images\\Letters\\Letter_plate_";
    protected static Dictionary<string, Image> CorrectLetters = new()
    {
        ["A"] = Image.FromFile($"{_imagesLocation}A.png"),
        ["B"] = Image.FromFile($"{_imagesLocation}B.png"),
        ["C"] = Image.FromFile($"{_imagesLocation}C.png"),
        ["D"] = Image.FromFile($"{_imagesLocation}D.png"),
        ["E"] = Image.FromFile($"{_imagesLocation}E.png"),
        ["G"] = Image.FromFile($"{_imagesLocation}G.png"),
        ["H"] = Image.FromFile($"{_imagesLocation}H.png"),
        ["I"] = Image.FromFile($"{_imagesLocation}I.png"),
        ["L"] = Image.FromFile($"{_imagesLocation}L.png"),
        ["N"] = Image.FromFile($"{_imagesLocation}N.png"),
        ["O"] = Image.FromFile($"{_imagesLocation}O.png"),
        ["Q"] = Image.FromFile($"{_imagesLocation}Q.png"),
        ["R"] = Image.FromFile($"{_imagesLocation}R.png"),
        ["T"] = Image.FromFile($"{_imagesLocation}T.png"),
        ["U"] = Image.FromFile($"{_imagesLocation}U.png"),
        ["V"] = Image.FromFile($"{_imagesLocation}V.png"),
        ["W"] = Image.FromFile($"{_imagesLocation}W.png"),
        ["Y"] = Image.FromFile($"{_imagesLocation}Y.png"),
        ["F"] = Image.FromFile($"{_imagesLocation}F.png"),
        ["P"] = Image.FromFile($"{_imagesLocation}P.png"),
        ["S"] = Image.FromFile($"{_imagesLocation}S.png"),
        ["M"] = Image.FromFile($"{_imagesLocation}M.png"),
    };

    public WordEnterWindow(string name, float x, float y, float width, string text) : base(name, x, y, width, height: 400,
        text: "", textX: width / 2, textY: 30, textSize: 32, isCameraAffected: true)
    {
        _normalText = text;
        Text.Text = _normalText;
        Text.ChangeTextAlignment(TextPositioning.Center);

        Background.ChangeBackground(Color.FromArgb(0, 0, 0, 0));
    }

    public void PrepareWord(string word)
    {
        var xOffset = 74;
        var startXPosition = Position.X + Size.Width / 2 - word.Length / 2f * xOffset;

        EnteredWord = "";
        IsWordFinished = false;
        _guessedWord = word;

        LetterWindowsHitboxes = new();
        RenderList.RemoveObjectRange(1, 0, LetterWindows.Count);
        LetterWindows = new();
        _letterStatus = new();

        for (int i = 0; i < word.Length; i++)
        {
            if ($"{word[i]}" == " ")
            {
                EnteredWord += " ";
                _letterStatus.Add(i, true);
            }
            else
            {
                EnteredWord += "*";
                _letterStatus.Add(i, false);
            }

            LetterWindows.Add(new GameWindow(
            name: $"Letter_{word[i]}_window", x: startXPosition + xOffset * i, y: Position.Y + 180, width: 64, height: 64,
                text: "", textX: 32, textY: 7, textSize: 50, IsCameraAffected));
            LetterWindows.Last().Text.ChangeTextAlignment(TextPositioning.Center);

            AddObject(LetterWindows.Last(), 1);

            LetterWindowsHitboxes.Add(new(x: LetterWindows.Last().Position.X, y: LetterWindows.Last().Position.Y,
                width: 64, height: Position.Y + Size.Height - LetterWindows.Last().Position.Y, id: LetterWindows.Last().Name));

            if ($"{word[i]}" == " ")
            {
                LetterWindows.Last().ToggleVisibility(false);
                LetterWindowsHitboxes.Last().IsCollidable = false;
            }

        }
    }

    public void SelectLetterWindow(int ind)
    {
        if (IsWordFinished || IsLetterActivated ) return;
        if (_letterStatus[ind]) return;

        ChangeLetterWindowColors(ind, Color.Yellow, Color.Black);
        _selectedLetter = ind;
        IsLetterSelected = true;
    }

    public void DeselectLetterWindow(int ind)
    {
        if (IsWordFinished || IsLetterActivated) return;
        if (_letterStatus[ind]) return;

        ChangeLetterWindowColors(ind, _letterDefaultBackgroundColor, _letterDefaultTextColor);
        _selectedLetter = -1;
        IsLetterSelected = false;
    }

    private void ChangeLetterWindowColors(int ind, Color bgcolor, Color textcolor)
    {
        LetterWindows[ind].Background.ChangeBackground(bgcolor);
        LetterWindows[ind].Text.ChangeColor(textcolor);
    }

    public void ActivateSelected()
    {
        if (IsWordFinished) return;

        ChangeLetterWindowColors(_selectedLetter, Color.White, Color.Brown);
        IsLetterActivated = true;
        Text.Text = $"{_normalText}\nType a letter...";
    }

    public void ChangeLetter(Keys key)
    {
        if (!InputHandler.IsKeyAChar(key) || IsWordFinished || _selectedLetter < 0) return;

        LetterWindows[_selectedLetter].Text.ChangeColor(Color.Black);
        IsLetterActivated = false;
        LetterWindows[_selectedLetter].Text.Text = $"{key}";
        LetterWindows[_selectedLetter].Text.ToggleVisibility(true);
        Text.Text = _normalText;

        EnteredWord = $"{EnteredWord.Substring(0, _selectedLetter)}{key}{EnteredWord.Substring(_selectedLetter + 1)}";

        if (EnteredWord[_selectedLetter] == _guessedWord[_selectedLetter])
        {
            OnCorrectLetter(_selectedLetter);
            _selectedLetter = -1;
            IsLetterSelected = false;
        }
    }

    public void UpdateStateToEntered(string entered)
    {
        EnteredWord = entered;

        for (int i = 0; i < entered.Length; i++)
        {
            if ($"{entered[i]}" == "*" || $"{entered[i]}" == " ") continue;

            LetterWindows[i].Text.Text = $"{entered[i]}";

            if (EnteredWord[i] == _guessedWord[i])
            {
                OnCorrectLetter(i);
            }
        }
    }

    public void BlinkActive(float timeRemaining)
    {
        if (_selectedLetter < 0) return;
        if (timeRemaining - _lastToggleTime >= _blinkPeriod)
        {
            _lastToggleTime = timeRemaining;
            LetterWindows[_selectedLetter].Text.ToggleVisibility();
        }
    }

    public void OnCorrectWord() {
        _selectedLetter = -1;
        IsLetterActivated = false;
        IsLetterSelected = false;
        IsWordFinished = true;
    }

    public void OnCorrectLetter(int ind)
    {
        _letterStatus[ind] = true;
        LetterWindows[ind].Background.ChangeBackground(CorrectLetters[LetterWindows[ind].Text.Text]);
        LetterWindows[ind].Text.Text = "";
        LetterWindows[ind].Text.ToggleVisibility(false);
    }
}
