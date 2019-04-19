using System;

namespace TechLifeForum
{
	public class ChannelMessageEventArgs : EventArgs
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

		public string Message
		{
			get;
			internal set;
		}

		public ChannelMessageEventArgs(string channel, string from, string message)
		{
			Channel = channel;
			From = from;
			Message = message;
		}
	}
}
