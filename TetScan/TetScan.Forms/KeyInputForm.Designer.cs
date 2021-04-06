
namespace TetScan.TetScan.Forms
{
    partial class KeyInputForm
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
            this.leftButton = new System.Windows.Forms.Button();
            this.rightButton = new System.Windows.Forms.Button();
            this.DropButton = new System.Windows.Forms.Button();
            this.RotateButton = new System.Windows.Forms.Button();
            this.LeftText = new System.Windows.Forms.TextBox();
            this.RightText = new System.Windows.Forms.TextBox();
            this.DropText = new System.Windows.Forms.TextBox();
            this.RotateText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // leftButton
            // 
            this.leftButton.Location = new System.Drawing.Point(13, 13);
            this.leftButton.Name = "leftButton";
            this.leftButton.Size = new System.Drawing.Size(75, 23);
            this.leftButton.TabIndex = 0;
            this.leftButton.TabStop = false;
            this.leftButton.Text = "Left";
            this.leftButton.UseVisualStyleBackColor = true;
            this.leftButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // rightButton
            // 
            this.rightButton.Location = new System.Drawing.Point(12, 42);
            this.rightButton.Name = "rightButton";
            this.rightButton.Size = new System.Drawing.Size(75, 23);
            this.rightButton.TabIndex = 1;
            this.rightButton.TabStop = false;
            this.rightButton.Text = "Right";
            this.rightButton.UseVisualStyleBackColor = true;
            this.rightButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // DropButton
            // 
            this.DropButton.Location = new System.Drawing.Point(12, 68);
            this.DropButton.Name = "DropButton";
            this.DropButton.Size = new System.Drawing.Size(75, 23);
            this.DropButton.TabIndex = 3;
            this.DropButton.TabStop = false;
            this.DropButton.Text = "Drop";
            this.DropButton.UseVisualStyleBackColor = true;
            this.DropButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // RotateButton
            // 
            this.RotateButton.Location = new System.Drawing.Point(13, 97);
            this.RotateButton.Name = "RotateButton";
            this.RotateButton.Size = new System.Drawing.Size(75, 23);
            this.RotateButton.TabIndex = 4;
            this.RotateButton.TabStop = false;
            this.RotateButton.Text = "Rotate";
            this.RotateButton.UseVisualStyleBackColor = true;
            this.RotateButton.Click += new System.EventHandler(this.Button_Click);
            // 
            // LeftText
            // 
            this.LeftText.Location = new System.Drawing.Point(95, 15);
            this.LeftText.Name = "LeftText";
            this.LeftText.ReadOnly = true;
            this.LeftText.Size = new System.Drawing.Size(122, 20);
            this.LeftText.TabIndex = 6;
            // 
            // RightText
            // 
            this.RightText.Location = new System.Drawing.Point(95, 44);
            this.RightText.Name = "RightText";
            this.RightText.ReadOnly = true;
            this.RightText.Size = new System.Drawing.Size(122, 20);
            this.RightText.TabIndex = 7;
            // 
            // DropText
            // 
            this.DropText.Location = new System.Drawing.Point(95, 70);
            this.DropText.Name = "DropText";
            this.DropText.ReadOnly = true;
            this.DropText.Size = new System.Drawing.Size(122, 20);
            this.DropText.TabIndex = 9;
            // 
            // RotateText
            // 
            this.RotateText.Location = new System.Drawing.Point(95, 99);
            this.RotateText.Name = "RotateText";
            this.RotateText.ReadOnly = true;
            this.RotateText.Size = new System.Drawing.Size(122, 20);
            this.RotateText.TabIndex = 10;
            // 
            // KeyInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(229, 131);
            this.Controls.Add(this.RotateText);
            this.Controls.Add(this.DropText);
            this.Controls.Add(this.RightText);
            this.Controls.Add(this.LeftText);
            this.Controls.Add(this.RotateButton);
            this.Controls.Add(this.DropButton);
            this.Controls.Add(this.rightButton);
            this.Controls.Add(this.leftButton);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyInputForm";
            this.Text = "Key Configuration";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyInputForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeyInputForm_KeyDown);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.KeyInputForm_PreviewKeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button leftButton;
        private System.Windows.Forms.Button rightButton;
        private System.Windows.Forms.Button DropButton;
        private System.Windows.Forms.Button RotateButton;
        private System.Windows.Forms.TextBox LeftText;
        private System.Windows.Forms.TextBox RightText;
        private System.Windows.Forms.TextBox DropText;
        private System.Windows.Forms.TextBox RotateText;
    }
}