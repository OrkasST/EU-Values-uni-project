using EU_values.Game;
using EU_values.Utilities;
using EU_values.Utilities.Events;

namespace EU_values;

public partial class MainForm : Form
{
    private GameManager gameManager = new GameManager();
    public MainForm()
    {
        InitializeComponent();
    }

    private void LoopTimer_Tick(object sender, EventArgs e)
    {
        gameManager.UpdateGameState(this);
        this.Refresh();
    }

    private void MainForm_Paint(object sender, PaintEventArgs e)
    {
        gameManager.RenderGameObjects(e.Graphics);
    }

    private void MainForm_KeyUp(object sender, KeyEventArgs e)
    {
        InputHandler.AddEvent(new GameKeyboardEvent(GameUserEventType.KeyUp, e.KeyCode));
    }
    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        InputHandler.AddEvent(new GameKeyboardEvent(GameUserEventType.KeyDown, e.KeyCode));
    }



    private void MainForm_MouseDown(object sender, MouseEventArgs e)
    {
        InputHandler.AddEvent(new GameMouseEvent(GameUserEventType.MouseMove, e.Location, e.Button));
    }
    private void MainForm_MouseUp(object sender, MouseEventArgs e)
    {
        InputHandler.AddEvent(new GameMouseEvent(GameUserEventType.MouseMove, e.Location, e.Button));
    }
    private void MainForm_MouseMove(object sender, MouseEventArgs e)
    {
        InputHandler.AddEvent(new GameMouseEvent(GameUserEventType.MouseMove, e.Location, e.Button));
    }
}
