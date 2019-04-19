using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.Threading;

namespace rubotNet.chYoutube
{
	public class ChatYoutube
	{
		private static IWebDriver chatBotYoutube;

		public static void ChatBot(string channelName, string acc, string word)
		{
			string text = acc.ToString().Split(':')[0].ToString();
			string text2 = acc.ToString().Split(':')[1].ToString();
			try
			{
				ChromeOptions chromeOptions = new ChromeOptions();
				chromeOptions.AddArguments("--disable-gpu");
				chromeOptions.AddArguments("--window-size=500,500");
				chromeOptions.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36");
				ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
				chromeDriverService.HideCommandPromptWindow = true;
				chatBotYoutube = new ChromeDriver(chromeDriverService, chromeOptions);
				chatBotYoutube.Navigate().GoToUrl("https://accounts.google.com/ServiceLogin?continue=http://youtube.com/live_chat?v=" + channelName + "&is_popout=1");
				chatBotYoutube.FindElement(By.Id("identifierId")).SendKeys(text);
				Thread.Sleep(1500);
				chatBotYoutube.FindElement(By.Id("identifierNext")).Click();
				Thread.Sleep(2000);
				chatBotYoutube.FindElement(By.Name("password")).SendKeys(text2);
				Thread.Sleep(1500);
				chatBotYoutube.FindElement(By.Id("passwordNext")).Click();
				Thread.Sleep(3000);
				IWebElement webElement = chatBotYoutube.FindElement(By.Id("input"));
				webElement.SendKeys(word);
				webElement.SendKeys(Keys.Enter);
				Thread.Sleep(3000);
				chatBotYoutube.Quit();
			}
			catch
			{
			}
		}

		public static void stopChatYoutube()
		{
			chatBotYoutube.Quit();
			Process[] processesByName = Process.GetProcessesByName("chromedriver");
			foreach (Process process in processesByName)
			{
				process.Kill();
			}
		}
	}
}
