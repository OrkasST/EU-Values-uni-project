namespace EU_values
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            LoopTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // LoopTimer
            // 
            LoopTimer.Enabled = true;
            LoopTimer.Interval = 10;
            LoopTimer.Tick += LoopTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            DoubleBuffered = true;
            Location = new Point(1000, 100);
            Name = "MainForm";
            Text = "EU Values";
            WindowState = FormWindowState.Maximized;
            Paint += MainForm_Paint;
            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;
            MouseDown += MainForm_MouseDown;
            MouseMove += MainForm_MouseMove;
            MouseUp += MainForm_MouseUp;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer LoopTimer;
    }
}
