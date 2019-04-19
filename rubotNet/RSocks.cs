using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace rubotNet
{
	public class RSocks : Form
	{
		private IContainer components;

		private TextBox textBox4;

		private Button rsocksProxyInfoButton;

		private Label label30;

		private Label label29;

		private Label label28;

		private TextBox textBox3;

		private TextBox textBox2;

		private LinkLabel linkLabel3;

		private PictureBox pictureBox2;

		public RSocks()
		{
			InitializeComponent();
		}

		private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
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
			textBox4 = new System.Windows.Forms.TextBox();
			rsocksProxyInfoButton = new System.Windows.Forms.Button();
			label30 = new System.Windows.Forms.Label();
			label29 = new System.Windows.Forms.Label();
			label28 = new System.Windows.Forms.Label();
			textBox3 = new System.Windows.Forms.TextBox();
			textBox2 = new System.Windows.Forms.TextBox();
			linkLabel3 = new System.Windows.Forms.LinkLabel();
			pictureBox2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
			SuspendLayout();
			textBox4.Location = new System.Drawing.Point(12, 201);
			textBox4.Multiline = true;
			textBox4.Name = "textBox4";
			textBox4.Size = new System.Drawing.Size(230, 75);
			textBox4.TabIndex = 32;
			rsocksProxyInfoButton.Location = new System.Drawing.Point(12, 128);
			rsocksProxyInfoButton.Name = "rsocksProxyInfoButton";
			rsocksProxyInfoButton.Size = new System.Drawing.Size(112, 34);
			rsocksProxyInfoButton.TabIndex = 30;
			rsocksProxyInfoButton.Text = "Авторизация";
			rsocksProxyInfoButton.UseVisualStyleBackColor = true;
			label30.AutoSize = true;
			label30.Location = new System.Drawing.Point(9, 175);
			label30.Name = "label30";
			label30.Size = new System.Drawing.Size(100, 13);
			label30.TabIndex = 29;
			label30.Text = "Активные пакеты:";
			label29.AutoSize = true;
			label29.Location = new System.Drawing.Point(9, 108);
			label29.Name = "label29";
			label29.Size = new System.Drawing.Size(63, 13);
			label29.TabIndex = 28;
			label29.Text = "X-Auth-Key:";
			label28.AutoSize = true;
			label28.Location = new System.Drawing.Point(9, 84);
			label28.Name = "label28";
			label28.Size = new System.Drawing.Size(56, 13);
			label28.TabIndex = 27;
			label28.Text = "X-Auth-ID:";
			textBox3.Location = new System.Drawing.Point(71, 102);
			textBox3.Name = "textBox3";
			textBox3.Size = new System.Drawing.Size(100, 20);
			textBox3.TabIndex = 26;
			textBox2.Location = new System.Drawing.Point(71, 78);
			textBox2.Name = "textBox2";
			textBox2.Size = new System.Drawing.Size(100, 20);
			textBox2.TabIndex = 25;
			linkLabel3.AutoSize = true;
			linkLabel3.Location = new System.Drawing.Point(187, 30);
			linkLabel3.Name = "linkLabel3";
			linkLabel3.Size = new System.Drawing.Size(97, 13);
			linkLabel3.TabIndex = 24;
			linkLabel3.TabStop = true;
			linkLabel3.Text = "Получить аккаунт";
			linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel3_LinkClicked);
			pictureBox2.Location = new System.Drawing.Point(12, 12);
			pictureBox2.Name = "pictureBox2";
			pictureBox2.Size = new System.Drawing.Size(169, 52);
			pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			pictureBox2.TabIndex = 0;
			pictureBox2.TabStop = false;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(288, 292);
			base.Controls.Add(textBox4);
			base.Controls.Add(pictureBox2);
			base.Controls.Add(label30);
			base.Controls.Add(rsocksProxyInfoButton);
			base.Controls.Add(linkLabel3);
			base.Controls.Add(label28);
			base.Controls.Add(textBox3);
			base.Controls.Add(label29);
			base.Controls.Add(textBox2);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "RSocks";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "RSocks";
			((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
			ResumeLayout(performLayout: false);
			PerformLayout();
		}
	}
}
