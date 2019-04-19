using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace rubotNet.UrlsProxy
{
	public class UrlListProxy : UserControl
	{
		private IContainer components;

		public static RichTextBox richTextBox1;

		public UrlListProxy()
		{
			InitializeComponent();
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
			richTextBox1 = new System.Windows.Forms.RichTextBox();
			SuspendLayout();
			richTextBox1.Location = new System.Drawing.Point(0, 0);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new System.Drawing.Size(586, 118);
			richTextBox1.TabIndex = 78;
			richTextBox1.Text = "";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(richTextBox1);
			base.Name = "UrlListProxy";
			base.Size = new System.Drawing.Size(586, 118);
			ResumeLayout(performLayout: false);
		}
	}
}
