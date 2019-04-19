using Extreme.Net;
using IniFileClass;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace rubotNet
{
	public class AWM : Form
	{
		private IniFile MyIni = new IniFile("Settings.ini");

		private IContainer components;

		private TextBox urlProxyAWM;

		private Label label9;

		private Label label32;

		private LinkLabel linkLabel1;

		private Label label12;

		private Label label11;

		private Label label10;

		private Button awmProxyInfoButton;

		private TextBox awmKeyText;

		private Label label2;

		private PictureBox pictureBox1;

		public AWM()
		{
			InitializeComponent();
			awmKeyText.Text = MyIni.Read("AWMkey");
		}

		private void awmProxyInfoButton_Click(object sender, EventArgs e)
		{
			HttpRequest httpRequest = new HttpRequest();
			string text = httpRequest.Get("https://awmproxy.com/app_api.php?apikey=" + awmKeyText.Text).ToString();
			if (text.Contains("error"))
			{
				Interaction.MsgBox("Неверный ключ.", MsgBoxStyle.Information);
				return;
			}
			label10.Text = "Текущий тариф: " + text.Substring("name_tariff\":\"", "\",");
			label11.Text = "Активен до: " + text.Substring("date_expired\":\"", "\",");
			label12.Text = "Проксей доступно: " + text.Substring("count\":", ",");
			label32.Text = "Доступно потоков: " + text.Substring("threads\":\"", "\",");
			urlProxyAWM.Text = text.Substring("url\":\"", "\",").Replace("\\", "");
			MyIni.Write("AWMkey", awmKeyText.Text);
			awmProxyInfoButton.Text = "Ваш IP привязан";
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
			urlProxyAWM = new System.Windows.Forms.TextBox();
			label9 = new System.Windows.Forms.Label();
			label32 = new System.Windows.Forms.Label();
			linkLabel1 = new System.Windows.Forms.LinkLabel();
			label12 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			awmProxyInfoButton = new System.Windows.Forms.Button();
			awmKeyText = new System.Windows.Forms.TextBox();
			label2 = new System.Windows.Forms.Label();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
			SuspendLayout();
			urlProxyAWM.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			urlProxyAWM.Location = new System.Drawing.Point(136, 233);
			urlProxyAWM.Name = "urlProxyAWM";
			urlProxyAWM.Size = new System.Drawing.Size(207, 25);
			urlProxyAWM.TabIndex = 27;
			label9.AutoSize = true;
			label9.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			label9.Location = new System.Drawing.Point(12, 236);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(118, 17);
			label9.TabIndex = 26;
			label9.Text = "Ссылка на прокси:";
			label32.AutoSize = true;
			label32.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			label32.Location = new System.Drawing.Point(12, 210);
			label32.Name = "label32";
			label32.Size = new System.Drawing.Size(120, 17);
			label32.TabIndex = 25;
			label32.Text = "Доступно потоков:";
			linkLabel1.AutoSize = true;
			linkLabel1.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			linkLabel1.Location = new System.Drawing.Point(180, 29);
			linkLabel1.Name = "linkLabel1";
			linkLabel1.Size = new System.Drawing.Size(112, 17);
			linkLabel1.TabIndex = 23;
			linkLabel1.TabStop = true;
			linkLabel1.Text = "Получить аккаунт";
			linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
			label12.AutoSize = true;
			label12.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			label12.Location = new System.Drawing.Point(12, 180);
			label12.Name = "label12";
			label12.Size = new System.Drawing.Size(120, 17);
			label12.TabIndex = 21;
			label12.Text = "Проксей доступно:";
			label11.AutoSize = true;
			label11.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			label11.Location = new System.Drawing.Point(12, 153);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(77, 17);
			label11.TabIndex = 19;
			label11.Text = "Активен до:";
			label10.AutoSize = true;
			label10.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			label10.Location = new System.Drawing.Point(12, 127);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(102, 17);
			label10.TabIndex = 17;
			label10.Text = "Текущий тариф:";
			awmProxyInfoButton.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			awmProxyInfoButton.Location = new System.Drawing.Point(173, 90);
			awmProxyInfoButton.Name = "awmProxyInfoButton";
			awmProxyInfoButton.Size = new System.Drawing.Size(167, 25);
			awmProxyInfoButton.TabIndex = 16;
			awmProxyInfoButton.Text = "Проверка и привязка";
			awmProxyInfoButton.UseVisualStyleBackColor = true;
			awmProxyInfoButton.Click += new System.EventHandler(awmProxyInfoButton_Click);
			awmKeyText.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			awmKeyText.Location = new System.Drawing.Point(15, 91);
			awmKeyText.Name = "awmKeyText";
			awmKeyText.Size = new System.Drawing.Size(154, 25);
			awmKeyText.TabIndex = 13;
			label2.AutoSize = true;
			label2.Font = new System.Drawing.Font("Segoe UI", 9.75f);
			label2.Location = new System.Drawing.Point(12, 71);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(164, 17);
			label2.TabIndex = 10;
			label2.Text = "Секретный ключ (API-key):";
			pictureBox1.Location = new System.Drawing.Point(12, 12);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(162, 34);
			pictureBox1.TabIndex = 24;
			pictureBox1.TabStop = false;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(352, 263);
			base.Controls.Add(urlProxyAWM);
			base.Controls.Add(label9);
			base.Controls.Add(pictureBox1);
			base.Controls.Add(label32);
			base.Controls.Add(linkLabel1);
			base.Controls.Add(label12);
			base.Controls.Add(label2);
			base.Controls.Add(label11);
			base.Controls.Add(awmKeyText);
			base.Controls.Add(label10);
			base.Controls.Add(awmProxyInfoButton);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "AWM";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			Text = "AWMProxy service";
			((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
			ResumeLayout(performLayout: false);
			PerformLayout();
		}
	}
}
