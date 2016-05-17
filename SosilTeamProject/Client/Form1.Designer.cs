namespace Client
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.IdTextBox = new System.Windows.Forms.TextBox();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.LoginButton = new System.Windows.Forms.Button();
            this.ChatTextBox = new System.Windows.Forms.TextBox();
            this.WriteChatBox = new System.Windows.Forms.TextBox();
            this.UserList = new System.Windows.Forms.ListBox();
            this.SignInButton = new System.Windows.Forms.Button();
            this.UserNicknameText = new System.Windows.Forms.Label();
            this.UserNumofGameText = new System.Windows.Forms.Label();
            this.UserNumof1stText = new System.Windows.Forms.Label();
            this.SendTextButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // IdTextBox
            // 
            this.IdTextBox.Location = new System.Drawing.Point(34, 15);
            this.IdTextBox.Name = "IdTextBox";
            this.IdTextBox.Size = new System.Drawing.Size(100, 21);
            this.IdTextBox.TabIndex = 0;
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(208, 15);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PasswordChar = '*';
            this.PasswordTextBox.Size = new System.Drawing.Size(100, 21);
            this.PasswordTextBox.TabIndex = 1;
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(314, 15);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(75, 23);
            this.LoginButton.TabIndex = 2;
            this.LoginButton.Text = "LogIn";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // ChatTextBox
            // 
            this.ChatTextBox.Enabled = false;
            this.ChatTextBox.Location = new System.Drawing.Point(12, 467);
            this.ChatTextBox.Multiline = true;
            this.ChatTextBox.Name = "ChatTextBox";
            this.ChatTextBox.Size = new System.Drawing.Size(797, 124);
            this.ChatTextBox.TabIndex = 3;
            // 
            // WriteChatBox
            // 
            this.WriteChatBox.Location = new System.Drawing.Point(12, 597);
            this.WriteChatBox.Name = "WriteChatBox";
            this.WriteChatBox.Size = new System.Drawing.Size(714, 21);
            this.WriteChatBox.TabIndex = 4;
            this.WriteChatBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.WriteChatBox_KeyDown);
            // 
            // UserList
            // 
            this.UserList.FormattingEnabled = true;
            this.UserList.ItemHeight = 12;
            this.UserList.Location = new System.Drawing.Point(815, 12);
            this.UserList.Name = "UserList";
            this.UserList.Size = new System.Drawing.Size(120, 436);
            this.UserList.TabIndex = 5;
            // 
            // SignInButton
            // 
            this.SignInButton.Location = new System.Drawing.Point(395, 15);
            this.SignInButton.Name = "SignInButton";
            this.SignInButton.Size = new System.Drawing.Size(75, 23);
            this.SignInButton.TabIndex = 6;
            this.SignInButton.Text = "Sign In";
            this.SignInButton.UseVisualStyleBackColor = true;
            this.SignInButton.Click += new System.EventHandler(this.SignInButton_Click);
            // 
            // UserNicknameText
            // 
            this.UserNicknameText.AutoSize = true;
            this.UserNicknameText.Font = new System.Drawing.Font("굴림", 11F);
            this.UserNicknameText.Location = new System.Drawing.Point(815, 477);
            this.UserNicknameText.Name = "UserNicknameText";
            this.UserNicknameText.Size = new System.Drawing.Size(62, 15);
            this.UserNicknameText.TabIndex = 7;
            this.UserNicknameText.Text = "닉네임: ";
            // 
            // UserNumofGameText
            // 
            this.UserNumofGameText.AutoSize = true;
            this.UserNumofGameText.Font = new System.Drawing.Font("굴림", 11F);
            this.UserNumofGameText.Location = new System.Drawing.Point(815, 521);
            this.UserNumofGameText.Name = "UserNumofGameText";
            this.UserNumofGameText.Size = new System.Drawing.Size(105, 15);
            this.UserNumofGameText.TabIndex = 8;
            this.UserNumofGameText.Text = "총 게임수: 0회";
            // 
            // UserNumof1stText
            // 
            this.UserNumof1stText.AutoSize = true;
            this.UserNumof1stText.Font = new System.Drawing.Font("굴림", 11F);
            this.UserNumof1stText.Location = new System.Drawing.Point(815, 565);
            this.UserNumof1stText.Name = "UserNumof1stText";
            this.UserNumof1stText.Size = new System.Drawing.Size(105, 15);
            this.UserNumof1stText.TabIndex = 9;
            this.UserNumof1stText.Text = "우승 횟수: 0회";
            // 
            // SendTextButton
            // 
            this.SendTextButton.Location = new System.Drawing.Point(732, 597);
            this.SendTextButton.Name = "SendTextButton";
            this.SendTextButton.Size = new System.Drawing.Size(203, 23);
            this.SendTextButton.TabIndex = 10;
            this.SendTextButton.Text = "전송";
            this.SendTextButton.UseVisualStyleBackColor = true;
            this.SendTextButton.Click += new System.EventHandler(this.SendTextButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(140, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "Password";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(947, 626);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SendTextButton);
            this.Controls.Add(this.UserNumof1stText);
            this.Controls.Add(this.UserNumofGameText);
            this.Controls.Add(this.UserNicknameText);
            this.Controls.Add(this.SignInButton);
            this.Controls.Add(this.UserList);
            this.Controls.Add(this.WriteChatBox);
            this.Controls.Add(this.ChatTextBox);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.IdTextBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IdTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.TextBox ChatTextBox;
        private System.Windows.Forms.TextBox WriteChatBox;
        private System.Windows.Forms.ListBox UserList;
        private System.Windows.Forms.Button SignInButton;
        private System.Windows.Forms.Label UserNicknameText;
        private System.Windows.Forms.Label UserNumofGameText;
        private System.Windows.Forms.Label UserNumof1stText;
        private System.Windows.Forms.Button SendTextButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

