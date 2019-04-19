using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace rubotNet.Chatlist
{
	public class ChatList : UserControl
	{
		private IContainer components;

		public static ListBox chattersListBox;

		public ChatList()
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
			chattersListBox = new System.Windows.Forms.ListBox();
			SuspendLayout();
			chattersListBox.FormattingEnabled = true;
			chattersListBox.Location = new System.Drawing.Point(0, 0);
			chattersListBox.Name = "chattersListBox";
			chattersListBox.Size = new System.Drawing.Size(125, 230);
			chattersListBox.TabIndex = 59;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(chattersListBox);
			base.Name = "ChatList";
			base.Size = new System.Drawing.Size(144, 295);
			ResumeLayout(performLayout: false);
		}
	}
}
