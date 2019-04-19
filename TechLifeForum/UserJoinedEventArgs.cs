using System;

namespace TechLifeForum
{
	public class UserJoinedEventArgs : EventArgs
	{
		public string Channel
		{
			get;
			internal set;
		}

		public string User
		{
			get;
			internal set;
		}

		public UserJoinedEventArgs(string channel, string user)
		{
			Channel = channel;
			User = user;
		}
	}
}
