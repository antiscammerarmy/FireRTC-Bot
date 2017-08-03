namespace FireRTCBot
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
       

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.chk_Terms = new System.Windows.Forms.CheckBox();
			this.btn_StartSpamming = new System.Windows.Forms.Button();
			this.txt_Password = new System.Windows.Forms.TextBox();
			this.txt_Username = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// chk_Terms
			// 
			this.chk_Terms.BackColor = System.Drawing.Color.Transparent;
			this.chk_Terms.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chk_Terms.Location = new System.Drawing.Point(9, 148);
			this.chk_Terms.Margin = new System.Windows.Forms.Padding(4);
			this.chk_Terms.Name = "chk_Terms";
			this.chk_Terms.Size = new System.Drawing.Size(259, 59);
			this.chk_Terms.TabIndex = 15;
			this.chk_Terms.Text = "I agree that the author is not responsible if my FireRTC account is banned as a r" +
    "esult of this program.";
			this.chk_Terms.UseVisualStyleBackColor = false;
			this.chk_Terms.CheckedChanged += new System.EventHandler(this.Chk_Terms_CheckedChanged);
			// 
			// btn_StartSpamming
			// 
			this.btn_StartSpamming.Enabled = false;
			this.btn_StartSpamming.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btn_StartSpamming.Location = new System.Drawing.Point(9, 215);
			this.btn_StartSpamming.Margin = new System.Windows.Forms.Padding(4);
			this.btn_StartSpamming.Name = "btn_StartSpamming";
			this.btn_StartSpamming.Size = new System.Drawing.Size(260, 25);
			this.btn_StartSpamming.TabIndex = 16;
			this.btn_StartSpamming.Text = "Start Bot\r\n";
			this.btn_StartSpamming.UseVisualStyleBackColor = true;
			this.btn_StartSpamming.Click += new System.EventHandler(this.Btn_StartSpamming_Click);
			// 
			// txt_Password
			// 
			this.txt_Password.Location = new System.Drawing.Point(9, 118);
			this.txt_Password.Margin = new System.Windows.Forms.Padding(4);
			this.txt_Password.Name = "txt_Password";
			this.txt_Password.Size = new System.Drawing.Size(259, 22);
			this.txt_Password.TabIndex = 12;
			this.txt_Password.Text = "Password";
			this.txt_Password.UseSystemPasswordChar = true;
			// 
			// txt_Username
			// 
			this.txt_Username.Location = new System.Drawing.Point(9, 88);
			this.txt_Username.Margin = new System.Windows.Forms.Padding(4);
			this.txt_Username.Name = "txt_Username";
			this.txt_Username.Size = new System.Drawing.Size(259, 22);
			this.txt_Username.TabIndex = 11;
			this.txt_Username.Text = "Username/Email";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Image = global::FireRTCSpammer.Properties.Resources.Untitled;
			this.pictureBox1.Location = new System.Drawing.Point(8, 11);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(259, 67);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 9;
			this.pictureBox1.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(280, 249);
			this.Controls.Add(this.chk_Terms);
			this.Controls.Add(this.btn_StartSpamming);
			this.Controls.Add(this.txt_Password);
			this.Controls.Add(this.txt_Username);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FireRTC Bot";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chk_Terms;
        private System.Windows.Forms.Button btn_StartSpamming;
        private System.Windows.Forms.TextBox txt_Password;
        private System.Windows.Forms.TextBox txt_Username;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}