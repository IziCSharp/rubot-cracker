using Extreme.Net;
using Microsoft.VisualBasic.CompilerServices;
using rubotNet.Logs;
using System;
using System.IO;
using System.Net.Sockets;
using TechLifeForum;

namespace chatTwitch
{
	public class ChatTwitch
	{
		private static IrcClient client;

		public static string channelName;

		public static void ChatBotTwitchWrite(string channelname, string account, string word)
		{
			string text = account.ToString().Split(':')[0].ToString();
			string text2 = "oauth:" + account.ToString().Split(':')[2].ToString();
			Log.richLog.Text = text + "writing: " + word;
			TcpClient tcpClient = new TcpClient();
			tcpClient.Connect("irc.chat.twitch.tv", 6667);
			TextReader textReader = new StreamReader(tcpClient.GetStream());
			TextWriter textWriter = new StreamWriter(tcpClient.GetStream());
			textWriter.Write("PASS " + text2 + "\r\nUSER " + text + " 0 * :" + text + "\r\nNICK " + text + "\r\n");
			textWriter.Flush();
			string text3 = textReader.ReadLine();
			if (!string.IsNullOrEmpty(text3))
			{
				textWriter.WriteLine("PRIVMSG #" + channelname + " : " + word);
				textWriter.Flush();
			}
			if (text3 == null || Operators.CompareString(text3.Split(' ')[1], "001", TextCompare: false) == 0)
			{
				textWriter.Write("MODE " + text + " +B\r\nJOIN #" + channelname + "\r\n");
				textWriter.Flush();
			}
		}

		public static void ChatBotTwitchConnect(string channelname, string account)
		{
			channelName = channelname;
			string text = account.ToString().Split(':')[0].ToString();
			string serverPass = "oauth:" + account.ToString().Split(':')[2].ToString();
			client = new IrcClient("irc.chat.twitch.tv", Conversions.ToInteger("6667"));
			client.OnConnect += client_OnConnect;
			client.Nick = text;
			client.ServerPass = serverPass;
			client.Connect();
			Log.richLog.Text = text + " connected in " + channelName;
		}

		private static void client_OnConnect(object sender, EventArgs e)
		{
			client.JoinChannel("#" + channelName);
		}

		public static void TwitchBotFollow(string idChannel, string account)
		{
			HttpRequest httpRequest = new HttpRequest
			{
				KeepAlive = true,
				UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0"
			};
			httpRequest.Cookies = new CookieDictionary();
			httpRequest.AddHeader(HttpHeader.Accept, "*/*");
			httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU");
			httpRequest.AddHeader("client-id", "kimne78kx3ncx6brgo4mv6wki5h1ko");
			httpRequest.AddHeader("x-device-id", "njVBh19GZVf3F21vLtm3ieBBsz2VBEl9");
			httpRequest.AddHeader("authorization", "OAuth " + account);
			httpRequest.AddHeader("DNT", "1");
			RequestParams requestParams = new RequestParams();
			string str = "[{\"query\":\"mutation FollowButton_FollowUser($input: FollowUserInput!) { followUser(input: $input) { follow { disableNotifications __typename } __typename }}\",\"variables\":{\"input\":{\"disableNotifications\":false,\"targetID\":\"" + idChannel + "\"}},\"operationName\":\"FollowButton_FollowUser\"},{\"query\":\"query FollowButton_FollowEvent_User($id: ID!) { user(id: $id) { id isPartner stream { id game { id name __typename } __typename } hosting { id stream { id game { id name __typename } __typename } __typename } __typename }}\",\"variables\":{\"id\":\"" + idChannel + "\"},\"operationName\":\"FollowButton_FollowEvent_User\"}]";
			httpRequest.Post("https://gql.twitch.tv/gql", str, "application/json");
		}
	}
}
