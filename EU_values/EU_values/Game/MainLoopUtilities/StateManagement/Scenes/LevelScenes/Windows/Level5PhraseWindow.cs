using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.UI.Elements;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class Level5PhraseWindow : GameWindow
{
    private string[] _acceptedPhrases = { "The law protects all citizens equally,", "unless they hold high office,", "and guarantees justice for everyone,",
       "except during emergencies,", "regardless of wealth and status,", "unless a donation was made,", "and applies at all times,", "except on special occasions" };

    private List<string> _currentPhrases = new() { "The law protects all citizens equally,", "unless they are important", "unless they hold high office,",
        "and guarantees justice for everyone,", "except the wealthy", "except during emergencies,", "regardless of wealth and status,", "unless a donation was made,",
        "not applicable to officials", "and applies at all times,", "temporarily suspended", "except on special occasions", "for national security" };

    public List<Level5Phrase> PhrasesVisualization = [];
    public List<Hitbox> PhrasesHitboxes = [];

    private static int _textSize = 40;
    private static int _windowWidth = 1500;

    public int SelectedPhrase { get; private set; } = -1;
    public bool IsPhraseSelected { get; private set; } = false;

    public Level5PhraseWindow(float x, float y)
        : base("Level5PhraseWindow", x, y, width: _windowWidth, height: 1100, text: "", textX: 500, textY: 100, textSize: _textSize, isCameraAffected: true)
    {
        int i = 0;
        int offsetX = 100;
        int offsetY = 100;

        foreach (var phrase in _currentPhrases)
        {
            if (i > 0)
            {
                if (offsetX + PhrasesVisualization[i-1].Phrase.Text.Length * _textSize/2 + phrase.Length * _textSize/2 > _windowWidth)
                {
                    offsetY += _textSize + 140;
                    offsetX = 100;
                } else
                {
                    offsetX += PhrasesVisualization[i - 1].Phrase.Text.Length * _textSize/2;
                }
            }
            var oox = x + offsetX;
            var ooy = y + offsetY;

            PhrasesVisualization.Add(new(x: x+offsetX, y: y+offsetY, text: phrase, fontSize: _textSize));
            PhrasesVisualization.Last().Phrase.ChangeColor(Color.Gold);
            AddObject(PhrasesVisualization.Last().Phrase, 1);

            PhrasesHitboxes.Add(new(x: x + offsetX, y: y + offsetY, width: (_textSize / 2.3f) * phrase.Length, height: 160, id: phrase));

            i++;
        }
    }
    public void SelectPhrase(int ind)
    {
        PhrasesVisualization[ind].Phrase.ChangeColor(PhrasesVisualization[ind].IsExcluded == true ? Color.LightBlue : Color.White);
        SelectedPhrase = ind;
        IsPhraseSelected = true;
    }

    public void DeselectPhrase(int ind)
    {
        PhrasesVisualization[ind].Phrase.ChangeColor(PhrasesVisualization[ind].IsExcluded == true ? Color.Gray : Color.Gold);
        SelectedPhrase = -1;
        IsPhraseSelected = false;
    }

    public void ExcludePhrase()
    {
        if (SelectedPhrase == -1) return;
        if (PhrasesVisualization[SelectedPhrase].IsExcluded)
        {
            AddPhrase();
            return;
        }
        _currentPhrases[SelectedPhrase] = "";
        PhrasesVisualization[SelectedPhrase].Phrase.ChangeColor(Color.Gray);
        PhrasesVisualization[SelectedPhrase].IsExcluded = true;
    }
    public void AddPhrase()
    {
        if (SelectedPhrase == -1) return;
        _currentPhrases[SelectedPhrase] = PhrasesVisualization[SelectedPhrase].Phrase.Text;
        PhrasesVisualization[SelectedPhrase].Phrase.ChangeColor(Color.Gold);
        PhrasesVisualization[SelectedPhrase].IsExcluded = false;
    }

    public bool ArePhrasesAccepted()
    {
        int acceptedCount = 0;

        for (int i = 0; i < _currentPhrases.Count; i++)
        {
            if (_currentPhrases[i] == "") continue;

            bool isPhraseAccepted = false;

            for (int j = 0; j < _acceptedPhrases.Length; j++)
            {
                if (_currentPhrases[i] == _acceptedPhrases[j])
                {
                    isPhraseAccepted = true;
                    acceptedCount++;
                    break;
                }
            }
            if (!isPhraseAccepted) return false;
        }
        return acceptedCount == _acceptedPhrases.Length;
    }

    public override void ChangePosition(float x, float y)
    {
        base.ChangePosition(x, y);
        for (int i = 0; i < PhrasesHitboxes.Count; i++)
        {
            PhrasesHitboxes[i].Update(PhrasesVisualization[i].Phrase.Position.X, PhrasesVisualization[i].Phrase.Position.Y);
        }
    }

}
