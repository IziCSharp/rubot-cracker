using IniFileClass;
using rubotNet.Lcheck;
using rubotNet.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace rubotNet
{
	public class Login : UserControl
	{
		private IniFile MyIni = new IniFile("Settings.ini");

		public string login;

		public string autostart;

		public Form main;

		public Panel panel;

		private IContainer components;

		public Login(Form main1, Panel panel1)
		{
			InitializeComponent();
			main = main1;
			panel = panel1;
	
		}

		private void loginButton_Click(object sender, EventArgs e)
		{
		
			LicenseCheck.Connectvkabinu(panel, this);
		
		}

		private void pictureBox3_Click(object sender, EventArgs e)
		{
			Process.Start("https://awmproxy.com/?a=317883");
		}

		private void pictureBox2_Click(object sender, EventArgs e)
		{
			Process.Start("https://rsocks.net/?ref=97_KKosKQLiFPDfvqJxxKnFoS6Q0ID-Hh8DVsX");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "Login";
            this.Size = new System.Drawing.Size(630, 545);
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);

		}

        private void Login_Load(object sender, EventArgs e)
        {
            LicenseCheck.Connectvkabinu(panel, this);
        }
    }
}
