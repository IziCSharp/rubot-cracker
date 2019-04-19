using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Extreme.Net
{
	public class ChainProxyClient : ProxyClient
	{
		[ThreadStatic]
		private static Random _rand;

		private List<ProxyClient> _proxies = new List<ProxyClient>();

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

		public bool EnableShuffle
		{
			get;
			set;
		}

		public List<ProxyClient> Proxies => _proxies;

		public override string Host
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Port
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override string Username
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override string Password
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int ConnectTimeout
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int ReadWriteTimeout
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public ChainProxyClient(bool enableShuffle = false)
			: base(ProxyType.Chain)
		{
			EnableShuffle = enableShuffle;
		}

		public override TcpClient CreateConnection(string destinationHost, int destinationPort, TcpClient tcpClient = null)
		{
			if (_proxies.Count == 0)
			{
				throw new InvalidOperationException(Resources.InvalidOperationException_ChainProxyClient_NotProxies);
			}
			List<ProxyClient> list;
			if (EnableShuffle)
			{
				list = _proxies.ToList();
				for (int i = 0; i < list.Count; i++)
				{
					int index = Rand.Next(list.Count);
					ProxyClient value = list[i];
					list[i] = list[index];
					list[index] = value;
				}
			}
			else
			{
				list = _proxies;
			}
			int num = list.Count - 1;
			TcpClient tcpClient2 = tcpClient;
			for (int j = 0; j < num; j++)
			{
				tcpClient2 = list[j].CreateConnection(list[j + 1].Host, list[j + 1].Port, tcpClient2);
			}
			return list[num].CreateConnection(destinationHost, destinationPort, tcpClient2);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ProxyClient proxy in _proxies)
			{
				stringBuilder.AppendLine(proxy.ToString());
			}
			return stringBuilder.ToString();
		}

		public new virtual string ToExtendedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ProxyClient proxy in _proxies)
			{
				stringBuilder.AppendLine(proxy.ToExtendedString());
			}
			return stringBuilder.ToString();
		}

		public void AddProxy(ProxyClient proxy)
		{
			if (proxy == null)
			{
				throw new ArgumentNullException("proxy");
			}
			_proxies.Add(proxy);
		}

		public void AddHttpProxy(string proxyAddress)
		{
			_proxies.Add(HttpProxyClient.Parse(proxyAddress));
		}

		public void AddSocks4Proxy(string proxyAddress)
		{
			_proxies.Add(Socks4ProxyClient.Parse(proxyAddress));
		}

		public void AddSocks4aProxy(string proxyAddress)
		{
			_proxies.Add(Socks4aProxyClient.Parse(proxyAddress));
		}

		public void AddSocks5Proxy(string proxyAddress)
		{
			_proxies.Add(Socks5ProxyClient.Parse(proxyAddress));
		}
	}
}
