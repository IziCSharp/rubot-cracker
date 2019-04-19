using System;

namespace Extreme.Net
{
	internal sealed class DownloadProgressChangedEventArgs : EventArgs
	{
		public long BytesReceived
		{
			get;
			private set;
		}

		public long TotalBytesToReceive
		{
			get;
			private set;
		}

		public double ProgressPercentage => (double)BytesReceived / (double)TotalBytesToReceive * 100.0;

		public DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive)
		{
			BytesReceived = bytesReceived;
			TotalBytesToReceive = totalBytesToReceive;
		}
	}
}
