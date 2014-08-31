namespace TestOnline
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.ConsoleContent = new System.Windows.Forms.RichTextBox();
            this.start = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
            this.backgroundLoader = new System.ComponentModel.BackgroundWorker();
            this.startStreamingWorker = new System.ComponentModel.BackgroundWorker();
            this.playNextFileWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // ConsoleContent
            // 
            this.ConsoleContent.Location = new System.Drawing.Point(13, 13);
            this.ConsoleContent.Name = "ConsoleContent";
            this.ConsoleContent.ReadOnly = true;
            this.ConsoleContent.Size = new System.Drawing.Size(769, 404);
            this.ConsoleContent.TabIndex = 0;
            this.ConsoleContent.Text = "";
            // 
            // start
            // 
            this.start.Enabled = false;
            this.start.Location = new System.Drawing.Point(706, 426);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 1;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.start_Click);
            // 
            // stop
            // 
            this.stop.Enabled = false;
            this.stop.Location = new System.Drawing.Point(625, 426);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(75, 23);
            this.stop.TabIndex = 2;
            this.stop.Text = "Stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.stop_Click);
            // 
            // backgroundLoader
            // 
            this.backgroundLoader.WorkerReportsProgress = true;
            this.backgroundLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundLoader_DoWork);
            this.backgroundLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundLoader_RunWorkerCompleted);
            // 
            // startStreamingWorker
            // 
            this.startStreamingWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.startStreamingWorker_DoWork);
            this.startStreamingWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RunWorkerCompleted);
            // 
            // playNextFileWorker
            // 
            this.playNextFileWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.playNextFileWorker_DoWork);
            this.playNextFileWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 461);
            this.Controls.Add(this.stop);
            this.Controls.Add(this.start);
            this.Controls.Add(this.ConsoleContent);
            this.Name = "MainForm";
            this.Text = "Taurons Test Streaming Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ConsoleContent;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.Button stop;
        private System.ComponentModel.BackgroundWorker backgroundLoader;
        private System.ComponentModel.BackgroundWorker startStreamingWorker;
        private System.ComponentModel.BackgroundWorker playNextFileWorker;
    }
}

