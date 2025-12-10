namespace EU_values
{
    public partial class MainForm : Form
    {
        private Utilities.FullScreen fullScreen = new Utilities.FullScreen();
        public MainForm()
        {
            InitializeComponent();

            fullScreen.EnterFullScreenMode(this);
        }
    }
}
