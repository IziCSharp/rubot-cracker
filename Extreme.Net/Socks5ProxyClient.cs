using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Extreme.Net
{
	public class Socks5ProxyClient : ProxyClient
	{
		private const int DefaultPort = 1080;

		private const byte VersionNumber = 5;

		private const byte Reserved = 0;

		private const byte AuthMethodNoAuthenticationRequired = 0;

		private const byte AuthMethodGssapi = 1;

		private const byte AuthMethodUsernamePassword = 2;

		private const byte AuthMethodIanaAssignedRangeBegin = 3;

		private const byte AuthMethodIanaAssignedRangeEnd = 127;

		private const byte AuthMethodReservedRangeBegin = 128;

		private const byte AuthMethodReservedRangeEnd = 254;

		private const byte AuthMethodReplyNoAcceptableMethods = byte.MaxValue;

		private const byte CommandConnect = 1;

		private const byte CommandBind = 2;

		private const byte CommandUdpAssociate = 3;

		private const byte CommandReplySucceeded = 0;

		private const byte CommandReplyGeneralSocksServerFailure = 1;

		private const byte CommandReplyConnectionNotAllowedByRuleset = 2;

		private const byte CommandReplyNetworkUnreachable = 3;

		private const byte CommandReplyHostUnreachable = 4;

		private const byte CommandReplyConnectionRefused = 5;

		private const byte CommandReplyTTLExpired = 6;

		private const byte CommandReplyCommandNotSupported = 7;

		private const byte CommandReplyAddressTypeNotSupported = 8;

		private const byte AddressTypeIPV4 = 1;

		private const byte AddressTypeDomainName = 3;

		private const byte AddressTypeIPV6 = 4;

		public Socks5ProxyClient()
			: this(null)
		{
		}

		public Socks5ProxyClient(string host)
			: this(host, 1080)
		{
		}

		public Socks5ProxyClient(string host, int port)
			: this(host, port, string.Empty, string.Empty)
		{
		}

		public Socks5ProxyClient(string host, int port, string username, string password)
			: base(ProxyType.Socks5, host, port, username, password)
		{
		}

		public static Socks5ProxyClient Parse(string proxyAddress)
		{
			return ProxyClient.Parse(ProxyType.Socks5, proxyAddress) as Socks5ProxyClient;
		}

		public static bool TryParse(string proxyAddress, out Socks5ProxyClient result)
		{
			if (ProxyClient.TryParse(ProxyType.Socks5, proxyAddress, out ProxyClient result2))
			{
				result = (result2 as Socks5ProxyClient);
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
			try
			{
				NetworkStream stream = tcpClient2.GetStream();
				InitialNegotiation(stream);
				SendCommand(stream, 1, destinationHost, destinationPort);
				return tcpClient2;
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
		}

		private void InitialNegotiation(NetworkStream nStream)
		{
			byte b = (byte)((!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password)) ? 2 : 0);
			byte[] array = new byte[3]
			{
				5,
				1,
				b
			};
			nStream.Write(array, 0, array.Length);
			byte[] array2 = new byte[2];
			nStream.Read(array2, 0, array2.Length);
			byte b2 = array2[1];
			if (b == 2 && b2 == 2)
			{
				SendUsernameAndPassword(nStream);
			}
			else if (b2 != 0)
			{
				HandleCommandError(b2);
			}
		}

		private void SendUsernameAndPassword(NetworkStream nStream)
		{
			byte[] array = string.IsNullOrEmpty(_username) ? new byte[0] : Encoding.ASCII.GetBytes(_username);
			byte[] array2 = string.IsNullOrEmpty(_password) ? new byte[0] : Encoding.ASCII.GetBytes(_password);
			byte[] array3 = new byte[array.Length + array2.Length + 3];
			array3[0] = 1;
			array3[1] = (byte)array.Length;
			array.CopyTo(array3, 2);
			array3[2 + array.Length] = (byte)array2.Length;
			array2.CopyTo(array3, 3 + array.Length);
			nStream.Write(array3, 0, array3.Length);
			byte[] array4 = new byte[2];
			nStream.Read(array4, 0, array4.Length);
			if (array4[1] != 0)
			{
				throw NewProxyException(Resources.ProxyException_Socks5_FailedAuthOn);
			}
		}

		private void SendCommand(NetworkStream nStream, byte command, string destinationHost, int destinationPort)
		{
			byte addressType = GetAddressType(destinationHost);
			byte[] addressBytes = GetAddressBytes(addressType, destinationHost);
			byte[] portBytes = GetPortBytes(destinationPort);
			byte[] array = new byte[4 + addressBytes.Length + 2];
			array[0] = 5;
			array[1] = command;
			array[2] = 0;
			array[3] = addressType;
			addressBytes.CopyTo(array, 4);
			portBytes.CopyTo(array, 4 + addressBytes.Length);
			nStream.Write(array, 0, array.Length);
			byte[] array2 = new byte[255];
			nStream.Read(array2, 0, array2.Length);
			byte b = array2[1];
			if (b != 0)
			{
				HandleCommandError(b);
			}
		}

		private byte GetAddressType(string host)
		{
			IPAddress address = null;
			if (!IPAddress.TryParse(host, out address))
			{
				return 3;
			}
			switch (address.AddressFamily)
			{
			case AddressFamily.InterNetwork:
				return 1;
			case AddressFamily.InterNetworkV6:
				return 4;
			default:
				throw new ProxyException(string.Format(Resources.ProxyException_NotSupportedAddressType, host, Enum.GetName(typeof(AddressFamily), address.AddressFamily), ToString()), this);
			}
		}

		private byte[] GetAddressBytes(byte addressType, string host)
		{
			switch (addressType)
			{
			case 1:
			case 4:
				return IPAddress.Parse(host).GetAddressBytes();
			case 3:
			{
				byte[] array = new byte[host.Length + 1];
				array[0] = (byte)host.Length;
				Encoding.ASCII.GetBytes(host).CopyTo(array, 1);
				return array;
			}
			default:
				return null;
			}
		}

		private byte[] GetPortBytes(int port)
		{
			return new byte[2]
			{
				(byte)(port / 256),
				(byte)(port % 256)
			};
		}

		private void HandleCommandError(byte command)
		{
			string arg;
			switch (command)
			{
			case byte.MaxValue:
				arg = Resources.Socks5_AuthMethodReplyNoAcceptableMethods;
				break;
			case 1:
				arg = Resources.Socks5_CommandReplyGeneralSocksServerFailure;
				break;
			case 2:
				arg = Resources.Socks5_CommandReplyConnectionNotAllowedByRuleset;
				break;
			case 3:
				arg = Resources.Socks5_CommandReplyNetworkUnreachable;
				break;
			case 4:
				arg = Resources.Socks5_CommandReplyHostUnreachable;
				break;
			case 5:
				arg = Resources.Socks5_CommandReplyConnectionRefused;
				break;
			case 6:
				arg = Resources.Socks5_CommandReplyTTLExpired;
				break;
			case 7:
				arg = Resources.Socks5_CommandReplyCommandNotSupported;
				break;
			case 8:
				arg = Resources.Socks5_CommandReplyAddressTypeNotSupported;
				break;
			default:
				arg = Resources.Socks_UnknownError;
				break;
			}
			string message = string.Format(Resources.ProxyException_CommandError, arg, ToString());
			throw new ProxyException(message, this);
		}
	}
}
