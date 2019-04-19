using System;

namespace TechLifeForum
{
	public class ModeSetEventArgs : EventArgs
	{
		public string Channel
		{
			get;
			internal set;
		}

		public string From
		{
			get;
			internal set;
		}

		public string To
		{
			get;
			internal set;
		}

		public string Mode
		{
			get;
			internal set;
		}

		public ModeSetEventArgs(string channel, string from, string to, string mode)
		{
			Channel = channel;
			From = from;
			To = to;
			Mode = mode;
		}
	}
}
