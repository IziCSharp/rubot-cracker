using System;
using System.IO;

namespace Extreme.Net
{
	internal class BytesContent : HttpContent
	{
		protected byte[] _content;

		protected int _offset;

		protected int _count;

		public BytesContent(byte[] content)
			: this(content, 0, content.Length)
		{
		}

		public BytesContent(byte[] content, int offset, int count)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (offset < 0)
			{
				throw ExceptionHelper.CanNotBeLess("offset", 0);
			}
			if (offset > content.Length)
			{
				throw ExceptionHelper.CanNotBeGreater("offset", content.Length);
			}
			if (count < 0)
			{
				throw ExceptionHelper.CanNotBeLess("count", 0);
			}
			if (count > content.Length - offset)
			{
				throw ExceptionHelper.CanNotBeGreater("count", content.Length - offset);
			}
			_content = content;
			_offset = offset;
			_count = count;
			_contentType = "application/octet-stream";
		}

		protected BytesContent()
		{
		}

		public override long CalculateContentLength()
		{
			return _content.LongLength;
		}

		public override void WriteTo(Stream stream)
		{
			stream.Write(_content, _offset, _count);
		}
	}
}
