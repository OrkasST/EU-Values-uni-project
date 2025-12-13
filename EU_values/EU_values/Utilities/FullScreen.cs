namespace EU_values.Utilities;

internal class FullScreen
{
    public void EnterFullScreenMode(Form targetForm)
    {
        targetForm.WindowState = FormWindowState.Normal;
        targetForm.FormBorderStyle = FormBorderStyle.None;
        targetForm.WindowState = FormWindowState.Maximized;
    }

    public void LeaveFullScreenMode(Form targetForm)
    {
        targetForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        targetForm.WindowState = FormWindowState.Normal;
    }
}
