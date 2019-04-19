using System;

namespace Extreme.Net
{
	internal static class ProxyHelper
	{
		public static ProxyClient CreateProxyClient(ProxyType proxyType, string host = null, int port = 0, string username = null, string password = null)
		{
			switch (proxyType)
			{
			case ProxyType.Http:
				if (port != 0)
				{
					return new HttpProxyClient(host, port, username, password);
				}
				return new HttpProxyClient(host);
			case ProxyType.Socks4:
				if (port != 0)
				{
					return new Socks4ProxyClient(host, port, username);
				}
				return new Socks4ProxyClient(host);
			case ProxyType.Socks4a:
				if (port != 0)
				{
					return new Socks4aProxyClient(host, port, username);
				}
				return new Socks4aProxyClient(host);
			case ProxyType.Socks5:
				if (port != 0)
				{
					return new Socks5ProxyClient(host, port, username, password);
				}
				return new Socks5ProxyClient(host);
			default:
				throw new InvalidOperationException();
			}
		}
	}
}
