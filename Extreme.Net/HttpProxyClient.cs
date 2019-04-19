using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Extreme.Net
{
	public class HttpProxyClient : ProxyClient
	{
		private const int BufferSize = 50;

		private const int DefaultPort = 8080;

		public HttpProxyClient()
			: this(null)
		{
		}

		public HttpProxyClient(string host)
			: this(host, 8080)
		{
		}

		public HttpProxyClient(string host, int port)
			: this(host, port, string.Empty, string.Empty)
		{
		}

		public HttpProxyClient(string host, int port, string username, string password)
			: base(ProxyType.Http, host, port, username, password)
		{
		}

		public static HttpProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.Http, proxyAddress) as HttpProxyClient;
		}

		public static bool TryParse(string proxyAddress, out HttpProxyClient result)
		{
			if (ProxyClient.TryParse(ProxyType.Http, proxyAddress, out ProxyClient result2))
			{
				result = (result2 as HttpProxyClient);
				return true;
			}
			result = null;
			return false;
		}

		public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
		{
			CheckState();
			if (destinationHost == null)
			{
				throw new ArgumentNullException("destinationHost");
			}
			if (destinationHost.Length == 0)
			{
				throw ExceptionHelper.EmptyString("destinationHost");
			}
			if (!ExceptionHelper.ValidateTcpPort(destinationPort))
			{
				throw ExceptionHelper.WrongTcpPort("destinationPort");
			}
			TcpClient tcpClient2 = tcpClient;
			if (tcpClient2 == null)
			{
				tcpClient2 = CreateConnectionToProxy();
			}
			if (destinationPort != 80)
			{
				HttpStatusCode httpStatusCode = HttpStatusCode.OK;
				try
				{
					NetworkStream stream = tcpClient2.GetStream();
					SendConnectionCommand(stream, destinationHost, destinationPort);
					httpStatusCode = ReceiveResponse(stream);
				}
				catch (Exception ex)
				{
					tcpClient2.Close();
					if (ex is IOException || ex is SocketException)
					{
						throw NewProxyException(Resources.ProxyException_Error, ex);
					}
					throw;
				}
				if (httpStatusCode != HttpStatusCode.OK)
				{
					tcpClient2.Close();
					throw new ProxyException(string.Format(Resources.ProxyException_ReceivedWrongStatusCode, httpStatusCode, ToString()), this);
				}
			}
			return tcpClient2;
		}

		private string GenerateAuthorizationHeader()
		{
			if (!string.IsNullOrEmpty(_username) || !string.IsNullOrEmpty(_password))
			{
				string arg = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}"));
				return $"Proxy-Authorization: Basic {arg}\r\n";
			}
			return string.Empty;
		}

		private void SendConnectionCommand(NetworkStream nStream, string destinationHost, int destinationPort)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("CONNECT {0}:{1} HTTP/1.1\r\n", destinationHost, destinationPort);
			stringBuilder.AppendFormat(GenerateAuthorizationHeader());
			stringBuilder.AppendLine();
			byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
			nStream.Write(bytes, 0, bytes.Length);
		}

		private HttpStatusCode ReceiveResponse(NetworkStream nStream)
		{
			byte[] array = new byte[50];
			StringBuilder stringBuilder = new StringBuilder();
			WaitData(nStream);
			do
			{
				int count = nStream.Read(array, 0, 50);
				stringBuilder.Append(Encoding.ASCII.GetString(array, 0, count));
			}
			while (nStream.DataAvailable);
			string text = stringBuilder.ToString();
			if (text.Length == 0)
			{
				throw NewProxyException(Resources.ProxyException_ReceivedEmptyResponse);
			}
			string text2 = text.Substring(" ", "\r\n");
			int num = text2.IndexOf(' ');
			if (num == -1)
			{
				throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
			}
			string text3 = text2.Substring(0, num);
			if (text3.Length == 0)
			{
				throw NewProxyException(Resources.ProxyException_ReceivedWrongResponse);
			}
			return (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), text3);
		}

		private void WaitData(NetworkStream nStream)
		{
			int num = 0;
			int num2 = (nStream.ReadTimeout < 10) ? 10 : nStream.ReadTimeout;
			while (true)
			{
				if (!nStream.DataAvailable)
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
			throw NewProxyException(Resources.ProxyException_WaitDataTimeout);
		}
	}
}
