using System;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;

namespace Extreme.Net
{
	public abstract class ProxyClient : IEquatable<ProxyClient>
	{
		protected ProxyType _type;

		protected string _host;

		protected int _port = 1;

		protected string _username;

		protected string _password;

		protected int _connectTimeout = 60000;

		protected int _readWriteTimeout = 60000;

		public virtual ProxyType Type => _type;

		public virtual string Host
		{
			get
			{
				return _host;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Host");
				}
				if (value.Length == 0)
				{
					throw ExceptionHelper.EmptyString("Host");
				}
				_host = value;
			}
		}

		public virtual int Port
		{
			get
			{
				return _port;
			}
			set
			{
				if (!ExceptionHelper.ValidateTcpPort(value))
				{
					throw ExceptionHelper.WrongTcpPort("Port");
				}
				_port = value;
			}
		}

		public virtual string Username
		{
			get
			{
				return _username;
			}
			set
			{
				if (value != null && value.Length > 255)
				{
					throw new ArgumentOutOfRangeException("Username", string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, 255));
				}
				_username = value;
			}
		}

		public virtual string Password
		{
			get
			{
				return _password;
			}
			set
			{
				if (value != null && value.Length > 255)
				{
					throw new ArgumentOutOfRangeException("Password", string.Format(Resources.ArgumentOutOfRangeException_StringLengthCanNotBeMore, 255));
				}
				_password = value;
			}
		}

		public virtual int ConnectTimeout
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

		public virtual int ReadWriteTimeout
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

		protected internal ProxyClient(ProxyType proxyType)
		{
			_type = proxyType;
		}

		protected internal ProxyClient(ProxyType proxyType, string address, int port)
		{
			_type = proxyType;
			_host = address;
			_port = port;
		}

		protected internal ProxyClient(ProxyType proxyType, string address, int port, string username, string password)
		{
			_type = proxyType;
			_host = address;
			_port = port;
			_username = username;
			_password = password;
		}

		public static ProxyClient Parse(ProxyType proxyType, string proxyAddress)
		{
			if (proxyAddress == null)
			{
				throw new ArgumentNullException("proxyAddress");
			}
			if (proxyAddress.Length == 0)
			{
				throw ExceptionHelper.EmptyString("proxyAddress");
			}
			string[] array = proxyAddress.Split(':');
			int port = 0;
			string host = array[0];
			if (array.Length >= 2)
			{
				try
				{
					port = int.Parse(array[1]);
				}
				catch (Exception ex)
				{
					if (ex is FormatException || ex is OverflowException)
					{
						throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort, ex);
					}
					throw;
				}
				if (!ExceptionHelper.ValidateTcpPort(port))
				{
					throw new FormatException(Resources.InvalidOperationException_ProxyClient_WrongPort);
				}
			}
			string username = null;
			string password = null;
			if (array.Length >= 3)
			{
				username = array[2];
			}
			if (array.Length >= 4)
			{
				password = array[3];
			}
			return ProxyHelper.CreateProxyClient(proxyType, host, port, username, password);
		}

		public static bool TryParse(ProxyType proxyType, string proxyAddress, out ProxyClient result)
		{
			result = null;
			if (string.IsNullOrEmpty(proxyAddress))
			{
				return false;
			}
			string[] array = proxyAddress.Split(':');
			int result2 = 0;
			string host = array[0];
			if (array.Length >= 2 && (!int.TryParse(array[1], out result2) || !ExceptionHelper.ValidateTcpPort(result2)))
			{
				return false;
			}
			string username = null;
			string password = null;
			if (array.Length >= 3)
			{
				username = array[2];
			}
			if (array.Length >= 4)
			{
				password = array[3];
			}
			try
			{
				result = ProxyHelper.CreateProxyClient(proxyType, host, result2, username, password);
			}
			catch (InvalidOperationException)
			{
				return false;
			}
			return true;
		}

		public abstract TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null);

		public override string ToString()
		{
			return $"{_host}:{_port}";
		}

		public virtual string ToExtendedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}:{1}", _host, _port);
			if (!string.IsNullOrEmpty(_username))
			{
				stringBuilder.AppendFormat(":{0}", _username);
				if (!string.IsNullOrEmpty(_password))
				{
					stringBuilder.AppendFormat(":{0}", _password);
				}
			}
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(_host))
			{
				return 0;
			}
			return _host.GetHashCode() ^ _port;
		}

		public bool Equals(ProxyClient proxy)
		{
			if (proxy == null || _host == null)
			{
				return false;
			}
			if (_host.Equals(proxy._host, StringComparison.OrdinalIgnoreCase) && _port == proxy._port)
			{
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			ProxyClient proxyClient = obj as ProxyClient;
			if (proxyClient == null)
			{
				return false;
			}
			return Equals(proxyClient);
		}

		protected TcpClient CreateConnectionToProxy()
		{
			TcpClient tcpClient = null;
			tcpClient = new TcpClient();
			Exception connectException = null;
			ManualResetEventSlim connectDoneEvent = new ManualResetEventSlim();
			try
			{
				tcpClient.BeginConnect(_host, _port, delegate(IAsyncResult ar)
				{
					if (tcpClient.Client != null)
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
					}
				}, tcpClient);
			}
			catch (Exception ex)
			{
				tcpClient.Close();
				if (ex is SocketException || ex is SecurityException)
				{
					throw NewProxyException(Resources.ProxyException_FailedConnect, ex);
				}
				throw;
			}
			if (!connectDoneEvent.Wait(_connectTimeout))
			{
				tcpClient.Close();
				throw NewProxyException(Resources.ProxyException_ConnectTimeout);
			}
			if (connectException != null)
			{
				tcpClient.Close();
				if (connectException is SocketException)
				{
					throw NewProxyException(Resources.ProxyException_FailedConnect, connectException);
				}
				throw connectException;
			}
			if (!tcpClient.Connected)
			{
				tcpClient.Close();
				throw NewProxyException(Resources.ProxyException_FailedConnect);
			}
			tcpClient.SendTimeout = _readWriteTimeout;
			tcpClient.ReceiveTimeout = _readWriteTimeout;
			return tcpClient;
		}

		protected void CheckState()
		{
			if (string.IsNullOrEmpty(_host))
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongHost);
			}
			if (!ExceptionHelper.ValidateTcpPort(_port))
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPort);
			}
			if (_username != null && _username.Length > 255)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongUsername);
			}
			if (_password != null && _password.Length > 255)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ProxyClient_WrongPassword);
			}
		}

		protected ProxyException NewProxyException(string message, Exception innerException = null)
		{
			return new ProxyException(string.Format(message, ToString()), this, innerException);
		}
	}
}
