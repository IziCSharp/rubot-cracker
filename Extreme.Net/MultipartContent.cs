using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Extreme.Net
{
	internal class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
	{
		private sealed class Element
		{
			public string Name;

			public string FileName;

			public HttpContent Content;

			public bool IsFieldFile()
			{
				return FileName != null;
			}
		}

		private const int FieldTemplateSize = 43;

		private const int FieldFileTemplateSize = 72;

		private const string FieldTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n";

		private const string FieldFileTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

		[ThreadStatic]
		private static Random _rand;

		private string _boundary;

		private List<Element> _elements = new List<Element>();

		private static Random Rand
		{
			get
			{
				if (_rand == null)
				{
					_rand = new Random();
				}
				return _rand;
			}
		}

		public MultipartContent()
			: this("----------------" + GetRandomString(16))
		{
		}

		public MultipartContent(string boundary)
		{
			if (boundary == null)
			{
				throw new ArgumentNullException("boundary");
			}
			if (boundary.Length == 0)
			{
				throw ExceptionHelper.EmptyString("boundary");
			}
			if (boundary.Length > 70)
			{
				throw ExceptionHelper.CanNotBeGreater("boundary", 70);
			}
			_boundary = boundary;
			_contentType = $"multipart/form-data; boundary={_boundary}";
		}

		public void Add(HttpContent content, string name)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			Element item = new Element
			{
				Name = name,
				Content = content
			};
			_elements.Add(item);
		}

		public void Add(HttpContent content, string name, string fileName)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			content.ContentType = Http.DetermineMediaType(Path.GetExtension(fileName));
			Element item = new Element
			{
				Name = name,
				FileName = fileName,
				Content = content
			};
			_elements.Add(item);
		}

		public void Add(HttpContent content, string name, string fileName, string contentType)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			content.ContentType = contentType;
			Element item = new Element
			{
				Name = name,
				FileName = fileName,
				Content = content
			};
			_elements.Add(item);
		}

		public override long CalculateContentLength()
		{
			ThrowIfDisposed();
			long num = 0L;
			foreach (Element element in _elements)
			{
				num += element.Content.CalculateContentLength();
				if (element.IsFieldFile())
				{
					num += 72;
					num += element.Name.Length;
					num += element.FileName.Length;
					num += element.Content.ContentType.Length;
				}
				else
				{
					num += 43;
					num += element.Name.Length;
				}
				num += _boundary.Length + 6;
			}
			return num + (_boundary.Length + 6);
		}

		public override void WriteTo(Stream stream)
		{
			ThrowIfDisposed();
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			byte[] bytes = Encoding.ASCII.GetBytes("\r\n");
			byte[] bytes2 = Encoding.ASCII.GetBytes("--" + _boundary + "\r\n");
			foreach (Element element in _elements)
			{
				stream.Write(bytes2, 0, bytes2.Length);
				string s = (!element.IsFieldFile()) ? $"Content-Disposition: form-data; name=\"{element.Name}\"\r\n\r\n" : $"Content-Disposition: form-data; name=\"{element.Name}\"; filename=\"{element.FileName}\"\r\nContent-Type: {element.Content.ContentType}\r\n\r\n";
				byte[] bytes3 = Encoding.ASCII.GetBytes(s);
				stream.Write(bytes3, 0, bytes3.Length);
				element.Content.WriteTo(stream);
				stream.Write(bytes, 0, bytes.Length);
			}
			bytes2 = Encoding.ASCII.GetBytes("--" + _boundary + "--\r\n");
			stream.Write(bytes2, 0, bytes2.Length);
		}

		public IEnumerator<HttpContent> GetEnumerator()
		{
			ThrowIfDisposed();
			return (from e in _elements
			select e.Content).GetEnumerator();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _elements != null)
			{
				foreach (Element element in _elements)
				{
					element.Content.Dispose();
				}
				_elements = null;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			ThrowIfDisposed();
			return GetEnumerator();
		}

		public static string GetRandomString(int length)
		{
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				switch (Rand.Next(3))
				{
				case 0:
					stringBuilder.Append((char)Rand.Next(48, 58));
					break;
				case 1:
					stringBuilder.Append((char)Rand.Next(97, 123));
					break;
				case 2:
					stringBuilder.Append((char)Rand.Next(65, 91));
					break;
				}
			}
			return stringBuilder.ToString();
		}

		private void ThrowIfDisposed()
		{
			if (_elements == null)
			{
				throw new ObjectDisposedException("MultipartContent");
			}
		}
	}
}
