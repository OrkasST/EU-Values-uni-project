using EU_values.Game.UI.Elements;
using EU_values.Utilities;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class HintProposalWindow: GameWindow
{
    public GameUIButton YesHintButton { get; private set; }
    public GameUIButton NoHintButton { get; private set; }

    public bool IsProposed = false;

    public HintProposalWindow() : base(name: "ProposeHintToLastLevelWindow", x: UIPositioner.CenterX - 400, y: UIPositioner.CenterY - 100, width: 800, height: 180,
            text: "Do you want to see the hint for the last level?",
            textX: 400, textY: 30, textSize: 30)
    {
        Text.ChangeTextAlignment(TextPositioning.Center);

        YesHintButton = new GameUIButton("HintProposal_YesButton",
           x: Background.Position.X + 120, y: Background.Position.Y + 100, width: 200, height: 36,
           text: "See the hint",
           textX: 100, textY: 5, textSize: 26);
        YesHintButton.ChangeTextAlignment(TextPositioning.Center);

        NoHintButton = new GameUIButton("HintProposal_NoButton",
            x: Background.Position.X + 480, y: Background.Position.Y + 100, width: 200, height: 36,
            text: "Skip the hint",
            textX: 100, textY: 5, textSize: 26);
        NoHintButton.ChangeTextAlignment(TextPositioning.Center);

        AddObject(YesHintButton, 1);
        AddObject(NoHintButton, 1);

        ToggleVisibility(false);
    }
}
