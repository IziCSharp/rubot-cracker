using System;

namespace Extreme.Net
{
	internal sealed class UploadProgressChangedEventArgs : EventArgs
	{
		public long BytesSent
		{
			get;
			private set;
		}

		public long TotalBytesToSend
		{
			get;
			private set;
		}

		public double ProgressPercentage => (double)BytesSent / (double)TotalBytesToSend * 100.0;

		public UploadProgressChangedEventArgs(long bytesSent, long totalBytesToSend)
		{
			BytesSent = bytesSent;
			TotalBytesToSend = totalBytesToSend;
		}
	}
}
