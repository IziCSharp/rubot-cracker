using System.Net.Sockets;
using System.Text;

namespace Extreme.Net
{
	public class Socks4aProxyClient : Socks4ProxyClient
	{
		public Socks4aProxyClient()
			: this(null)
		{
		}

		public Socks4aProxyClient(string host)
			: this(host, 1080)
		{
		}

		public Socks4aProxyClient(string host, int port)
			: this(host, port, string.Empty)
		{
		}

		public Socks4aProxyClient(string host, int port, string username)
			: base(host, port, username)
		{
			_type = ProxyType.Socks4a;
		}

		public new static Socks4aProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.Socks4a, proxyAddress) as Socks4aProxyClient;
		}

		public static bool TryParse(string proxyAddress, out Socks4aProxyClient result)
		{
			if (ProxyClient.TryParse(ProxyType.Socks4a, proxyAddress, out ProxyClient result2))
			{
				result = (result2 as Socks4aProxyClient);
				return true;
			}
			result = null;
			return false;
		}

		protected internal override void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
		{
			byte[] portBytes = GetPortBytes(destinationPort);
			byte[] array = new byte[4]
			{
				0,
				0,
				0,
				1
			};
			byte[] array2 = string.IsNullOrEmpty(_username) ? new byte[0] : Encoding.ASCII.GetBytes(_username);
			byte[] bytes = Encoding.ASCII.GetBytes(destinationHost);
			byte[] array3 = new byte[10 + array2.Length + bytes.Length];
			array3[0] = 4;
			array3[1] = command;
			portBytes.CopyTo(array3, 2);
			array.CopyTo(array3, 4);
			array2.CopyTo(array3, 8);
			array3[8 + array2.Length] = 0;
			bytes.CopyTo(array3, 9 + array2.Length);
			array3[9 + array2.Length + bytes.Length] = 0;
			nStream.Write(array3, 0, array3.Length);
			byte[] array4 = new byte[8];
			nStream.Read(array4, 0, 8);
			byte b = array4[1];
			if (b != 90)
			{
				HandleCommandError(b);
			}
		}
	}
}
