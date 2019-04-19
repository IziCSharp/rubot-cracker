using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Other;
using System;
using System.Linq;
using System.Threading;

namespace rubotNet.Views
{
	public class ViewsBot
	{
		public static int Generation;

		public static int countWorkThreads;

		public int gen;

		public string channelName;

		public int typeProxy;

		public int threads;

		public Random rand;

		public Random rand1;

		private static IWebDriver viewTwitch;

		public bool Working()
		{
			return gen == Generation;
		}

		public ViewsBot(string channelName, int gen, int typeProxy, int threads)
		{
			this.gen = gen;
			this.channelName = channelName;
			this.typeProxy = typeProxy;
			this.threads = threads;
			Thread thread = new Thread(TwitchViewBot);
			thread.IsBackground = true;
			thread.Start();
		}

		public static void startViews(string channelName, int typeProxy, int threads)
		{
			StopAll();
			int gen = Generation;
			Thread thread = new Thread((ThreadStart)delegate
			{
				int num = Enumerable.Range(0, 1).Select((Func<int, ViewsBot>)delegate
				{
					Thread.Sleep(2000);
					return new ViewsBot(channelName, gen, typeProxy, threads);
				}).Count();
			});
			thread.IsBackground = true;
			thread.Start();
		}

		public void TwitchViewBot()
		{
			rand = new Random();
			rand1 = new Random();
			countWorkThreads++;
			while (Working())
			{
				try
				{
					ChromeOptions chromeOptions = new ChromeOptions();
					chromeOptions.AddArguments("--disable-gpu");
					ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
					chromeDriverService.HideCommandPromptWindow = true;
					if (typeProxy == 0)
					{
						chromeOptions.AddArguments("--proxy-server=http://" + Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
					}
					if (typeProxy == 1)
					{
						chromeOptions.AddArguments("--proxy-server=socks4://" + Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
					}
					if (typeProxy == 2)
					{
						chromeOptions.AddArguments("--proxy-server=socks5://" + Otherparams.smartProxyList[rand.Next(Otherparams.smartProxyList.Length)]);
					}
					chromeOptions.AddArgument("ignore-certificate-errors");
					chromeOptions.AddArgument("--incognito");
					chromeOptions.AddArgument("user-agent=" + Otherparams.UAlist[rand1.Next(Otherparams.UAlist.Count)]);
					viewTwitch = new ChromeDriver(chromeDriverService, chromeOptions);
					viewTwitch.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20.0);
				
					viewTwitch.Navigate().GoToUrl(channelName);
					Thread.Sleep(20000);
					viewTwitch.Quit();
					Thread.Sleep(5000);
				}
				catch
				{
					viewTwitch.Quit();
				}
			}
			countWorkThreads--;
		}

		public static void StopAll()
		{
			Generation++;
		}
	}
}
