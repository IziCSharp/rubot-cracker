using Extreme.Net;
using Other;

using rubotNet.Logs;
using System;
using System.Linq;
using System.Threading;
using Timers;

namespace rb_twitch
{
	public class TwitchBot
	{
		public static int Generation;

		public static int countWorkThreads;

		public Random rand;

		public string channelName;

		public static string channelCon;

		public static string video;

		public static string apiTwitchPost;

		public int gen;

		public int typeProxy;

		public int methodTwitch;

		public bool oldTwitch;

		public int timeTwitch;

		public static string tokenNew;

		public static string sigNew;

		public static string clientID;

		public static string apiParam1;

		public static string apiParam2;

		public static string usher1;

		public static string usher2;

		public static string ushersig;

		public static string ushertoken;

		public static string videoCont;

		public bool Working()
		{
			return gen == Generation;
		}

		public TwitchBot(string channelName, int gen, int typeProxy, int methodTwitch, bool oldTwitch, int timeTwitch)
		{
			this.gen = gen;
			this.channelName = channelName;
			this.typeProxy = typeProxy;
			this.methodTwitch = methodTwitch;
			this.oldTwitch = oldTwitch;
			this.timeTwitch = timeTwitch;
			Thread thread = new Thread(LoopChromeTwitch);
			thread.IsBackground = true;
			thread.Start();
		}

		public static void StartAll(string login, string channelName, int count, int interval, int typeProxy, int methodTwitch, bool oldTwitch, int timeTwitch, string UrlProxy)
		{
			StopAll();
			channelName = channelName.Trim();
			int gen = Generation;
			Log.richLog.Text = "Channel: " + channelName + ". Count threads: " + count + ". Method: " + methodTwitch + ". Proxies: " + Otherparams.smartProxyList.Length;
            clientID = "jzkbprff40iqj646a697cyrvl0zt2m6";
            apiParam1 = "https://api.twitch.tv/api/channels/";
            apiParam2 = "/access_token?need_https=true&oauth_token&platform=web&player_backend=mediaplayer&player_type=site";
            usher1 = "https://usher.ttvnw.net/api/channel/hls/";
            usher2 = ".m3u8?allow_source=true&baking_bread=true&baking_brownies=true&baking_brownies_timeout=1050&fast_bread=true&p=3168255&player_backend=mediaplayer&playlist_include_framerate=true&reassignments_supported=false&rtqos=business_logic_reverse&cdm=wv";
            ushersig = "&sig=";
            ushertoken = "&token=";
            videoCont = "video-";
            Thread thread = new Thread((ThreadStart)delegate
			{
				int num = Enumerable.Range(0, count).Select((Func<int, TwitchBot>)delegate
				{
					Thread.Sleep(interval);
					return new TwitchBot(channelName, gen, typeProxy, methodTwitch, oldTwitch, timeTwitch);
				}).Count();
			});
			thread.IsBackground = true;
			thread.Start();
		}

		public static void StopAll()
		{
			Generation++;
		}

		public void LoopChromeTwitch()
		{
			rand = new Random();
			countWorkThreads++;
			while (Working())
			{
				try
				{
					HttpRequest httpRequest = new HttpRequest();
					if (methodTwitch == 1)
					{
						if (typeProxy == 0)
						{
							httpRequest.Proxy = HttpProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
						}
						if (typeProxy == 1)
						{
							httpRequest.Proxy = Socks4ProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
						}
						if (typeProxy == 2)
						{
							httpRequest.Proxy = Socks5ProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
						}
					}
					httpRequest.AddHeader(HttpHeader.Accept, "*/*");
					httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
					httpRequest.AddHeader("Client-ID", clientID);
					httpRequest.AddHeader(HttpHeader.Referer, "https://www.twitch.tv/" + channelName);
					httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
					string text = httpRequest.Get(apiParam1 + channelName + apiParam2).ToString();
					if (text.Contains("token"))
					{
						string text2 = text.Substring("token\":\"", "\",\"sig").Replace("\\", "").Replace("u0026", "\\u0026");
						string text3 = text.Substring("sig\":\"", "\",\"mobile");
						httpRequest.AddHeader(HttpHeader.Accept, "application/x-mpegURL, application/vnd.apple.mpegurl, application/json, text/plain");
						httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
						string str = httpRequest.Get(usher1 + channelName + usher2 + ushersig + text3 + ushertoken + text2).ToString();
						string text4 = "https://" + str.Substring("https://", ".m3u8") + ".m3u8";
						if (text4.Contains(videoCont))
						{
							while (Working())
							{
								if (methodTwitch == 0)
								{
									if (typeProxy == 0)
									{
										httpRequest.Proxy = HttpProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
									}
									if (typeProxy == 1)
									{
										httpRequest.Proxy = Socks4ProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
									}
									if (typeProxy == 2)
									{
										httpRequest.Proxy = Socks5ProxyClient.Parse(Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
									}
								}
								httpRequest.AddHeader(HttpHeader.Accept, "application/x-mpegURL, application/vnd.apple.mpegurl, application/json, text/plain");
								httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
								if (!oldTwitch)
								{
									string text5 = httpRequest.Raw(HttpMethod.HEAD, text4).ToString();
								}
								if (oldTwitch)
								{
									string text6 = httpRequest.Get(text4).ToString();
								}
								httpRequest.Close();
								Thread.Sleep(timeTwitch);
							}
						}
					}
				}
				catch
				{
					continue;
				}
				break;
			}
			countWorkThreads--;
		}
	}
}
