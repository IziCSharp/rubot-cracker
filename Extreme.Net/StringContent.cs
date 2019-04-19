using System;
using System.Text;

namespace Extreme.Net
{
	internal class StringContent : BytesContent
	{
		public StringContent(string content)
			: this(content, Encoding.UTF8)
		{
		}

		public StringContent(string content, Encoding encoding)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			_content = encoding.GetBytes(content);
			_offset = 0;
			_count = _content.Length;
			_contentType = "text/plain";
		}
	}
}
