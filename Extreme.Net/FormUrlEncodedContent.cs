using System;
using System.Collections.Generic;
using System.Text;

namespace Extreme.Net
{
	internal class FormUrlEncodedContent : BytesContent
	{
		public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> content, bool dontEscape = false, Encoding encoding = null)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			string s = Http.ToPostQueryString(content, dontEscape, encoding);
			_content = Encoding.ASCII.GetBytes(s);
			_offset = 0;
			_count = _content.Length;
			_contentType = "application/x-www-form-urlencoded";
		}
	}
}
