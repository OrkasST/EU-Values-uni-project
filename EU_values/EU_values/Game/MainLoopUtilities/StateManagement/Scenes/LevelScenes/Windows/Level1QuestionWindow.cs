using EU_values.Game.UI.Elements;
using EU_values.Utilities;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace EU_values.Game.MainLoopUtilities.StateManagement.Scenes.LevelScenes.Windows;

public class Level1QuestionWindow: GameWindow
{
    public GameUIButton AcceptButton { get; private set; }
    public GameUIButton QuestionButton { get; private set; }
    public GameUIText Description { get; private set; }

    public Level1QuestionWindow(string question) : base(name: "Level_1_Question", x: UIPositioner.CenterX - 500, y: UIPositioner.CenterY - 300, width: 1000, height: 500,
            text: question,
            textX: 500, textY: 250, textSize: 40, isCameraAffected: false)
    {
        Text.ChangeTextAlignment(TextPositioning.Center);
        Text.ChangeColor(Color.Gold);
        Background.ChangeBackground(Color.FromArgb(230, 41, 21, 46));

        Description = new(name: "Level_1_Question_Description", x: Background.Position.X + 500, y: Background.Position.Y + 100,
            text: "Description", fontFamily: "Times New Roman", fontSize: 36, Color.White);

        Description.ChangeTextAlignment(TextPositioning.Center);

        AcceptButton = new("Level_1_Question_YesButton",
            x: Background.Position.X + 90, y: Background.Position.Y + 350, width: 310, height: 55,
            text: "Accept the Judgement",
            textX: 155, textY: 10, textSize: 30);
        AcceptButton.ChangeTextAlignment(TextPositioning.Center);

        QuestionButton = new("Level_1_Question_NoButton",
            x: Background.Position.X + 600, y: Background.Position.Y + 350, width: 310, height: 55,
            text: "Question the Judgement",
            textX: 155, textY: 10, textSize: 30);
        QuestionButton.ChangeTextAlignment(TextPositioning.Center);

        AddObject(Description, 1);
        AddObject(AcceptButton, 1);
        AddObject(QuestionButton, 1);

        ToggleVisibility(false);
    }

    public void SetDescription(string description) => Description.Text = $"\"{description}\"";
}
