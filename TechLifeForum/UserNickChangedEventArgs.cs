using System;

namespace TechLifeForum
{
	public class UserNickChangedEventArgs : EventArgs
	{
		public string Old
		{
			get;
			internal set;
		}

		public string New
		{
			get;
			internal set;
		}

		public UserNickChangedEventArgs(string oldNick, string newNick)
		{
			Old = oldNick;
			New = newNick;
		}
	}
}
