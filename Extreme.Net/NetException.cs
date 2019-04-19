using System;
using System.Runtime.Serialization;

namespace Extreme.Net
{
	[Serializable]
	public class NetException : Exception
	{
		public NetException()
			: this(Resources.NetException_Default)
		{
		}

		public NetException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		protected NetException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
