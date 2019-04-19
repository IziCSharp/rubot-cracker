using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace rubotNet.Logs
{
	public class Log : UserControl
	{
		private IContainer components;

		public static RichTextBox richLog;

		public Log()
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
			richLog = new System.Windows.Forms.RichTextBox();
			SuspendLayout();
			richLog.Location = new System.Drawing.Point(0, 0);
			richLog.Name = "richLog";
			richLog.Size = new System.Drawing.Size(617, 72);
			richLog.TabIndex = 96;
			richLog.Text = "";
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(richLog);
			base.Name = "Log";
			base.Size = new System.Drawing.Size(617, 72);
			ResumeLayout(performLayout: false);
		}
	}
}
