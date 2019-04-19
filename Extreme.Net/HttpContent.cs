using System.IO;

namespace Extreme.Net
{
	internal abstract class HttpContent
	{
		protected string _contentType = string.Empty;

		public string ContentType
		{
			get
			{
				return _contentType;
			}
			set
			{
				_contentType = (value ?? string.Empty);
			}
		}

		public abstract long CalculateContentLength();

		public abstract void WriteTo(Stream stream);

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
