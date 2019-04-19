using System;
using System.IO;

namespace Extreme.Net
{
	internal class FileContent : StreamContent
	{
		public FileContent(string pathToContent, int bufferSize = 32768)
		{
			if (pathToContent == null)
			{
				throw new ArgumentNullException("pathToContent");
			}
			if (pathToContent.Length == 0)
			{
				throw ExceptionHelper.EmptyString("pathToContent");
			}
			if (bufferSize < 1)
			{
				throw ExceptionHelper.CanNotBeLess("bufferSize", 1);
			}
			_content = new FileStream(pathToContent, FileMode.Open, FileAccess.Read);
			_bufferSize = bufferSize;
			_initialStreamPosition = 0L;
			_contentType = Http.DetermineMediaType(Path.GetExtension(pathToContent));
		}
	}
}
