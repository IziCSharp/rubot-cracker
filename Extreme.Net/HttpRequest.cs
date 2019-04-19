using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Extreme.Net
{
	internal class HttpRequest : IDisposable
	{
		private sealed class HttpWraperStream : Stream
		{
			private Stream _baseStream;

			private int _sendBufferSize;

			public Action<int> BytesReadCallback
			{
				get;
				set;
			}

			public Action<int> BytesWriteCallback
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

			public HttpWraperStream(Stream baseStream, int sendBufferSize)
			{
				_baseStream = baseStream;
				_sendBufferSize = sendBufferSize;
			}

			public override void Flush()
			{
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
				int num = _baseStream.Read(buffer, offset, count);
				if (BytesReadCallback != null)
				{
					BytesReadCallback(num);
				}
				return num;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (BytesWriteCallback == null)
				{
					_baseStream.Write(buffer, offset, count);
					return;
				}
				int num = 0;
				while (count > 0)
				{
					int num2 = 0;
					if (count >= _sendBufferSize)
					{
						num2 = _sendBufferSize;
						_baseStream.Write(buffer, num, num2);
						num += _sendBufferSize;
						count -= _sendBufferSize;
					}
					else
					{
						num2 = count;
						_baseStream.Write(buffer, num, num2);
						count = 0;
					}
					BytesWriteCallback(num2);
				}
			}
		}

		public static readonly Version ProtocolVersion = new Version(1, 1);

		private static readonly List<string> _closedHeaders = new List<string>();

		private HttpResponse _response;

		private TcpClient _connection;

		private Stream _connectionCommonStream;

		private NetworkStream _connectionNetworkStream;

		private ProxyClient _currentProxy;

		private int _redirectionCount;

		private int _maximumAutomaticRedirections = 5;

		private int _connectTimeout = 60000;

		private int _readWriteTimeout = 60000;

		private DateTime _whenConnectionIdle;

		private int _keepAliveTimeout = 30000;

		private int _maximumKeepAliveRequests = 100;

		private int _keepAliveRequestCount;

		private bool _keepAliveReconnected;

		private int _reconnectLimit = 3;

		private int _reconnectDelay = 100;

		private int _reconnectCount;

		private HttpMethod _method;

		private HttpContent _content;

		private readonly Dictionary<string, string> _permanentHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private RequestParams _temporaryParams;

		private RequestParams _temporaryUrlParams;

		private Dictionary<string, string> _temporaryHeaders;

		private MultipartContent _temporaryMultipartContent;

		private long _bytesSent;

		private long _totalBytesSent;

		private long _bytesReceived;

		private long _totalBytesReceived;

		private bool _canReportBytesReceived;

		private EventHandler<UploadProgressChangedEventArgs> _uploadProgressChangedHandler;

		private EventHandler<DownloadProgressChangedEventArgs> _downloadProgressChangedHandler;

		public RemoteCertificateValidationCallback SslCertificateValidatorCallback;

		public static bool DisableProxyForLocalAddress
		{
			get;
			set;
		}

		public static ProxyClient GlobalProxy
		{
			get;
			set;
		}

		public Uri BaseAddress
		{
			get;
			set;
		}

		public Uri Address
		{
			get;
			private set;
		}

		public HttpResponse Response => _response;

		public ProxyClient Proxy
		{
			get;
			set;
		}

		public bool AllowAutoRedirect
		{
			get;
			set;
		}

		public int MaximumAutomaticRedirections
		{
			get
			{
				return _maximumAutomaticRedirections;
			}
			set
			{
				if (value < 1)
				{
					throw ExceptionHelper.CanNotBeLess("MaximumAutomaticRedirections", 1);
				}
				_maximumAutomaticRedirections = value;
			}
		}

		public int ConnectTimeout
		{
			get
			{
				return _connectTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess("ConnectTimeout", 0);
				}
				_connectTimeout = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return _readWriteTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess("ReadWriteTimeout", 0);
				}
				_readWriteTimeout = value;
			}
		}

		public bool IgnoreProtocolErrors
		{
			get;
			set;
		}

		public bool KeepAlive
		{
			get;
			set;
		}

		public int KeepAliveTimeout
		{
			get
			{
				return _keepAliveTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess("KeepAliveTimeout", 0);
				}
				_keepAliveTimeout = value;
			}
		}

		public int MaximumKeepAliveRequests
		{
			get
			{
				return _maximumKeepAliveRequests;
			}
			set
			{
				if (value < 1)
				{
					throw ExceptionHelper.CanNotBeLess("MaximumKeepAliveRequests", 1);
				}
				_maximumKeepAliveRequests = value;
			}
		}

		public bool Reconnect
		{
			get;
			set;
		}

		public int ReconnectLimit
		{
			get
			{
				return _reconnectLimit;
			}
			set
			{
				if (value < 1)
				{
					throw ExceptionHelper.CanNotBeLess("ReconnectLimit", 1);
				}
				_reconnectLimit = value;
			}
		}

		public int ReconnectDelay
		{
			get
			{
				return _reconnectDelay;
			}
			set
			{
				if (value < 0)
				{
					throw ExceptionHelper.CanNotBeLess("ReconnectDelay", 0);
				}
				_reconnectDelay = value;
			}
		}

		public CultureInfo Culture
		{
			get;
			set;
		}

		public Encoding CharacterSet
		{
			get;
			set;
		}

		public bool EnableEncodingContent
		{
			get;
			set;
		}

		public string Username
		{
			get;
			set;
		}

		public string Password
		{
			get;
			set;
		}

		public string UserAgent
		{
			get
			{
				return this["User-Agent"];
			}
			set
			{
				this["User-Agent"] = value;
			}
		}

		public string Referer
		{
			get
			{
				return this["Referer"];
			}
			set
			{
				this["Referer"] = value;
			}
		}

		public string Authorization
		{
			get
			{
				return this["Authorization"];
			}
			set
			{
				this["Authorization"] = value;
			}
		}

		public CookieDictionary Cookies
		{
			get;
			set;
		}

		internal TcpClient TcpClient => _connection;

		internal Stream ClientStream => _connectionCommonStream;

		internal NetworkStream ClientNetworkStream => _connectionNetworkStream;

		private MultipartContent AddedMultipartData
		{
			get
			{
				if (_temporaryMultipartContent == null)
				{
					_temporaryMultipartContent = new MultipartContent();
				}
				return _temporaryMultipartContent;
			}
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
				if (!_permanentHeaders.TryGetValue(headerName, out string value))
				{
					return string.Empty;
				}
				return value;
			}
			set
			{
				if (headerName == null)
				{
					throw new ArgumentNullException("headerName");
				}
				if (headerName.Length == 0)
				{
					throw ExceptionHelper.EmptyString("headerName");
				}
				if (IsClosedHeader(headerName))
				{
					throw new ArgumentException(string.Format(Resources.ArgumentException_HttpRequest_SetNotAvailableHeader, headerName), "headerName");
				}
				if (string.IsNullOrEmpty(value))
				{
					_permanentHeaders.Remove(headerName);
				}
				else
				{
					_permanentHeaders[headerName] = value;
				}
			}
		}

		public string this[HttpHeader header]
		{
			get
			{
				return this[Http.Headers[header]];
			}
			set
			{
				this[Http.Headers[header]] = value;
			}
		}

		public event EventHandler<UploadProgressChangedEventArgs> UploadProgressChanged
		{
			add
			{
				_uploadProgressChangedHandler = (EventHandler<UploadProgressChangedEventArgs>)Delegate.Combine(_uploadProgressChangedHandler, value);
			}
			remove
			{
				_uploadProgressChangedHandler = (EventHandler<UploadProgressChangedEventArgs>)Delegate.Remove(_uploadProgressChangedHandler, value);
			}
		}

		public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged
		{
			add
			{
				_downloadProgressChangedHandler = (EventHandler<DownloadProgressChangedEventArgs>)Delegate.Combine(_downloadProgressChangedHandler, value);
			}
			remove
			{
				_downloadProgressChangedHandler = (EventHandler<DownloadProgressChangedEventArgs>)Delegate.Remove(_downloadProgressChangedHandler, value);
			}
		}

		public HttpRequest()
		{
			Init();
		}

		public HttpRequest(string baseAddress)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (baseAddress.Length == 0)
			{
				throw ExceptionHelper.EmptyString("baseAddress");
			}
			if (!baseAddress.StartsWith("http"))
			{
				baseAddress = "http://" + baseAddress;
			}
			Uri uri = new Uri(baseAddress);
			if (!uri.IsAbsoluteUri)
			{
				throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, "baseAddress");
			}
			BaseAddress = uri;
			Init();
		}

		public HttpRequest(Uri baseAddress)
		{
			if (baseAddress == null)
			{
				throw new ArgumentNullException("baseAddress");
			}
			if (!baseAddress.IsAbsoluteUri)
			{
				throw new ArgumentException(Resources.ArgumentException_OnlyAbsoluteUri, "baseAddress");
			}
			BaseAddress = baseAddress;
			Init();
		}

		public HttpResponse Get(string address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				_temporaryUrlParams = urlParams;
			}
			return Raw(HttpMethod.GET, address);
		}

		public HttpResponse Get(Uri address, RequestParams urlParams = null)
		{
			if (urlParams != null)
			{
				_temporaryUrlParams = urlParams;
			}
			return Raw(HttpMethod.GET, address);
		}

		public async Task<HttpResponse> GetAsync(string address, RequestParams urlParams = null)
		{
			return await Task.Run(() => Get(address, urlParams));
		}

		public async Task<HttpResponse> GetAsync(Uri address, RequestParams urlParams = null)
		{
			return await Task.Run(() => Get(address, urlParams));
		}

		public HttpResponse Post(string address)
		{
			return Raw(HttpMethod.POST, address);
		}

		public async Task<HttpResponse> PostAsync(string address)
		{
			return await Task.Run(() => Post(address));
		}

		public HttpResponse Post(Uri address)
		{
			return Raw(HttpMethod.POST, address);
		}

		public async Task<HttpResponse> PostAsync(Uri address)
		{
			return await Task.Run(() => Post(address));
		}

		public HttpResponse Post(string address, RequestParams reqParams, bool dontEscape = false)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return Raw(HttpMethod.POST, address, new FormUrlEncodedContent(reqParams, dontEscape, CharacterSet));
		}

		public async Task<HttpResponse> PostAsync(string address, RequestParams reqParams, bool dontEscape = false)
		{
			return await Task.Run(() => Post(address, reqParams, dontEscape));
		}

		public HttpResponse Post(Uri address, RequestParams reqParams, bool dontEscape = false)
		{
			if (reqParams == null)
			{
				throw new ArgumentNullException("reqParams");
			}
			return Raw(HttpMethod.POST, address, new FormUrlEncodedContent(reqParams, dontEscape, CharacterSet));
		}

		public async Task<HttpResponse> PostAsync(Uri address, RequestParams reqParams, bool dontEscape = false)
		{
			return await Task.Run(() => Post(address, reqParams, dontEscape));
		}

		public HttpResponse Post(string address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent content = new StringContent(str)
			{
				ContentType = contentType
			};
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(string address, string str, string contentType)
		{
			return await Task.Run(() => Post(address, str, contentType));
		}

		public HttpResponse Post(Uri address, string str, string contentType)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			if (str.Length == 0)
			{
				throw new ArgumentNullException("str");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StringContent content = new StringContent(str)
			{
				ContentType = contentType
			};
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(Uri address, string str, string contentType)
		{
			return await Task.Run(() => Post(address, str, contentType));
		}

		public HttpResponse Post(string address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent content = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(string address, byte[] bytes, string contentType = "application/octet-stream")
		{
			return await Task.Run(() => Post(address, bytes, contentType));
		}

		public HttpResponse Post(Uri address, byte[] bytes, string contentType = "application/octet-stream")
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			BytesContent content = new BytesContent(bytes)
			{
				ContentType = contentType
			};
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(Uri address, byte[] bytes, string contentType = "application/octet-stream")
		{
			return await Task.Run(() => Post(address, bytes, contentType));
		}

		public HttpResponse Post(string address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent content = new StreamContent(stream)
			{
				ContentType = contentType
			};
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(string address, Stream stream, string contentType = "application/octet-stream")
		{
			return await Task.Run(() => Post(address, stream, contentType));
		}

		public HttpResponse Post(Uri address, Stream stream, string contentType = "application/octet-stream")
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (contentType == null)
			{
				throw new ArgumentNullException("contentType");
			}
			if (contentType.Length == 0)
			{
				throw new ArgumentNullException("contentType");
			}
			StreamContent content = new StreamContent(stream)
			{
				ContentType = contentType
			};
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(Uri address, Stream stream, string contentType = "application/octet-stream")
		{
			return await Task.Run(() => Post(address, stream, contentType));
		}

		public HttpResponse Post(string address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return Raw(HttpMethod.POST, address, new FileContent(path));
		}

		public async Task<HttpResponse> PostAsync(string address, string path)
		{
			return await Task.Run(() => Post(address, path));
		}

		public HttpResponse Post(Uri address, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentNullException("path");
			}
			return Raw(HttpMethod.POST, address, new FileContent(path));
		}

		public async Task<HttpResponse> PostAsync(Uri address, string path)
		{
			return await Task.Run(() => Post(address, path));
		}

		public HttpResponse Post(string address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(string address, HttpContent content)
		{
			return await Task.Run(() => Post(address, content));
		}

		public HttpResponse Post(Uri address, HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			return Raw(HttpMethod.POST, address, content);
		}

		public async Task<HttpResponse> PostAsync(Uri address, HttpContent content)
		{
			return await Task.Run(() => Post(address, content));
		}

		public HttpResponse Raw(HttpMethod method, string address, HttpContent content = null)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (address.Length == 0)
			{
				throw ExceptionHelper.EmptyString("address");
			}
			Uri address2 = new Uri(address, UriKind.RelativeOrAbsolute);
			return Raw(method, address2, content);
		}

		public Task<HttpResponse> RawAsync(HttpMethod method, string address, HttpContent content = null)
		{
			return Task.Run(() => Raw(method, address));
		}

		public HttpResponse Raw(HttpMethod method, Uri address, HttpContent content = null)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (!address.IsAbsoluteUri)
			{
				address = GetRequestAddress(BaseAddress, address);
			}
			if (_temporaryUrlParams != null)
			{
				UriBuilder uriBuilder = new UriBuilder(address);
				uriBuilder.Query = Http.ToQueryString(_temporaryUrlParams, dontEscape: true);
				address = uriBuilder.Uri;
			}
			if (content == null)
			{
				if (_temporaryParams != null)
				{
					content = new FormUrlEncodedContent(_temporaryParams, dontEscape: false, CharacterSet);
				}
				else if (_temporaryMultipartContent != null)
				{
					content = _temporaryMultipartContent;
				}
			}
			try
			{
				return Request(method, address, content);
			}
			finally
			{
				content?.Dispose();
				ClearRequestData();
			}
		}

		public HttpRequest AddUrlParam(string name, object value = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (_temporaryUrlParams == null)
			{
				_temporaryUrlParams = new RequestParams();
			}
			_temporaryUrlParams[name] = value;
			return this;
		}

		public HttpRequest AddParam(string name, object value = null)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (_temporaryParams == null)
			{
				_temporaryParams = new RequestParams();
			}
			_temporaryParams[name] = value;
			return this;
		}

		public HttpRequest AddField(string name, object value = null)
		{
			return AddField(name, value, CharacterSet ?? Encoding.UTF8);
		}

		public HttpRequest AddField(string name, object value, Encoding encoding)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			string content = (value == null) ? string.Empty : value.ToString();
			AddedMultipartData.Add(new StringContent(content, encoding), name);
			return this;
		}

		public HttpRequest AddField(string name, byte[] value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			AddedMultipartData.Add(new BytesContent(value), name);
			return this;
		}

		public HttpRequest AddFile(string name, string fileName, byte[] value)
		{
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
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			AddedMultipartData.Add(new BytesContent(value), name, fileName);
			return this;
		}

		public HttpRequest AddFile(string name, string fileName, string contentType, byte[] value)
		{
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
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			AddedMultipartData.Add(new BytesContent(value), name, fileName, contentType);
			return this;
		}

		public HttpRequest AddFile(string name, string fileName, Stream stream)
		{
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
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			AddedMultipartData.Add(new StreamContent(stream), name, fileName);
			return this;
		}

		public HttpRequest AddFile(string name, string fileName, string path)
		{
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
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw ExceptionHelper.EmptyString("path");
			}
			AddedMultipartData.Add(new FileContent(path), name, fileName);
			return this;
		}

		public HttpRequest AddFile(string name, string path)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw ExceptionHelper.EmptyString("path");
			}
			AddedMultipartData.Add(new FileContent(path), name, Path.GetFileName(path));
			return this;
		}

		public HttpRequest AddHeader(string name, string value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw ExceptionHelper.EmptyString("name");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				throw ExceptionHelper.EmptyString("value");
			}
			if (IsClosedHeader(name))
			{
				throw new ArgumentException(string.Format(Resources.ArgumentException_HttpRequest_SetNotAvailableHeader, name), "name");
			}
			if (_temporaryHeaders == null)
			{
				_temporaryHeaders = new Dictionary<string, string>();
			}
			_temporaryHeaders[name] = value;
			return this;
		}

		public HttpRequest AddHeader(HttpHeader header, string value)
		{
			AddHeader(Http.Headers[header], value);
			return this;
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}

		public bool ContainsCookie(string name)
		{
			if (Cookies == null)
			{
				return false;
			}
			return Cookies.ContainsKey(name);
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
			return _permanentHeaders.ContainsKey(headerName);
		}

		public bool ContainsHeader(HttpHeader header)
		{
			return ContainsHeader(Http.Headers[header]);
		}

		public Dictionary<string, string>.Enumerator EnumerateHeaders()
		{
			return _permanentHeaders.GetEnumerator();
		}

		public void ClearAllHeaders()
		{
			_permanentHeaders.Clear();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _connection != null)
			{
				_connection.Close();
				_connection = null;
				_connectionCommonStream = null;
				_connectionNetworkStream = null;
				_keepAliveRequestCount = 0;
			}
		}

		protected virtual void OnUploadProgressChanged(UploadProgressChangedEventArgs e)
		{
			_uploadProgressChangedHandler?.Invoke(this, e);
		}

		protected virtual void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
		{
			_downloadProgressChangedHandler?.Invoke(this, e);
		}

		private void Init()
		{
			KeepAlive = true;
			AllowAutoRedirect = true;
			EnableEncodingContent = true;
			_response = new HttpResponse(this);
		}

		private Uri GetRequestAddress(Uri baseAddress, Uri address)
		{
			Uri result = address;
			if (baseAddress == null)
			{
				UriBuilder uriBuilder = new UriBuilder(address.OriginalString);
				result = uriBuilder.Uri;
			}
			else
			{
				Uri.TryCreate(baseAddress, address, out result);
			}
			return result;
		}

		private HttpResponse Request(HttpMethod method, Uri address, HttpContent content)
		{
			_method = method;
			_content = content;
			CloseConnectionIfNeeded();
			Uri address2 = Address;
			Address = address;
			bool flag = false;
			try
			{
				flag = TryCreateConnectionOrUseExisting(address, address2);
			}
			catch (HttpException)
			{
				if (!CanReconnect())
				{
					throw;
				}
				return ReconnectAfterFail();
			}
			if (flag)
			{
				_keepAliveRequestCount = 1;
			}
			else
			{
				_keepAliveRequestCount++;
			}
			try
			{
				SendRequestData(method);
			}
			catch (SecurityException innerException)
			{
				throw NewHttpException(Resources.HttpException_FailedSendRequest, innerException, HttpExceptionStatus.SendFailure);
			}
			catch (IOException innerException2)
			{
				if (!CanReconnect())
				{
					throw NewHttpException(Resources.HttpException_FailedSendRequest, innerException2, HttpExceptionStatus.SendFailure);
				}
				return ReconnectAfterFail();
			}
			try
			{
				ReceiveResponseHeaders(method);
			}
			catch (HttpException ex2)
			{
				if (CanReconnect())
				{
					return ReconnectAfterFail();
				}
				if (!KeepAlive || _keepAliveReconnected || flag || !ex2.EmptyMessageBody)
				{
					throw;
				}
				return KeepAliveReconect();
			}
			_response.ReconnectCount = _reconnectCount;
			_reconnectCount = 0;
			_keepAliveReconnected = false;
			_whenConnectionIdle = DateTime.Now;
			if (!IgnoreProtocolErrors)
			{
				CheckStatusCode(_response.StatusCode);
			}
			if (AllowAutoRedirect && _response.HasRedirect)
			{
				if (++_redirectionCount > _maximumAutomaticRedirections)
				{
					throw NewHttpException(Resources.HttpException_LimitRedirections);
				}
				ClearRequestData();
				return Request(HttpMethod.GET, _response.RedirectAddress, null);
			}
			_redirectionCount = 0;
			return _response;
		}

		private void CloseConnectionIfNeeded()
		{
			if (_connection != null && !_response.HasError && !_response.MessageBodyLoaded)
			{
				try
				{
					_response.None();
				}
				catch (HttpException)
				{
					Dispose();
				}
			}
		}

		private bool TryCreateConnectionOrUseExisting(Uri address, Uri previousAddress)
		{
			ProxyClient proxy = GetProxy();
			bool flag = _connection != null;
			bool flag2 = _currentProxy != proxy;
			bool flag3 = previousAddress == null || previousAddress.Port != address.Port || previousAddress.Host != address.Host || previousAddress.Scheme != address.Scheme;
			if ((!flag | flag2 | flag3) || _response.HasError || KeepAliveLimitIsReached())
			{
				_currentProxy = proxy;
				Dispose();
				CreateConnection(address);
				return true;
			}
			return false;
		}

		private bool KeepAliveLimitIsReached()
		{
			if (!KeepAlive)
			{
				return false;
			}
			int num = _response.MaximumKeepAliveRequests ?? _maximumKeepAliveRequests;
			if (_keepAliveRequestCount >= num)
			{
				return true;
			}
			int num2 = _response.KeepAliveTimeout ?? _keepAliveTimeout;
			DateTime t = _whenConnectionIdle.AddMilliseconds(num2);
			if (t < DateTime.Now)
			{
				return true;
			}
			return false;
		}

		private void SendRequestData(HttpMethod method)
		{
			long num = 0L;
			string contentType = string.Empty;
			if (CanContainsRequestBody(method) && _content != null)
			{
				contentType = _content.ContentType;
				num = _content.CalculateContentLength();
			}
			string s = GenerateStartingLine(method);
			string s2 = GenerateHeaders(method, num, contentType);
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			byte[] bytes2 = Encoding.ASCII.GetBytes(s2);
			_bytesSent = 0L;
			_totalBytesSent = bytes.Length + bytes2.Length + num;
			_connectionCommonStream.Write(bytes, 0, bytes.Length);
			_connectionCommonStream.Write(bytes2, 0, bytes2.Length);
			if (_content != null && num > 0)
			{
				_content.WriteTo(_connectionCommonStream);
			}
		}

		private void ReceiveResponseHeaders(HttpMethod method)
		{
			_canReportBytesReceived = false;
			_bytesReceived = 0L;
			_totalBytesReceived = _response.LoadResponse(method);
			_canReportBytesReceived = true;
		}

		private bool CanReconnect()
		{
			if (Reconnect)
			{
				return _reconnectCount < _reconnectLimit;
			}
			return false;
		}

		private HttpResponse ReconnectAfterFail()
		{
			Dispose();
			Thread.Sleep(_reconnectDelay);
			_reconnectCount++;
			return Request(_method, Address, _content);
		}

		private HttpResponse KeepAliveReconect()
		{
			Dispose();
			_keepAliveReconnected = true;
			return Request(_method, Address, _content);
		}

		private void CheckStatusCode(HttpStatusCode statusCode)
		{
			if (statusCode >= HttpStatusCode.BadRequest && statusCode < HttpStatusCode.InternalServerError)
			{
				throw new HttpException(string.Format(Resources.HttpException_ClientError, (int)statusCode), HttpExceptionStatus.ProtocolError, _response.StatusCode);
			}
			if (statusCode >= HttpStatusCode.InternalServerError)
			{
				throw new HttpException(string.Format(Resources.HttpException_SeverError, (int)statusCode), HttpExceptionStatus.ProtocolError, _response.StatusCode);
			}
		}

		private bool CanContainsRequestBody(HttpMethod method)
		{
			if (method != HttpMethod.PUT && method != HttpMethod.POST)
			{
				return method == HttpMethod.DELETE;
			}
			return true;
		}

		private ProxyClient GetProxy()
		{
			if (DisableProxyForLocalAddress)
			{
				try
				{
					IPAddress obj = IPAddress.Parse("127.0.0.1");
					IPAddress[] hostAddresses = Dns.GetHostAddresses(Address.Host);
					IPAddress[] array = hostAddresses;
					foreach (IPAddress iPAddress in array)
					{
						if (iPAddress.Equals(obj))
						{
							return null;
						}
					}
				}
				catch (Exception ex)
				{
					if (ex is SocketException || ex is ArgumentException)
					{
						throw NewHttpException(Resources.HttpException_FailedGetHostAddresses, ex);
					}
					throw;
				}
			}
			return Proxy ?? GlobalProxy;
		}

		private TcpClient CreateTcpConnection(string host, int port)
		{
			TcpClient tcpClient;
			if (_currentProxy == null)
			{
				tcpClient = new TcpClient();
				Exception connectException = null;
				ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
				try
				{
					tcpClient.BeginConnect(host, port, delegate(IAsyncResult ar)
					{
						try
						{
							tcpClient.EndConnect(ar);
						}
						catch (Exception ex2)
						{
							Exception ex3 = connectException = ex2;
						}
						connectDoneEvent.Set();
					}, tcpClient);
				}
				catch (Exception ex)
				{
					tcpClient.Close();
					if (ex is SocketException || ex is SecurityException)
					{
						throw NewHttpException(Resources.HttpException_FailedConnect, ex, HttpExceptionStatus.ConnectFailure);
					}
					throw;
				}
				if (!connectDoneEvent.Wait(_connectTimeout))
				{
					tcpClient.Close();
					throw NewHttpException(Resources.HttpException_ConnectTimeout, null, HttpExceptionStatus.ConnectFailure);
				}
				if (connectException != null)
				{
					tcpClient.Close();
					if (connectException is SocketException)
					{
						throw NewHttpException(Resources.HttpException_FailedConnect, connectException, HttpExceptionStatus.ConnectFailure);
					}
					throw connectException;
				}
				if (!tcpClient.Connected)
				{
					tcpClient.Close();
					throw NewHttpException(Resources.HttpException_FailedConnect, null, HttpExceptionStatus.ConnectFailure);
				}
				tcpClient.SendTimeout = _readWriteTimeout;
				tcpClient.ReceiveTimeout = _readWriteTimeout;
			}
			else
			{
				try
				{
					tcpClient = _currentProxy.CreateConnection(host, port);
				}
				catch (ProxyException innerException)
				{
					throw NewHttpException(Resources.HttpException_FailedConnect, innerException, HttpExceptionStatus.ConnectFailure);
				}
			}
			return tcpClient;
		}

		private void CreateConnection(Uri address)
		{
			_connection = CreateTcpConnection(address.Host, address.Port);
			_connectionNetworkStream = _connection.GetStream();
			if (address.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					SslStream sslStream = (SslCertificateValidatorCallback != null) ? new SslStream(_connectionNetworkStream, leaveInnerStreamOpen: false, SslCertificateValidatorCallback) : new SslStream(_connectionNetworkStream, leaveInnerStreamOpen: false, Http.AcceptAllCertificationsCallback);
					sslStream.AuthenticateAsClient(address.Host);
					_connectionCommonStream = sslStream;
				}
				catch (Exception ex)
				{
					if (ex is IOException || ex is AuthenticationException)
					{
						throw NewHttpException(Resources.HttpException_FailedSslConnect, ex, HttpExceptionStatus.ConnectFailure);
					}
					throw;
				}
			}
			else
			{
				_connectionCommonStream = _connectionNetworkStream;
			}
			if (_uploadProgressChangedHandler != null || _downloadProgressChangedHandler != null)
			{
				HttpWraperStream httpWraperStream = new HttpWraperStream(_connectionCommonStream, _connection.SendBufferSize);
				if (_uploadProgressChangedHandler != null)
				{
					httpWraperStream.BytesWriteCallback = ReportBytesSent;
				}
				if (_downloadProgressChangedHandler != null)
				{
					httpWraperStream.BytesReadCallback = ReportBytesReceived;
				}
				_connectionCommonStream = httpWraperStream;
			}
		}

		private string GenerateStartingLine(HttpMethod method)
		{
			return string.Format(arg1: (_currentProxy == null || (_currentProxy.Type != 0 && _currentProxy.Type != ProxyType.Chain)) ? Address.PathAndQuery : Address.AbsoluteUri, format: "{0} {1} HTTP/{2}\r\n", arg0: method, arg2: ProtocolVersion);
		}

		private string GenerateHeaders(HttpMethod method, long contentLength = 0L, string contentType = null)
		{
			Dictionary<string, string> dictionary = GenerateCommonHeaders(method, contentLength, contentType);
			MergeHeaders(dictionary, _permanentHeaders);
			if (_temporaryHeaders != null && _temporaryHeaders.Count > 0)
			{
				MergeHeaders(dictionary, _temporaryHeaders);
			}
			if (Cookies != null && Cookies.Count != 0 && !dictionary.ContainsKey("Cookie"))
			{
				dictionary["Cookie"] = Cookies.ToString();
			}
			return ToHeadersString(dictionary);
		}

		private Dictionary<string, string> GenerateCommonHeaders(HttpMethod method, long contentLength = 0L, string contentType = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			if (Address.IsDefaultPort)
			{
				dictionary["Host"] = Address.Host;
			}
			else
			{
				dictionary["Host"] = $"{Address.Host}:{Address.Port}";
			}
			HttpProxyClient httpProxyClient = null;
			if (_currentProxy != null && _currentProxy.Type == ProxyType.Http)
			{
				httpProxyClient = (_currentProxy as HttpProxyClient);
			}
			else if (_currentProxy != null && _currentProxy.Type == ProxyType.Chain)
			{
				httpProxyClient = FindHttpProxyInChain(_currentProxy as ChainProxyClient);
			}
			if (httpProxyClient != null)
			{
				if (KeepAlive)
				{
					dictionary["Proxy-Connection"] = "keep-alive";
				}
				else
				{
					dictionary["Proxy-Connection"] = "close";
				}
				if (!string.IsNullOrEmpty(httpProxyClient.Username) || !string.IsNullOrEmpty(httpProxyClient.Password))
				{
					dictionary["Proxy-Authorization"] = GetProxyAuthorizationHeader(httpProxyClient);
				}
			}
			else if (KeepAlive)
			{
				dictionary["Connection"] = "keep-alive";
			}
			else
			{
				dictionary["Connection"] = "close";
			}
			if (!string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password))
			{
				dictionary["Authorization"] = GetAuthorizationHeader();
			}
			if (EnableEncodingContent)
			{
				dictionary["Accept-Encoding"] = "gzip,deflate";
			}
			if (Culture != null)
			{
				dictionary["Accept-Language"] = GetLanguageHeader();
			}
			if (CharacterSet != null)
			{
				dictionary["Accept-Charset"] = GetCharsetHeader();
			}
			if (CanContainsRequestBody(method))
			{
				if (contentLength > 0)
				{
					dictionary["Content-Type"] = contentType;
				}
				dictionary["Content-Length"] = contentLength.ToString();
			}
			return dictionary;
		}

		private string GetAuthorizationHeader()
		{
			string arg = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
			return $"Basic {arg}";
		}

		private string GetProxyAuthorizationHeader(HttpProxyClient httpProxy)
		{
			string arg = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{httpProxy.Username}:{httpProxy.Password}"));
			return $"Basic {arg}";
		}

		private string GetLanguageHeader()
		{
			string text = (Culture == null) ? CultureInfo.CurrentCulture.Name : Culture.Name;
			if (text.StartsWith("en"))
			{
				return text;
			}
			return $"{text},{text.Substring(0, 2)};q=0.8,en-US;q=0.6,en;q=0.4";
		}

		private string GetCharsetHeader()
		{
			if (CharacterSet == Encoding.UTF8)
			{
				return "utf-8;q=0.7,*;q=0.3";
			}
			string arg = (CharacterSet != null) ? CharacterSet.WebName : Encoding.Default.WebName;
			return $"{arg},utf-8;q=0.7,*;q=0.3";
		}

		private void MergeHeaders(Dictionary<string, string> destination, Dictionary<string, string> source)
		{
			foreach (KeyValuePair<string, string> item in source)
			{
				destination[item.Key] = item.Value;
			}
		}

		private HttpProxyClient FindHttpProxyInChain(ChainProxyClient chainProxy)
		{
			HttpProxyClient httpProxyClient = null;
			foreach (ProxyClient proxy in chainProxy.Proxies)
			{
				if (proxy.Type == ProxyType.Http)
				{
					httpProxyClient = (proxy as HttpProxyClient);
					if (!string.IsNullOrEmpty(httpProxyClient.Username) || !string.IsNullOrEmpty(httpProxyClient.Password))
					{
						return httpProxyClient;
					}
				}
				else if (proxy.Type == ProxyType.Chain)
				{
					HttpProxyClient httpProxyClient2 = FindHttpProxyInChain(proxy as ChainProxyClient);
					if (httpProxyClient2 != null && (!string.IsNullOrEmpty(httpProxyClient2.Username) || !string.IsNullOrEmpty(httpProxyClient2.Password)))
					{
						return httpProxyClient2;
					}
				}
			}
			return httpProxyClient;
		}

		private string ToHeadersString(Dictionary<string, string> headers)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> header in headers)
			{
				stringBuilder.AppendFormat("{0}: {1}\r\n", header.Key, header.Value);
			}
			stringBuilder.AppendLine();
			return stringBuilder.ToString();
		}

		private void ReportBytesSent(int bytesSent)
		{
			_bytesSent += bytesSent;
			OnUploadProgressChanged(new UploadProgressChangedEventArgs(_bytesSent, _totalBytesSent));
		}

		private void ReportBytesReceived(int bytesReceived)
		{
			_bytesReceived += bytesReceived;
			if (_canReportBytesReceived)
			{
				OnDownloadProgressChanged(new DownloadProgressChangedEventArgs(_bytesReceived, _totalBytesReceived));
			}
		}

		private bool IsClosedHeader(string name)
		{
			return _closedHeaders.Contains(name, StringComparer.OrdinalIgnoreCase);
		}

		private void ClearRequestData()
		{
			_content = null;
			_temporaryUrlParams = null;
			_temporaryParams = null;
			_temporaryMultipartContent = null;
			_temporaryHeaders = null;
		}

		private HttpException NewHttpException(string message, Exception innerException = null, HttpExceptionStatus status = HttpExceptionStatus.Other)
		{
			return new HttpException(string.Format(message, Address.Host), status, HttpStatusCode.None, innerException);
		}
	}
}
