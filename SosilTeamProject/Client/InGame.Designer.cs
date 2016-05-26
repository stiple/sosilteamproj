namespace Client
{
    partial class InGame
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
            this.ExitButton = new System.Windows.Forms.Button();
            this.WriteTextBox = new System.Windows.Forms.TextBox();
            this.ChatBox = new System.Windows.Forms.TextBox();
            this.EnterButton = new System.Windows.Forms.Button();
            this.UserNameList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(772, 12);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(182, 26);
            this.ExitButton.TabIndex = 0;
            this.ExitButton.Text = "나가기";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // WriteTextBox
            // 
            this.WriteTextBox.Location = new System.Drawing.Point(13, 508);
            this.WriteTextBox.Name = "WriteTextBox";
            this.WriteTextBox.Size = new System.Drawing.Size(753, 21);
            this.WriteTextBox.TabIndex = 1;
            this.WriteTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WriteTextBox_KeyDown);
            // 
            // ChatBox
            // 
            this.ChatBox.Location = new System.Drawing.Point(201, 425);
            this.ChatBox.Multiline = true;
            this.ChatBox.Name = "ChatBox";
            this.ChatBox.Size = new System.Drawing.Size(753, 74);
            this.ChatBox.TabIndex = 2;
            // 
            // EnterButton
            // 
            this.EnterButton.Location = new System.Drawing.Point(773, 505);
            this.EnterButton.Name = "EnterButton";
            this.EnterButton.Size = new System.Drawing.Size(181, 23);
            this.EnterButton.TabIndex = 3;
            this.EnterButton.Text = "전송";
            this.EnterButton.UseVisualStyleBackColor = true;
            this.EnterButton.Click += new System.EventHandler(this.EnterButton_Click);
            // 
            // UserNameList
            // 
            this.UserNameList.FormattingEnabled = true;
            this.UserNameList.ItemHeight = 12;
            this.UserNameList.Location = new System.Drawing.Point(12, 426);
            this.UserNameList.Name = "UserNameList";
            this.UserNameList.Size = new System.Drawing.Size(181, 76);
            this.UserNameList.TabIndex = 4;
            // 
            // InGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(966, 542);
            this.ControlBox = false;
            this.Controls.Add(this.UserNameList);
            this.Controls.Add(this.EnterButton);
            this.Controls.Add(this.ChatBox);
            this.Controls.Add(this.WriteTextBox);
            this.Controls.Add(this.ExitButton);
            this.Name = "InGame";
            this.Text = "InGame";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InGame_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.TextBox WriteTextBox;
        private System.Windows.Forms.TextBox ChatBox;
        private System.Windows.Forms.Button EnterButton;
        private System.Windows.Forms.ListBox UserNameList;
    }
}