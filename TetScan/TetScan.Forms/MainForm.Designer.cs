namespace TetScan
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.regionSelectButton = new System.Windows.Forms.Button();
            this.inputConfigButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.regionSelectLabel = new System.Windows.Forms.Label();
            this.inputConfigLabel = new System.Windows.Forms.Label();
            this.currentState = new System.Windows.Forms.Label();
            this.logger = new System.Windows.Forms.TextBox();
            this.ghostPieceCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // regionSelectButton
            // 
            this.regionSelectButton.Location = new System.Drawing.Point(12, 26);
            this.regionSelectButton.Name = "regionSelectButton";
            this.regionSelectButton.Size = new System.Drawing.Size(75, 23);
            this.regionSelectButton.TabIndex = 2;
            this.regionSelectButton.Text = "Select";
            this.regionSelectButton.UseVisualStyleBackColor = true;
            this.regionSelectButton.Click += new System.EventHandler(this.regionSelectButton_Click);
            this.regionSelectButton.MouseEnter += new System.EventHandler(this.regionSelectButton_MouseEnter);
            // 
            // inputConfigButton
            // 
            this.inputConfigButton.Location = new System.Drawing.Point(12, 81);
            this.inputConfigButton.Name = "inputConfigButton";
            this.inputConfigButton.Size = new System.Drawing.Size(75, 23);
            this.inputConfigButton.TabIndex = 3;
            this.inputConfigButton.Text = "Configure";
            this.inputConfigButton.UseVisualStyleBackColor = true;
            this.inputConfigButton.Click += new System.EventHandler(this.inputConfigButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(221, 128);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 4;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // regionSelectLabel
            // 
            this.regionSelectLabel.AutoSize = true;
            this.regionSelectLabel.Location = new System.Drawing.Point(11, 10);
            this.regionSelectLabel.Name = "regionSelectLabel";
            this.regionSelectLabel.Size = new System.Drawing.Size(41, 13);
            this.regionSelectLabel.TabIndex = 5;
            this.regionSelectLabel.Text = "Region";
            // 
            // inputConfigLabel
            // 
            this.inputConfigLabel.AutoSize = true;
            this.inputConfigLabel.Location = new System.Drawing.Point(11, 65);
            this.inputConfigLabel.Name = "inputConfigLabel";
            this.inputConfigLabel.Size = new System.Drawing.Size(79, 13);
            this.inputConfigLabel.TabIndex = 6;
            this.inputConfigLabel.Text = "Keyboard Input";
            // 
            // currentState
            // 
            this.currentState.AutoSize = true;
            this.currentState.Location = new System.Drawing.Point(12, 133);
            this.currentState.Name = "currentState";
            this.currentState.Size = new System.Drawing.Size(0, 13);
            this.currentState.TabIndex = 7;
            // 
            // logger
            // 
            this.logger.Location = new System.Drawing.Point(12, 168);
            this.logger.Multiline = true;
            this.logger.Name = "logger";
            this.logger.ReadOnly = true;
            this.logger.Size = new System.Drawing.Size(286, 155);
            this.logger.TabIndex = 12;
            // 
            // ghostPieceCheck
            // 
            this.ghostPieceCheck.AutoSize = true;
            this.ghostPieceCheck.Checked = true;
            this.ghostPieceCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ghostPieceCheck.Location = new System.Drawing.Point(214, 10);
            this.ghostPieceCheck.Name = "ghostPieceCheck";
            this.ghostPieceCheck.Size = new System.Drawing.Size(84, 17);
            this.ghostPieceCheck.TabIndex = 13;
            this.ghostPieceCheck.Text = "Ghost Piece";
            this.ghostPieceCheck.UseVisualStyleBackColor = true;
            this.ghostPieceCheck.CheckedChanged += new System.EventHandler(this.ghostPieceCheck_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 335);
            this.Controls.Add(this.ghostPieceCheck);
            this.Controls.Add(this.logger);
            this.Controls.Add(this.currentState);
            this.Controls.Add(this.inputConfigLabel);
            this.Controls.Add(this.regionSelectLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.inputConfigButton);
            this.Controls.Add(this.regionSelectButton);
            this.Name = "MainForm";
            this.Text = "TetScan";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MainApp_Load);
            this.Shown += new System.EventHandler(this.MainApp_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label currentState;

        private System.Windows.Forms.Label regionSelectLabel;
        private System.Windows.Forms.Button regionSelectButton;

        private System.Windows.Forms.Label inputConfigLabel;
        private System.Windows.Forms.Button inputConfigButton;

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.TextBox logger;
        private System.Windows.Forms.CheckBox ghostPieceCheck;
    }
}

