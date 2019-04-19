using System;

namespace TechLifeForum
{
	public class ExceptionEventArgs : EventArgs
	{
		public Exception Exception
		{
			get;
			internal set;
		}

		public ExceptionEventArgs(Exception x)
		{
			Exception = x;
		}

		public override string ToString()
		{
			return Exception.ToString();
		}
	}
}
