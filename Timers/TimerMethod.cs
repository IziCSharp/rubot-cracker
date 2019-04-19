using chatTwitch;
using Extreme.Net;
using Microsoft.VisualBasic.CompilerServices;
using Other;
using rb_youtube;
using rubotNet.chYoutube;
using rubotNet.Lcheck;
using rubotNet.Logs;
using rubotNet.Waa;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Timers
{
	public class TimerMethod
	{
		public static string loginTimer;

		public static int TP;

		public static string monitorPotokov;

		public static string apiAWM;

		public static string getSetThreads;

		public static string wordChat;

		public static string accChat;

		public static string accFollowTwitch;

		public static string channelYoutube;

		public static string channelTwitch;

		public static string idChannelTwitch;

		public static string countViewers;

		public static string statusStream;

		public int count;

		public static int zero = 0;

		public static int zeroFollowTwitch = 0;

		public static int setNumberAccsTwitch;

		public static int close;

		public static Button buttonStart;

		public static NumericUpDown countThreads;

		public static ComboBox typeProxies;

		public static CheckBox modeViewers;

		public static int countNow;

		public static bool working;

		public static Random wordsChat = new Random();

		public static Random accsChat = new Random();

		private static Timer AutoUpdateProxyUrl = new Timer();

		private static Timer OnOff = new Timer();

		private static Timer ChatBotTwitchWrite = new Timer();

		private static Timer ChatBotTwitchConnect = new Timer();

		private static Timer chatBotYoutube = new Timer();

		private static Timer YoutubeLike = new Timer();

		private static Timer timerClose = new Timer();

		private static Timer twitchFollow = new Timer();

		private static Timer AutoUpdate300 = new Timer();

		private static Timer AutoUpdateAWM = new Timer();

		private static Timer LifeLicence = new Timer();

		public static void AutoUpdateProxyUrl_Tick(object sender, EventArgs e)
		{
			try
			{
				HttpRequest httpRequest = new HttpRequest();
				for (int i = 0; i < Otherparams.proxyURL.Length; i = checked(i + 1))
				{
					string text = httpRequest.Get(Otherparams.proxyURL[i]).ToString();
					Otherparams.smartProxyList = text.Split(new string[1]
					{
						"\n"
					}, StringSplitOptions.None);
				}
				Log.richLog.Text = "Uploaded proxies from URL: " + Otherparams.smartProxyList.Length;
			}
			catch
			{
				Log.richLog.Text = "Произошла ошибка при обновлении проксей из URL по таймеру";
			}
		}

		public static void AuPu_Start(string login, int timerUp)
		{
			loginTimer = login;
			AutoUpdateProxyUrl.Interval = timerUp * 60000;
			AutoUpdateProxyUrl.Tick += AutoUpdateProxyUrl_Tick;
			AutoUpdateProxyUrl.Start();
		}

		public static void AuPu_Stop()
		{
			AutoUpdateProxyUrl.Stop();
		}



		public static void AutoUpdate300_Start(string login, int typeProxies, int timerUp)
		{
			
		}

		public static void AutoUpdate300_Stop()
		{
			AutoUpdate300.Stop();
		}

		public static void AutoUpdateAWM_Tick(object sender, EventArgs e)
		{
			try
			{
				HttpRequest httpRequest = new HttpRequest();
				string text = httpRequest.Get("https://awmproxy.com/proxy/" + apiAWM).ToString();
				Otherparams.smartProxyList = text.Split(new string[1]
				{
					"\n"
				}, StringSplitOptions.None);
				Log.richLog.Text = "Uploaded proxies AWM: " + Otherparams.smartProxyList.Length;
			}
			catch
			{
				Log.richLog.Text = "Ошибка запроса к API AwmProxy.";
			}
		}

		public static void AutoUpdateAWM_Start(string apikeyAWM)
		{
			apiAWM = apikeyAWM;
			AutoUpdateAWM.Interval = 600000;
			AutoUpdateAWM.Tick += AutoUpdateAWM_Tick;
			AutoUpdateAWM.Start();
		}

		public static void AutoUpdateAWM_Stop()
		{
			AutoUpdateAWM.Stop();
		}

		public static void OnOff_Tick(object sender, EventArgs e)
		{
			try
			{
				HttpRequest httpRequest = new HttpRequest();
				string text = httpRequest.Get("https://api.twitch.tv/kraken/streams/" + channelTwitch + "?client_id=jzkbprff40iqj646a697cyrvl0zt2m6").ToString();
				if (text.ToString().Contains("{\"stream\":{"))
				{
					string text2 = countViewers = text.Substring("viewers\":", ",");
					statusStream = "ONLINE";
				}
				if (text.ToString().Contains("{\"stream\":null"))
				{
					Process.GetCurrentProcess().Kill();
				}
			}
			catch
			{
			}
		}

		public static void OnOff_Start(string channelname)
		{
			channelTwitch = channelname;
			OnOff.Interval = 60000;
			OnOff.Tick += OnOff_Tick;
			OnOff.Start();
		}

		public static void OnOff_Stop()
		{
			OnOff.Stop();
		}

		public static void ChatBotTwitchWrite_Tick(object sender, EventArgs e)
		{
			wordsChat = new Random();
			accsChat = new Random();
			accChat = Conversions.ToString(WordsAndAccs.accountsChatList[accsChat.Next(WordsAndAccs.accountsChatList.Count)]);
			wordChat = Conversions.ToString(WordsAndAccs.wordsList[wordsChat.Next(WordsAndAccs.wordsList.Count)]);
			ChatTwitch.ChatBotTwitchWrite(channelTwitch, accChat, wordChat);
		}

		public static void ChatBotTwitchWrite_Start(int intervalWriteChatTwitch, string channelname)
		{
			channelTwitch = channelname;
			ChatBotTwitchWrite.Interval = intervalWriteChatTwitch * 1000;
			ChatBotTwitchWrite.Tick += ChatBotTwitchWrite_Tick;
			ChatBotTwitchWrite.Start();
		}

		public static void ChatBotTwitchWrite_Stop()
		{
			ChatBotTwitchWrite.Stop();
		}

		public static void ChatBotTwitchConnect_Tick(object sender, EventArgs e)
		{
			accChat = Conversions.ToString(WordsAndAccs.accountsChatList[zero++]);
			ChatTwitch.ChatBotTwitchConnect(channelTwitch, accChat);
			if (zero == setNumberAccsTwitch)
			{
				ChatBotTwitchConnect.Stop();
			}
		}

		public static void ChatBotTwitchConnect_Start(string channelname, int countAccs)
		{
			channelTwitch = channelname;
			setNumberAccsTwitch = countAccs - 1;
			ChatBotTwitchConnect.Interval = 1000;
			ChatBotTwitchConnect.Tick += ChatBotTwitchConnect_Tick;
			ChatBotTwitchConnect.Start();
		}

		public static void ChatBotTwitchConnect_Stop()
		{
			ChatBotTwitchConnect.Stop();
		}

		public static void TwitchFollow_Tick(object sender, EventArgs e)
		{
			accFollowTwitch = Conversions.ToString(Otherparams.accsListSubTwitch[zeroFollowTwitch++]);
			ChatTwitch.TwitchBotFollow(idChannelTwitch, accFollowTwitch);
			if (zeroFollowTwitch == Otherparams.accsListSubTwitch.Length)
			{
				ChatBotTwitchConnect.Stop();
			}
		}

		public static void TwitchFollow_Start(string channelname)
		{
			HttpRequest httpRequest = new HttpRequest();
			string str = httpRequest.Get("https://api.twitch.tv/kraken/channels/" + channelname + "?client_id=kimne78kx3ncx6brgo4mv6wki5h1ko").ToString();
			idChannelTwitch = str.Substring("_id\":", ",\"");
			twitchFollow.Interval = 10000;
			twitchFollow.Tick += TwitchFollow_Tick;
			twitchFollow.Start();
		}

		public static void TwitchFollow_Stop()
		{
			twitchFollow.Stop();
		}

		public static void ChatBotYoutube_Tick(object sender, EventArgs e)
		{
			accChat = Conversions.ToString(WordsAndAccs.accountsChatList[accsChat.Next(WordsAndAccs.accountsChatList.Count)]);
			wordChat = Conversions.ToString(WordsAndAccs.wordsList[wordsChat.Next(WordsAndAccs.wordsList.Count)]);
			ChatYoutube.ChatBot(channelYoutube, accChat, wordChat);
		}

		public static void ChatBotYoutube_Start(int intervalWriteChatYouTube, string channelname)
		{
			channelYoutube = channelname;
			chatBotYoutube.Interval = intervalWriteChatYouTube * 1000;
			chatBotYoutube.Tick += ChatBotYoutube_Tick;
			chatBotYoutube.Start();
		}

		public static void ChatBotYoutube_Stop()
		{
			chatBotYoutube.Stop();
		}

		public static void YoutubeLike_Tick(object sender, EventArgs e)
		{
			try
			{
				accChat = Conversions.ToString(WordsAndAccs.accountsChatList[zero++]);
				YoutubeBot.LikeBot(channelYoutube, accChat);
			}
			catch
			{
				YoutubeLike.Stop();
			}
		}

		public static void YoutubeLike_Start(string channelname)
		{
			channelYoutube = channelname;
			YoutubeLike.Interval = 5000;
			YoutubeLike.Tick += YoutubeLike_Tick;
			YoutubeLike.Start();
		}

		public static void YoutubeLike_Stop()
		{
			YoutubeLike.Stop();
		}

		public static void LifeLicence_Tick(object sender, EventArgs e)
		{
			
		}

		public static void LifeLicence_Start(string login)
		{
			
		}

		public static void LifeLicence_Stop()
		{
			
		}
	}
}
