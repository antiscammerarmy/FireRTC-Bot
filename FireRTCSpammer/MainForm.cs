using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireRTCBot
{
    public partial class MainForm : Form
    {
        private static bool buttonPressed = false;
        public MainForm()
        {
            InitializeComponent();
            Application.EnableVisualStyles();

            btn_StartSpamming.Click += Btn_StartSpamming_Click;
            txt_Username.GotFocus += Txt_Username_GotFocus;
            txt_Password.GotFocus += Txt_Password_GotFocus;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!buttonPressed)
            {
                Environment.Exit(0);
            }
        }

        private void Txt_Username_GotFocus(object sender, EventArgs e)
        {
            if (txt_Username.Text == "Username/Email")
            {
                txt_Username.Text = "";
            }
        }

        private void Txt_Password_GotFocus(object sender, EventArgs e)
        {
            if (txt_Password.Text == "Password")
            {
                txt_Password.Text = "";
            }
        }
        private void CenterControl(Control c)
        {
            c.Left = (ClientSize.Width - c.Width) / 2;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ActiveControl = pictureBox1;
            foreach (Control c in Controls)
            {
                CenterControl(c);
            }
        }

        private void Btn_StartSpamming_Click(object sender, EventArgs e)
        {
            if (chk_Terms.Checked == false)
            {
                MessageBox.Show("You must agree to the terms before you can use this program.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (TextBox tb in Controls.OfType<TextBox>())
            {
                if (tb.Text == String.Empty)
                {
                    MessageBox.Show("At least one text box is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            MessageBox.Show("Please make sure your microphone is disabled/on mute or your Virtual Audio Cables/Stereo Mix is properly setup, then hit 'OK'.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Program.email = txt_Username.Text;
            Program.password = txt_Password.Text;
            buttonPressed = true;
            btn_StartSpamming.Enabled = false;
            Close();
        }

		private void chk_Terms_CheckedChanged(object sender, EventArgs e)
		{
			btn_StartSpamming.Enabled = chk_Terms.Checked;
		}
	}
}
