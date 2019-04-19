using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Extreme.Net
{
	internal sealed class HttpResponse
	{
		private sealed class BytesWraper
		{
			public int Length
			{
				get;
				set;
			}

			public byte[] Value
			{
				get;
				set;
			}
		}

		private sealed class ReceiverHelper
		{
			private const int InitialLineSize = 1000;

			private Stream _stream;

			private byte[] _buffer;

			private int _bufferSize;

			private int _linePosition;

			private byte[] _lineBuffer = new byte[1000];

			public bool HasData => Length - Position != 0;

			public int Length
			{
				get;
				private set;
			}

			public int Position
			{
				get;
				private set;
			}

			public ReceiverHelper(int bufferSize)
			{
				_bufferSize = bufferSize;
				_buffer = new byte[_bufferSize];
			}

			public void Init(Stream stream)
			{
				_stream = stream;
				_linePosition = 0;
				Length = 0;
				Position = 0;
			}

			public string ReadLine()
			{
				_linePosition = 0;
				while (true)
				{
					if (Position == Length)
					{
						Position = 0;
						Length = _stream.Read(_buffer, 0, _bufferSize);
						if (Length == 0)
						{
							break;
						}
					}
					byte b = _buffer[Position++];
					_lineBuffer[_linePosition++] = b;
					if (b == 10)
					{
						break;
					}
					if (_linePosition == _lineBuffer.Length)
					{
						byte[] array = new byte[_lineBuffer.Length * 2];
						_lineBuffer.CopyTo(array, 0);
						_lineBuffer = array;
					}
				}
				return Encoding.ASCII.GetString(_lineBuffer, 0, _linePosition);
			}

			public int Read(byte[] buffer, int index, int length)
			{
				int num = Length - Position;
				if (num > length)
				{
					num = length;
				}
				Array.Copy(_buffer, Position, buffer, index, num);
				Position += num;
				return num;
			}
		}

		private sealed class ZipWraperStream : Stream
		{
			private Stream _baseStream;

			private ReceiverHelper _receiverHelper;

			public int BytesRead
			{
				get;
				private set;
			}

			public int TotalBytesRead
			{
				get;
				set;
			}

			public int LimitBytesRead
			{
				get;
				set;
			}

			public override bool CanRead => _baseStream.CanRead;

			public override bool CanSeek => _baseStream.CanSeek;

			public override bool CanTimeout => _baseStream.CanTimeout;

			public override bool CanWrite => _baseStream.CanWrite;

			public override long Length => _baseStream.Length;

			public override long Position
			{
				get
				{
					return _baseStream.Position;
				}
				set
				{
					_baseStream.Position = value;
				}
			}

			public ZipWraperStream(Stream baseStream, ReceiverHelper receiverHelper)
			{
				_baseStream = baseStream;
				_receiverHelper = receiverHelper;
			}

			public override void Flush()
			{
				_baseStream.Flush();
			}

			public override void SetLength(long value)
			{
				_baseStream.SetLength(value);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return _baseStream.Seek(offset, origin);
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				if (LimitBytesRead != 0)
				{
					int num = LimitBytesRead - TotalBytesRead;
					if (num == 0)
					{
						return 0;
					}
					if (num > buffer.Length)
					{
						num = buffer.Length;
					}
					if (_receiverHelper.HasData)
					{
						BytesRead = _receiverHelper.Read(buffer, offset, num);
					}
					else
					{
						BytesRead = _baseStream.Read(buffer, offset, num);
					}
				}
				else if (_receiverHelper.HasData)
				{
					BytesRead = _receiverHelper.Read(buffer, offset, count);
				}
				else
				{
					BytesRead = _baseStream.Read(buffer, offset, count);
				}
				TotalBytesRead += BytesRead;
				return BytesRead;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				_baseStream.Write(buffer, offset, count);
			}
		}

		private static readonly byte[] _openHtmlSignature = Encoding.ASCII.GetBytes("<html");

		private static readonly byte[] _closeHtmlSignature = Encoding.ASCII.GetBytes("</html>");

		private static readonly Regex _keepAliveTimeoutRegex = new Regex("timeout(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex _keepAliveMaxRegex = new Regex("max(|\\s+)=(|\\s+)(?<value>\\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static readonly Regex _contentCharsetRegex = new Regex("charset(|\\s+)=(|\\s+)(?<value>[a-z,0-9,-]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly HttpRequest _request;

		private ReceiverHelper _receiverHelper;

		private readonly Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private readonly CookieDictionary _rawCookies = new CookieDictionary();

		public bool HasError
		{
			get;
			private set;
		}

		public bool MessageBodyLoaded
		{
			get;
			private set;
		}

		public bool IsOK => StatusCode == HttpStatusCode.OK;

		public bool HasRedirect
		{
			get
			{
				int statusCode = (int)StatusCode;
				if (statusCode >= 300 && statusCode < 400)
				{
					return true;
				}
				if (_headers.ContainsKey("Location"))
				{
					return true;
				}
				if (_headers.ContainsKey("Redirect-Location"))
				{
					return true;
				}
				return false;
			}
		}

		public int ReconnectCount
		{
			get;
			internal set;
		}

		public Uri Address
		{
			get;
			private set;
		}

		public HttpMethod Method
		{
			get;
			private set;
		}

		public Version ProtocolVersion
		{
			get;
			private set;
		}

		public HttpStatusCode StatusCode
		{
			get;
			private set;
		}

		public Uri RedirectAddress
		{
			get;
			private set;
		}

		public Encoding CharacterSet
		{
			get;
			private set;
		}

		public int ContentLength
		{
			get;
			private set;
		}

		public string ContentType
		{
			get;
			private set;
		}

		public string Location => this["Location"];

		public CookieDictionary Cookies
		{
			get;
			private set;
		}

		public int? KeepAliveTimeout
		{
			get;
			private set;
		}

		public int? MaximumKeepAliveRequests
		{
			get;
			private set;
		}

		public string this[string headerName]
		{
			get
			{
				if (headerName == null)
				{
					throw new ArgumentNullException("headerName");
				}
				if (headerName.Length == 0)
				{
					throw ExceptionHelper.EmptyString("headerName");
				}
				if (!_headers.TryGetValue(headerName, out string value))
				{
					return string.Empty;
				}
				return value;
			}
		}

		public string this[HttpHeader header]
		{
			get
			{
				return this[Http.Headers[header]];
			}
		}

		internal HttpResponse(HttpRequest request)
		{
			_request = request;
			ContentLength = -1;
			ContentType = string.Empty;
		}

		public byte[] ToBytes()
		{
			if (HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (MessageBodyLoaded)
			{
				return new byte[0];
			}
			MemoryStream memoryStream = new MemoryStream((ContentLength != -1) ? ContentLength : 0);
			try
			{
				IEnumerable<BytesWraper> messageBodySource = GetMessageBodySource();
				foreach (BytesWraper item in messageBodySource)
				{
					memoryStream.Write(item.Value, 0, item.Length);
				}
			}
			catch (Exception ex)
			{
				HasError = true;
				if (ex is IOException || ex is InvalidOperationException)
				{
					throw NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
				}
				throw;
			}
			if (ConnectionClosed())
			{
				_request.Dispose();
			}
			MessageBodyLoaded = true;
			return memoryStream.ToArray();
		}

		public override string ToString()
		{
			if (HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (MessageBodyLoaded)
			{
				return string.Empty;
			}
			MemoryStream memoryStream = new MemoryStream((ContentLength != -1) ? ContentLength : 0);
			try
			{
				IEnumerable<BytesWraper> messageBodySource = GetMessageBodySource();
				foreach (BytesWraper item in messageBodySource)
				{
					memoryStream.Write(item.Value, 0, item.Length);
				}
			}
			catch (Exception ex)
			{
				HasError = true;
				if (ex is IOException || ex is InvalidOperationException)
				{
					throw NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
				}
				throw;
			}
			if (ConnectionClosed())
			{
				_request.Dispose();
			}
			MessageBodyLoaded = true;
			return CharacterSet.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
		}

		public void ToFile(string path)
		{
			if (HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!MessageBodyLoaded)
			{
				try
				{
					using (FileStream fileStream = new FileStream(path, FileMode.Create))
					{
						IEnumerable<BytesWraper> messageBodySource = GetMessageBodySource();
						foreach (BytesWraper item in messageBodySource)
						{
							fileStream.Write(item.Value, 0, item.Length);
						}
					}
				}
				catch (ArgumentException innerException)
				{
					throw ExceptionHelper.WrongPath("path", innerException);
				}
				catch (NotSupportedException innerException2)
				{
					throw ExceptionHelper.WrongPath("path", innerException2);
				}
				catch (Exception ex)
				{
					HasError = true;
					if (ex is IOException || ex is InvalidOperationException)
					{
						throw NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
					}
					throw;
				}
				if (ConnectionClosed())
				{
					_request.Dispose();
				}
				MessageBodyLoaded = true;
			}
		}

		public MemoryStream ToMemoryStream()
		{
			if (HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (MessageBodyLoaded)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream((ContentLength != -1) ? ContentLength : 0);
			try
			{
				IEnumerable<BytesWraper> messageBodySource = GetMessageBodySource();
				foreach (BytesWraper item in messageBodySource)
				{
					memoryStream.Write(item.Value, 0, item.Length);
				}
			}
			catch (Exception ex)
			{
				HasError = true;
				if (ex is IOException || ex is InvalidOperationException)
				{
					throw NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
				}
				throw;
			}
			if (ConnectionClosed())
			{
				_request.Dispose();
			}
			MessageBodyLoaded = true;
			memoryStream.Position = 0L;
			return memoryStream;
		}

		public void None()
		{
			if (HasError)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_HttpResponse_HasError);
			}
			if (!MessageBodyLoaded)
			{
				if (ConnectionClosed())
				{
					_request.Dispose();
				}
				else
				{
					try
					{
						IEnumerable<BytesWraper> messageBodySource = GetMessageBodySource();
						foreach (BytesWraper item in messageBodySource)
						{
						}
					}
					catch (Exception ex)
					{
						HasError = true;
						if (ex is IOException || ex is InvalidOperationException)
						{
							throw NewHttpException(Resources.HttpException_FailedReceiveMessageBody, ex);
						}
						throw;
					}
				}
				MessageBodyLoaded = true;
			}
		}

		public bool ContainsCookie(string name)
		{
			if (Cookies == null)
			{
				return false;
			}
			return Cookies.ContainsKey(name);
		}

		public bool ContainsRawCookie(string name)
		{
			return _rawCookies.ContainsKey(name);
		}

		public string GetRawCookie(string name)
		{
			if (!_rawCookies.TryGetValue(name, out string value))
			{
				return string.Empty;
			}
			return value;
		}

		public Dictionary<string, string>.Enumerator EnumerateRawCookies()
		{
			return _rawCookies.GetEnumerator();
		}

		public bool ContainsHeader(string headerName)
		{
			if (headerName == null)
			{
				throw new ArgumentNullException("headerName");
			}
			if (headerName.Length == 0)
			{
				throw ExceptionHelper.EmptyString("headerName");
			}
			return _headers.ContainsKey(headerName);
		}

		public bool ContainsHeader(HttpHeader header)
		{
			return ContainsHeader(Http.Headers[header]);
		}

		public Dictionary<string, string>.Enumerator EnumerateHeaders()
		{
			return _headers.GetEnumerator();
		}

		internal long LoadResponse(HttpMethod method)
		{
			Method = method;
			Address = _request.Address;
			HasError = false;
			MessageBodyLoaded = false;
			KeepAliveTimeout = null;
			MaximumKeepAliveRequests = null;
			_headers.Clear();
			_rawCookies.Clear();
			if (_request.Cookies != null && !_request.Cookies.IsLocked)
			{
				Cookies = _request.Cookies;
			}
			else
			{
				Cookies = new CookieDictionary();
			}
			if (_receiverHelper == null)
			{
				_receiverHelper = new ReceiverHelper(_request.TcpClient.ReceiveBufferSize);
			}
			_receiverHelper.Init(_request.ClientStream);
			try
			{
				ReceiveStartingLine();
				ReceiveHeaders();
				RedirectAddress = GetLocation();
				CharacterSet = GetCharacterSet();
				ContentLength = GetContentLength();
				ContentType = GetContentType();
				KeepAliveTimeout = GetKeepAliveTimeout();
				MaximumKeepAliveRequests = GetKeepAliveMax();
			}
			catch (Exception ex)
			{
				HasError = true;
				if (ex is IOException)
				{
					throw NewHttpException(Resources.HttpException_FailedReceiveResponse, ex);
				}
				throw;
			}
			if (ContentLength == 0 || Method == HttpMethod.HEAD || StatusCode == HttpStatusCode.Continue || StatusCode == HttpStatusCode.NoContent || StatusCode == HttpStatusCode.NotModified)
			{
				MessageBodyLoaded = true;
			}
			long num = _receiverHelper.Position;
			if (ContentLength > 0)
			{
				num += ContentLength;
			}
			return num;
		}

		private void ReceiveStartingLine()
		{
			string text;
			do
			{
				text = _receiverHelper.ReadLine();
				if (text.Length == 0)
				{
					HttpException ex = NewHttpException(Resources.HttpException_ReceivedEmptyResponse);
					ex.EmptyMessageBody = true;
					throw ex;
				}
			}
			while (text == "\r\n");
			string text2 = text.Substring("HTTP/", " ");
			string text3 = text.Substring(" ", " ");
			if (text3.Length == 0)
			{
				text3 = text.Substring(" ", "\r\n");
			}
			if (text2.Length == 0 || text3.Length == 0)
			{
				throw NewHttpException(Resources.HttpException_ReceivedEmptyResponse);
			}
			ProtocolVersion = Version.Parse(text2);
			StatusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), text3);
		}

		private void SetCookie(string value)
		{
			if (value.Length == 0)
			{
				return;
			}
			int num = value.IndexOf(';');
			int num2 = value.IndexOf('=');
			if (num2 == -1)
			{
				string message = string.Format(Resources.HttpException_WrongCookie, value, Address.Host);
				throw NewHttpException(message);
			}
			string key = value.Substring(0, num2);
			string text;
			if (num == -1)
			{
				text = value.Substring(num2 + 1);
			}
			else
			{
				text = value.Substring(num2 + 1, num - num2 - 1);
				int num3 = value.IndexOf("expires=");
				if (num3 != -1)
				{
					int num4 = value.IndexOf(';', num3);
					num3 += 8;
					string s = (num4 != -1) ? value.Substring(num3, num4 - num3) : value.Substring(num3);
					if (DateTime.TryParse(s, out DateTime result) && result < DateTime.Now)
					{
						Cookies.Remove(key);
					}
				}
			}
			if (text.Length == 0 || text.Equals("deleted", StringComparison.OrdinalIgnoreCase))
			{
				Cookies.Remove(key);
			}
			else
			{
				Cookies[key] = text;
			}
			_rawCookies[key] = value;
		}

		private void ReceiveHeaders()
		{
			string text;
			while (true)
			{
				text = _receiverHelper.ReadLine();
				if (text == "\r\n")
				{
					return;
				}
				int num = text.IndexOf(':');
				if (num == -1)
				{
					break;
				}
				string text2 = text.Substring(0, num);
				string text3 = text.Substring(num + 1).Trim(' ', '\t', '\r', '\n');
				if (text2.Equals("Set-Cookie", StringComparison.OrdinalIgnoreCase))
				{
					SetCookie(text3);
				}
				else
				{
					_headers[text2] = text3;
				}
			}
			string message = string.Format(Resources.HttpException_WrongHeader, text, Address.Host);
			throw NewHttpException(message);
		}

		private IEnumerable<BytesWraper> GetMessageBodySource()
		{
			if (_headers.ContainsKey("Content-Encoding"))
			{
				return GetMessageBodySourceZip();
			}
			return GetMessageBodySourceStd();
		}

		private IEnumerable<BytesWraper> GetMessageBodySourceStd()
		{
			if (_headers.ContainsKey("Transfer-Encoding"))
			{
				return ReceiveMessageBodyChunked();
			}
			if (ContentLength != -1)
			{
				return ReceiveMessageBody(ContentLength);
			}
			return ReceiveMessageBody(_request.ClientStream);
		}

		private IEnumerable<BytesWraper> GetMessageBodySourceZip()
		{
			if (_headers.ContainsKey("Transfer-Encoding"))
			{
				return ReceiveMessageBodyChunkedZip();
			}
			if (ContentLength != -1)
			{
				return ReceiveMessageBodyZip(ContentLength);
			}
			ZipWraperStream stream = new ZipWraperStream(_request.ClientStream, _receiverHelper);
			return ReceiveMessageBody(GetZipStream(stream));
		}

		private IEnumerable<BytesWraper> ReceiveMessageBody(Stream stream)
		{
			BytesWraper bytesWraper = new BytesWraper();
			int bufferSize = _request.TcpClient.ReceiveBufferSize;
			byte[] buffer = bytesWraper.Value = new byte[bufferSize];
			int begBytesRead = 0;
			if (stream is GZipStream || stream is DeflateStream)
			{
				begBytesRead = stream.Read(buffer, 0, bufferSize);
			}
			else
			{
				if (_receiverHelper.HasData)
				{
					begBytesRead = _receiverHelper.Read(buffer, 0, bufferSize);
				}
				if (begBytesRead < bufferSize)
				{
					begBytesRead += stream.Read(buffer, begBytesRead, bufferSize - begBytesRead);
				}
			}
			bytesWraper.Length = begBytesRead;
			yield return bytesWraper;
			bool isHtml = FindSignature(buffer, begBytesRead, _openHtmlSignature);
			if (isHtml && FindSignature(buffer, begBytesRead, _closeHtmlSignature))
			{
				yield break;
			}
			int num;
			while (true)
			{
				num = stream.Read(buffer, 0, bufferSize);
				if (isHtml)
				{
					if (num == 0)
					{
						WaitData();
						continue;
					}
					if (FindSignature(buffer, num, _closeHtmlSignature))
					{
						break;
					}
				}
				else if (num == 0)
				{
					yield break;
				}
				bytesWraper.Length = num;
				yield return bytesWraper;
			}
			bytesWraper.Length = num;
			yield return bytesWraper;
		}

		private IEnumerable<BytesWraper> ReceiveMessageBody(int contentLength)
		{
			Stream stream = _request.ClientStream;
			BytesWraper bytesWraper = new BytesWraper();
			int bufferSize = _request.TcpClient.ReceiveBufferSize;
			byte[] buffer = bytesWraper.Value = new byte[bufferSize];
			int totalBytesRead = 0;
			while (totalBytesRead != contentLength)
			{
				int num = (!_receiverHelper.HasData) ? stream.Read(buffer, 0, bufferSize) : _receiverHelper.Read(buffer, 0, bufferSize);
				if (num == 0)
				{
					WaitData();
					continue;
				}
				totalBytesRead += num;
				bytesWraper.Length = num;
				yield return bytesWraper;
			}
		}

		private IEnumerable<BytesWraper> ReceiveMessageBodyChunked()
		{
			Stream stream = _request.ClientStream;
			BytesWraper bytesWraper = new BytesWraper();
			int bufferSize = _request.TcpClient.ReceiveBufferSize;
			byte[] buffer = bytesWraper.Value = new byte[bufferSize];
			while (true)
			{
				string text = _receiverHelper.ReadLine();
				if (text == "\r\n")
				{
					continue;
				}
				text = text.Trim(' ', '\r', '\n');
				if (text == string.Empty)
				{
					break;
				}
				int totalBytesRead = 0;
				int blockLength;
				try
				{
					blockLength = Convert.ToInt32(text, 16);
				}
				catch (Exception ex)
				{
					if (ex is FormatException || ex is OverflowException)
					{
						throw NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, text), ex);
					}
					throw;
				}
				if (blockLength == 0)
				{
					break;
				}
				while (totalBytesRead != blockLength)
				{
					int num = blockLength - totalBytesRead;
					if (num > bufferSize)
					{
						num = bufferSize;
					}
					int num2 = (!_receiverHelper.HasData) ? stream.Read(buffer, 0, num) : _receiverHelper.Read(buffer, 0, num);
					if (num2 == 0)
					{
						WaitData();
						continue;
					}
					totalBytesRead += num2;
					bytesWraper.Length = num2;
					yield return bytesWraper;
				}
			}
		}

		private IEnumerable<BytesWraper> ReceiveMessageBodyZip(int contentLength)
		{
			BytesWraper bytesWraper = new BytesWraper();
			ZipWraperStream streamWrapper = new ZipWraperStream(_request.ClientStream, _receiverHelper);
			using (Stream stream = GetZipStream(streamWrapper))
			{
				int bufferSize = _request.TcpClient.ReceiveBufferSize;
				byte[] buffer = bytesWraper.Value = new byte[bufferSize];
				while (true)
				{
					int num = stream.Read(buffer, 0, bufferSize);
					if (num == 0)
					{
						if (streamWrapper.TotalBytesRead == contentLength)
						{
							break;
						}
						WaitData();
					}
					else
					{
						bytesWraper.Length = num;
						yield return bytesWraper;
					}
				}
			}
		}

		private IEnumerable<BytesWraper> ReceiveMessageBodyChunkedZip()
		{
			BytesWraper bytesWraper = new BytesWraper();
			ZipWraperStream streamWrapper = new ZipWraperStream(_request.ClientStream, _receiverHelper);
			using (Stream stream = GetZipStream(streamWrapper))
			{
				int bufferSize = _request.TcpClient.ReceiveBufferSize;
				byte[] buffer = bytesWraper.Value = new byte[bufferSize];
				while (true)
				{
					string text = _receiverHelper.ReadLine();
					if (!(text == "\r\n"))
					{
						text = text.Trim(' ', '\r', '\n');
						if (text == string.Empty)
						{
							break;
						}
						int blockLength;
						try
						{
							blockLength = Convert.ToInt32(text, 16);
						}
						catch (Exception ex)
						{
							if (ex is FormatException || ex is OverflowException)
							{
								throw NewHttpException(string.Format(Resources.HttpException_WrongChunkedBlockLength, text), ex);
							}
							throw;
						}
						if (blockLength == 0)
						{
							break;
						}
						streamWrapper.TotalBytesRead = 0;
						streamWrapper.LimitBytesRead = blockLength;
						while (true)
						{
							int num = stream.Read(buffer, 0, bufferSize);
							if (num == 0)
							{
								if (streamWrapper.TotalBytesRead == blockLength)
								{
									break;
								}
								WaitData();
							}
							else
							{
								bytesWraper.Length = num;
								yield return bytesWraper;
							}
						}
					}
				}
			}
			/*Error near IL_01c5: Unexpected return in MoveNext()*/;
		}

		private bool ConnectionClosed()
		{
			if (_headers.ContainsKey("Connection") && _headers["Connection"].Equals("close", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (_headers.ContainsKey("Proxy-Connection") && _headers["Proxy-Connection"].Equals("close", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			return false;
		}

		private int? GetKeepAliveTimeout()
		{
			if (!_headers.ContainsKey("Keep-Alive"))
			{
				return null;
			}
			string input = _headers["Keep-Alive"];
			Match match = _keepAliveTimeoutRegex.Match(input);
			if (match.Success)
			{
				return int.Parse(match.Groups["value"].Value) * 1000;
			}
			return null;
		}

		private int? GetKeepAliveMax()
		{
			if (!_headers.ContainsKey("Keep-Alive"))
			{
				return null;
			}
			string input = _headers["Keep-Alive"];
			Match match = _keepAliveMaxRegex.Match(input);
			if (match.Success)
			{
				return int.Parse(match.Groups["value"].Value);
			}
			return null;
		}

		private Uri GetLocation()
		{
			if (!_headers.TryGetValue("Location", out string value))
			{
				_headers.TryGetValue("Redirect-Location", out value);
			}
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			Uri address = _request.Address;
			Uri.TryCreate(address, value, out Uri result);
			return result;
		}

		private Encoding GetCharacterSet()
		{
			if (!_headers.ContainsKey("Content-Type"))
			{
				return _request.CharacterSet ?? Encoding.Default;
			}
			string input = _headers["Content-Type"];
			Match match = _contentCharsetRegex.Match(input);
			if (!match.Success)
			{
				return _request.CharacterSet ?? Encoding.Default;
			}
			Group group = match.Groups["value"];
			try
			{
				return Encoding.GetEncoding(group.Value);
			}
			catch (ArgumentException)
			{
				return _request.CharacterSet ?? Encoding.Default;
			}
		}

		private int GetContentLength()
		{
			if (_headers.ContainsKey("Content-Length"))
			{
				int.TryParse(_headers["Content-Length"], out int result);
				return result;
			}
			return -1;
		}

		private string GetContentType()
		{
			if (_headers.ContainsKey("Content-Type"))
			{
				string text = _headers["Content-Type"];
				int num = text.IndexOf(';');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
				return text;
			}
			return string.Empty;
		}

		private void WaitData()
		{
			int num = 0;
			int num2 = (_request.TcpClient.ReceiveTimeout < 10) ? 10 : _request.TcpClient.ReceiveTimeout;
			while (true)
			{
				if (!_request.ClientNetworkStream.DataAvailable)
				{
					if (num >= num2)
					{
						break;
					}
					num += 10;
					Thread.Sleep(10);
					continue;
				}
				return;
			}
			throw NewHttpException(Resources.HttpException_WaitDataTimeout);
		}

		private Stream GetZipStream(Stream stream)
		{
			string text = _headers["Content-Encoding"].ToLower();
			if (!(text == "gzip"))
			{
				if (text == "deflate")
				{
					return new DeflateStream(stream, CompressionMode.Decompress, leaveOpen: true);
				}
				throw new InvalidOperationException(string.Format(Resources.InvalidOperationException_NotSupportedEncodingFormat, text));
			}
			return new GZipStream(stream, CompressionMode.Decompress, leaveOpen: true);
		}

		private bool FindSignature(byte[] source, int sourceLength, byte[] signature)
		{
			int num = sourceLength - signature.Length + 1;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < signature.Length; j++)
				{
					byte b = source[j + i];
					char c = (char)b;
					if (char.IsLetter(c))
					{
						c = char.ToLower(c);
					}
					b = (byte)c;
					if (b != signature[j])
					{
						break;
					}
					if (j == signature.Length - 1)
					{
						return true;
					}
				}
			}
			return false;
		}

		private HttpException NewHttpException(string message, Exception innerException = null)
		{
			return new HttpException(string.Format(message, Address.Host), HttpExceptionStatus.ReceiveFailure, HttpStatusCode.None, innerException);
		}
	}
}
