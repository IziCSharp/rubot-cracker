using System;

namespace TechLifeForum
{
	public class PrivateMessageEventArgs : EventArgs
	{
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

		public PrivateMessageEventArgs(string from, string message)
		{
			From = from;
			Message = message;
		}
	}
}
