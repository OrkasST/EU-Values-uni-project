using EU_values.Game.MainLoopUtilities.Physics;
using EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;
using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes;

public class Level5Scene : InGameScene
{
    private Level5PhraseWindow _phraseWindow;
    private GameWindow _levelHint;
    private int _hintApearTime = -1;

    public Level5Scene(GameLevels previousLevel) : base(previousLevel)
    {
        _background = new Box("Level_5_Background", 0, 0, 2361, 1500, Image.FromFile("..\\..\\..\\Resources\\Images\\Levels\\Level_5\\Level_5_Background.png"), true);
        _phraseWindow = new(x: 100, y: 100);

        _levelHint = new(name: "Level_5_Hint", x: UIPositioner.CenterX - 200, y: UIPositioner.CenterY - 100, width: 400, height: 150,
            text: "Press \"E\"\nto exclude/include the phrase",
            textX: 200, textY: 30, textSize: 26, isCameraAffected: true);
        _levelHint.Background.ChangeBackground(Color.Black);
        _levelHint.Text.ChangeTextAlignment(TextPositioning.Center);
        _levelHint.ToggleVisibility();

        RenderList.AddObject(0, _background);
        RenderList.AddObject(1, _phraseWindow);
        RenderList.AddObject(2, _player.Body);
        RenderList.AddObject(3, _levelHint);

        LastLevelHintPhrase = "\"Strength lies in rules that bend for no one.\"";
        FinalPhraseText = "\"Law applies to all. No exceptions.\"";

        ReadData("..\\..\\..\\Resources\\Data\\Level_5.json");
        if (_tiledMap != null)
            foreach (var layer in _tiledMap.Layers)
            {
                if (layer.Name == "PhrasesWindow" && layer.Objects != null)
                {
                    _phraseWindow.ChangePosition(layer.Objects[0].X, layer.Objects[0].Y);
                    break;
                }
            }

        SetupDoors(0);

        _player.SetJumpSpeed(410);
    }

    public override void Update(float timeDelta, int timeRemaining, int timeDifference)
    {
        base.Update(timeDelta, timeRemaining, timeDifference);

        if (!_levelHint.Background.IsVisible && _hintApearTime == -1)
        {
            _levelHint.ToggleVisibility();
            _hintApearTime = timeRemaining;
        }
        if (_levelHint.IsVisible && timeRemaining - _hintApearTime >= 6000) _levelHint.ToggleVisibility(false);

        for (int i = 0; i < _phraseWindow.PhrasesVisualization.Count; i++)
        {
            if (Collider.DetectFullEnterCollision(new PointF(_player.Body.Position.X + _player.Body.Size.Width / 2, _player.Body.Position.Y + _player.Body.Size.Height / 2),
                _phraseWindow.PhrasesHitboxes[i]))
            {
                _phraseWindow.SelectPhrase(i);
            }
            else if (_phraseWindow.SelectedPhrase == i)
            {
                _phraseWindow.DeselectPhrase(i);
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
            InputHandler.LastKeyboardEvent.IsHandled = true;
            _phraseWindow.ExcludePhrase();
        }
    }

    protected override bool IsLevelTaskFulfilled() => _phraseWindow.ArePhrasesAccepted();
}
