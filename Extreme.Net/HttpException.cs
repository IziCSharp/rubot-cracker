using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Extreme.Net
{
	[Serializable]
	internal sealed class HttpException : NetException
	{
		public HttpExceptionStatus Status
		{
			get;
			internal set;
		}

		public HttpStatusCode HttpStatusCode
		{
			get;
			private set;
		}

		internal bool EmptyMessageBody
		{
			get;
			set;
		}

		public HttpException()
			: this(Resources.HttpException_Default)
		{
		}

		public HttpException(string message, Exception innerException = null)
			: base(message, innerException)
		{
		}

		public HttpException(string message, HttpExceptionStatus status, HttpStatusCode httpStatusCode = HttpStatusCode.None, Exception innerException = null)
			: base(message, innerException)
		{
			Status = status;
			HttpStatusCode = httpStatusCode;
		}

		protected HttpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
			if (serializationInfo != null)
			{
				Status = (HttpExceptionStatus)serializationInfo.GetInt32("Status");
				HttpStatusCode = (HttpStatusCode)serializationInfo.GetInt32("HttpStatusCode");
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			base.GetObjectData(serializationInfo, streamingContext);
			if (serializationInfo != null)
			{
				serializationInfo.AddValue("Status", (int)Status);
				serializationInfo.AddValue("HttpStatusCode", (int)HttpStatusCode);
			}
		}
	}
}
