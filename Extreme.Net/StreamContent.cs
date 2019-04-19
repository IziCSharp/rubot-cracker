using System;
using System.IO;

namespace Extreme.Net
{
	internal class StreamContent : HttpContent
	{
		protected Stream _content;

		protected int _bufferSize;

		protected long _initialStreamPosition;

		public StreamContent(Stream content, int bufferSize = 32768)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (!content.CanRead || !content.CanSeek)
			{
				throw new ArgumentException(Resources.ArgumentException_CanNotReadOrSeek, "content");
			}
			if (bufferSize < 1)
			{
				throw ExceptionHelper.CanNotBeLess("bufferSize", 1);
			}
			_content = content;
			_bufferSize = bufferSize;
			_initialStreamPosition = _content.Position;
			_contentType = "application/octet-stream";
		}

		protected StreamContent()
		{
		}

		public override long CalculateContentLength()
		{
			ThrowIfDisposed();
			return _content.Length;
		}

		public override void WriteTo(Stream stream)
		{
			ThrowIfDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			_content.Position = _initialStreamPosition;
			byte[] array = new byte[_bufferSize];
			while (true)
			{
				int num = _content.Read(array, 0, array.Length);
				if (num != 0)
				{
					stream.Write(array, 0, num);
					continue;
				}
				break;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _content != null)
			{
				_content.Dispose();
				_content = null;
			}
		}

		private void ThrowIfDisposed()
		{
			if (_content == null)
			{
				throw new ObjectDisposedException("StreamContent");
			}
		}
	}
}
