using Extreme.Net;
using rubotNet.Chatlist;
using rubotNet.Logs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace rubotNet.Waa
{
	public class WordsAndAccs
	{
		public static List<string> wordsList = new List<string>();

		public static List<string> accountsChatList = new List<string>();

		public static void GetRuWords1(string login)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.CharacterSet = Encoding.GetEncoding("UTF-8");
			RequestParams requestParams = new RequestParams();
			requestParams["key"] = login;
			string text = httpRequest.Post("https://rubot.ovh/cloud/ruwords1.php", requestParams).ToString();
			wordsList.AddRange(text.Split(new string[1]
			{
				"\n"
			}, StringSplitOptions.None));
			Log.richLog.Text = "Loaded 1st list RU words: " + wordsList.Count;
		}

		public static void GetRuWords2(string login)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.CharacterSet = Encoding.GetEncoding("UTF-8");
			RequestParams requestParams = new RequestParams();
			requestParams["key"] = login;
			string text = httpRequest.Post("https://rubot.ovh/cloud/ruwords2.php", requestParams).ToString();
			wordsList.AddRange(text.Split(new string[1]
			{
				"\n"
			}, StringSplitOptions.None));
			Log.richLog.Text = "Loaded 2nd list RU words: " + wordsList.Count;
		}

		public static void GetRuWords3(string login)
		{
			HttpRequest httpRequest = new HttpRequest();
			httpRequest.CharacterSet = Encoding.GetEncoding("UTF-8");
			RequestParams requestParams = new RequestParams();
			requestParams["key"] = login;
			string text = httpRequest.Post("https://rubot.ovh/cloud/ruwords3.php", requestParams).ToString();
			wordsList.AddRange(text.Split(new string[1]
			{
				"\n"
			}, StringSplitOptions.None));
			Log.richLog.Text = "Loaded 3th list RU words: " + wordsList.Count;
		}

		public static void GetMyWords()
		{
			try
			{
				OpenFileDialog openFileDialog = new OpenFileDialog
				{
					Filter = "Текстовые файлы|*.txt"
				};
				openFileDialog.ShowDialog();
				if (File.Exists(openFileDialog.FileName))
				{
					wordsList.AddRange(File.ReadAllLines(openFileDialog.FileName));
				}
				Log.richLog.Text = "Loaded list words from txt-file: " + wordsList.Count;
			}
			catch
			{
				Log.richLog.Text = "ERROR UPLOAD WORDS FROM FILE!";
			}
		}

		public static void GetFreeAccsTwitch(string login)
		{
			HttpRequest httpRequest = new HttpRequest();
			RequestParams requestParams = new RequestParams();
			requestParams["key"] = login;
			string text = httpRequest.Post("https://rubot.ovh/cloud/acctwitchload.php", requestParams).ToString();
			accountsChatList.AddRange(text.Split(new string[1]
			{
				"\n"
			}, StringSplitOptions.None));
			foreach (string accountsChat in accountsChatList)
			{
				string[] array = Regex.Split(accountsChat, ":oauth:");
				ChatList.chattersListBox.Items.Add(array[0]);
			}
			Log.richLog.Text = "Free chat-account for Twitch: " + accountsChatList.Count;
		}

		public static void GetMyAccs(int typeAccs)
		{
			if (typeAccs == 0)
			{
				try
				{
					OpenFileDialog openFileDialog = new OpenFileDialog
					{
						Filter = "Текстовые файлы|*.txt"
					};
					openFileDialog.ShowDialog();
					if (File.Exists(openFileDialog.FileName))
					{
						accountsChatList.AddRange(File.ReadAllLines(openFileDialog.FileName));
						foreach (string accountsChat in accountsChatList)
						{
							string[] array = Regex.Split(accountsChat, ":oauth:");
							ChatList.chattersListBox.Items.Add(array[0]);
						}
					}
					Log.richLog.Text = "Chat-account for Twitch: " + accountsChatList.Count;
				}
				catch
				{
					Log.richLog.Text = "ERROR UPLOAD CHAT-BOTS FROM FILE!";
				}
			}
			if (typeAccs == 1)
			{
				try
				{
					OpenFileDialog openFileDialog2 = new OpenFileDialog
					{
						Filter = "Текстовые файлы|*.txt"
					};
					openFileDialog2.ShowDialog();
					if (File.Exists(openFileDialog2.FileName))
					{
						accountsChatList.AddRange(File.ReadAllLines(openFileDialog2.FileName));
						foreach (string accountsChat2 in accountsChatList)
						{
							string[] array2 = Regex.Split(accountsChat2, ":oauth:");
							ChatList.chattersListBox.Items.Add(array2[0]);
						}
					}
					Log.richLog.Text = "Chat-account for Youtube: " + accountsChatList.Count;
				}
				catch
				{
					Log.richLog.Text = "ERROR UPLOAD CHAT-BOTS FROM FILE!";
				}
			}
		}
	}
}
