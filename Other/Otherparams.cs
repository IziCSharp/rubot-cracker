using Extreme.Net;
using IniFileClass;
using Microsoft.VisualBasic;
using rubotNet.Logs;
using rubotNet.UrlsProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Other
{
	public class Otherparams
	{
		public static string[] smartProxyList;

		public static List<string> UAlist = new List<string>();

		public static string[] proxyURL;

		public static string[] accsListSubTwitch;

		private static IniFile MyIni = new IniFile("Settings.ini");

		public static void GetMyAccsSubTwitch()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Текстовые файлы|*.txt"
			};
			openFileDialog.ShowDialog();
			if (File.Exists(openFileDialog.FileName))
			{
				accsListSubTwitch = File.ReadAllLines(openFileDialog.FileName);
			}
		}

		public static void GetServiceProxy(string login, int typeProxy)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.UserAgent = "rubotUpdater";
			string text = httpRequest.Get("https://rubot.ovh/applications/nexus/interface/licenses/?info&key=" + login).ToString();
			if (text.Contains("item_id\":4,\"active\":1"))
			{
				if (typeProxy == 0)
				{
					RequestParams requestParams = new RequestParams();
					requestParams["key"] = login;
					string text2 = httpRequest.Post("https://rubot.ovh/cloud/proxyloadhttp.php", requestParams).ToString();
					smartProxyList = text2.Split(new string[1]
					{
						"\n"
					}, StringSplitOptions.None);
					Log.richLog.Text = "Uploaded proxies HTTP (Rubot): " + smartProxyList.Length;
				}
				if (typeProxy == 1)
				{
					RequestParams requestParams2 = new RequestParams();
					requestParams2["key"] = login;
					string text3 = httpRequest.Post("https://rubot.ovh/cloud/proxyloadsocks4.php", requestParams2).ToString();
					smartProxyList = text3.Split(new string[1]
					{
						"\n"
					}, StringSplitOptions.None);
					Log.richLog.Text = "Uploaded proxies SOCKS4 (Rubot): " + smartProxyList.Length;
				}
				if (typeProxy == 2)
				{
					RequestParams requestParams3 = new RequestParams();
					requestParams3["key"] = login;
					string text4 = httpRequest.Post("https://rubot.ovh/cloud/proxyloadsocks5.php", requestParams3).ToString();
					smartProxyList = text4.Split(new string[1]
					{
						"\n"
					}, StringSplitOptions.None);
					Log.richLog.Text = "Uploaded proxies SOCKS5 (Rubot): " + smartProxyList.Length;
				}
			}
			else
			{
				Interaction.MsgBox("Not access!", MsgBoxStyle.Information);
				Log.richLog.Text = "Ошибка загрузки проксей. Не доступен сервер или вы не имеете активной подписки.";
			}
		}

		public static void GetProxyFile(string login)
		{
            try { 
					OpenFileDialog openFileDialog = new OpenFileDialog
					{
						Filter = "Текстовые файлы|*.txt"
					};
					openFileDialog.ShowDialog();
					if (File.Exists(openFileDialog.FileName))
					{
						smartProxyList = File.ReadAllLines(openFileDialog.FileName);
					}
					Log.richLog.Text = "Uploaded proxies: " + smartProxyList.Length;
				}
				catch
				{
					Log.richLog.Text = "ERROR UPLOAD PROXIES FROM FILE!";
				}
			}
		
		

		public static void GetProxyUrls(string login)
		{
			
				UrlListProxy.richTextBox1.SaveFile("url_list.txt", RichTextBoxStreamType.PlainText);
				proxyURL = File.ReadAllLines("url_list.txt");
				HttpRequest httpRequest2 = new HttpRequest();
				for (int i = 0; i < proxyURL.Length; i = checked(i + 1))
				{
					string text2 = httpRequest2.Get(proxyURL[i]).ToString();
					smartProxyList = text2.Split(new string[1]
					{
						"\n"
					}, StringSplitOptions.None);
				}
				Log.richLog.Text = "Uploaded proxies from URL's: " + smartProxyList.Length;
		
		}

		public static void GetProxyFullAWM(string apiKeyAWM)
		{
			HttpRequest httpRequest = new HttpRequest();
			string text = httpRequest.Get("https://awmproxy.com/proxy/" + apiKeyAWM).ToString();
			if (text.Contains("Key is not found"))
			{
				Log.richLog.Text = "Key is not found!";
			}
			if (text.Contains("ERROR! Tariff expired!"))
			{
				Log.richLog.Text = "ERROR! Tariff expired!";
			}
		}

		public static void GetUA()
		{
			HttpRequest httpRequest = new HttpRequest();
			string text = httpRequest.Get("https://rubot.ovh/update/useragents.txt").ToString();
			UAlist.Clear();
			UAlist.AddRange(text.Split(new string[1]
			{
				"\n"
			}, StringSplitOptions.None));
			Log.richLog.Text = "Uploaded User-Agents: " + UAlist.Count;
		}
	}
}
