using System;
using System.Runtime.Serialization;

namespace Extreme.Net
{
	[Serializable]
	public sealed class ProxyException : NetException
	{
		public ProxyClient ProxyClient
		{
			get;
			private set;
		}

		public ProxyException()
			: this(Resources.ProxyException_Default)
		{
		}

		public ProxyException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		public ProxyException(string message, ProxyClient proxyClient, Exception innerException = null)
			: base(message, innerException)
		{
			ProxyClient = proxyClient;
		}

		protected ProxyException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
		}
	}
}
