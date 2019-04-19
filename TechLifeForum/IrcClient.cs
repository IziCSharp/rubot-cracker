using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace TechLifeForum
{
	public class IrcClient : IDisposable
	{
		public delegate void ConnectedEventDelegate();

		private string _server = "";

		private int _port = 6667;

		private string _ServerPass = "";

		private string _nick = "Test";

		private string _altNick = "";

		private bool _consoleOutput = true;

		public TcpClient irc;

		public NetworkStream stream;

		private string inputLine;

		public StreamReader reader;

		public StreamWriter writer;

		private AsyncOperation op;

		public string Server => _server;

		public int Port => _port;

		public string ServerPass
		{
			get
			{
				return _ServerPass;
			}
			set
			{
				_ServerPass = value;
			}
		}

		public string Nick
		{
			get
			{
				return _nick;
			}
			set
			{
				_nick = value;
			}
		}

		public string AltNick
		{
			get
			{
				return _altNick;
			}
			set
			{
				_altNick = value;
			}
		}

		public bool ConsoleOutput
		{
			get
			{
				return _consoleOutput;
			}
			set
			{
				_consoleOutput = value;
			}
		}

		public bool Connected
		{
			get
			{
				if (irc != null && irc.Connected)
				{
					return true;
				}
				return false;
			}
		}

		public event EventHandler<UpdateUsersEventArgs> UpdateUsers = delegate
		{
		};

		public event EventHandler<UserJoinedEventArgs> UserJoined = delegate
		{
		};

		public event EventHandler<UserLeftEventArgs> UserLeft = delegate
		{
		};

		public event EventHandler<UserNickChangedEventArgs> UserNickChange = delegate
		{
		};

		public event EventHandler<ChannelMessageEventArgs> ChannelMessage = delegate
		{
		};

		public event EventHandler<NoticeMessageEventArgs> NoticeMessage = delegate
		{
		};

		public event EventHandler<PrivateMessageEventArgs> PrivateMessage = delegate
		{
		};

		public event EventHandler<StringEventArgs> ServerMessage = delegate
		{
		};

		public event EventHandler<StringEventArgs> NickTaken = delegate
		{
		};

		public event EventHandler OnConnect = delegate
		{
		};

		public event EventHandler<ExceptionEventArgs> ExceptionThrown = delegate
		{
		};

		public event EventHandler<ModeSetEventArgs> ChannelModeSet = delegate
		{
		};

		public IrcClient(string Server)
			: this(Server, 6667)
		{
		}

		public IrcClient(string Server, int Port)
		{
			op = AsyncOperationManager.CreateOperation(null);
			_server = Server;
			_port = Port;
		}

		private void Fire_UpdateUsers(UpdateUsersEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.UpdateUsers(this, (UpdateUsersEventArgs)x);
			}, o);
		}

		private void Fire_UserJoined(UserJoinedEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.UserJoined(this, (UserJoinedEventArgs)x);
			}, o);
		}

		private void Fire_UserLeft(UserLeftEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.UserLeft(this, (UserLeftEventArgs)x);
			}, o);
		}

		private void Fire_NickChanged(UserNickChangedEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.UserNickChange(this, (UserNickChangedEventArgs)x);
			}, o);
		}

		private void Fire_ChannelMessage(ChannelMessageEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.ChannelMessage(this, (ChannelMessageEventArgs)x);
			}, o);
		}

		private void Fire_NoticeMessage(NoticeMessageEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.NoticeMessage(this, (NoticeMessageEventArgs)x);
			}, o);
		}

		private void Fire_PrivateMessage(PrivateMessageEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.PrivateMessage(this, (PrivateMessageEventArgs)x);
			}, o);
		}

		private void Fire_ServerMesssage(string s)
		{
			op.Post(delegate(object x)
			{
				this.ServerMessage(this, (StringEventArgs)x);
			}, new StringEventArgs(s));
		}

		private void Fire_NickTaken(string s)
		{
			op.Post(delegate(object x)
			{
				this.NickTaken(this, (StringEventArgs)x);
			}, new StringEventArgs(s));
		}

		private void Fire_Connected()
		{
			op.Post(delegate
			{
				this.OnConnect(this, null);
			}, null);
		}

		private void Fire_ExceptionThrown(Exception ex)
		{
			op.Post(delegate(object x)
			{
				this.ExceptionThrown(this, (ExceptionEventArgs)x);
			}, new ExceptionEventArgs(ex));
		}

		private void Fire_ChannelModeSet(ModeSetEventArgs o)
		{
			op.Post(delegate(object x)
			{
				this.ChannelModeSet(this, (ModeSetEventArgs)x);
			}, o);
		}

		public void Connect()
		{
			Thread thread = new Thread(DoConnect)
			{
				IsBackground = true
			};
			thread.Start();
		}

		private void DoConnect()
		{
			try
			{
				irc = new TcpClient(_server, _port);
				stream = irc.GetStream();
				reader = new StreamReader(stream);
				writer = new StreamWriter(stream);
				if (!string.IsNullOrEmpty(_ServerPass))
				{
					Send("PASS " + _ServerPass);
				}
				Send("NICK " + _nick);
				Send("USER " + _nick + " 0 * :" + _nick);
				Listen();
			}
			catch (Exception ex)
			{
				Fire_ExceptionThrown(ex);
			}
		}

		public void Disconnect()
		{
			if (irc != null)
			{
				if (irc.Connected)
				{
					Send("QUIT Client Disconnected: http://tech.reboot.pro");
				}
				irc = null;
			}
		}

		public void JoinChannel(string channel)
		{
			if (irc != null && irc.Connected)
			{
				Send("JOIN " + channel);
			}
		}

		public void PartChannel(string channel)
		{
			Send("PART " + channel);
		}

		public void SendNotice(string toNick, string message)
		{
			Send("NOTICE " + toNick + " :" + message);
		}

		public void SendMessage(string channel, string message)
		{
			Send("PRIVMSG " + channel + " :" + message);
		}

		public void SendRaw(string message)
		{
			Send(message);
		}

		public void Dispose()
		{
			stream.Dispose();
			writer.Dispose();
			reader.Dispose();
		}

		public void Listen()
		{
			while ((inputLine = reader.ReadLine()) != null)
			{
				try
				{
					ParseData(inputLine);
					if (_consoleOutput)
					{
						Console.Write(inputLine);
					}
				}
				catch (Exception ex)
				{
					Fire_ExceptionThrown(ex);
				}
			}
		}

		private void ParseData(string data)
		{
			string[] array = data.Split(' ');
			string text = array[1];
			if (data.Length > 4 && data.Substring(0, 4) == "PING")
			{
				Send("PONG " + array[1]);
				return;
			}
			switch (text)
			{
			case "001":
				Send("MODE " + _nick + " +B");
				Fire_Connected();
				break;
			case "353":
			{
				string channel3 = array[4];
				string[] userList = JoinArray(array, 5).Split(new char[1]
				{
					' '
				}, StringSplitOptions.RemoveEmptyEntries);
				Fire_UpdateUsers(new UpdateUsersEventArgs(channel3, userList));
				break;
			}
			case "433":
			{
				string text5 = array[3];
				Fire_NickTaken(text5);
				if (text5 == _altNick)
				{
					Random random = new Random();
					string text6 = "Guest" + random.Next(0, 9) + random.Next(0, 9) + random.Next(0, 9);
					Send("NICK " + text6);
					Send("USER " + text6 + " 0 * :" + text6);
					_nick = text6;
				}
				else
				{
					Send("NICK " + _altNick);
					Send("USER " + _altNick + " 0 * :" + _altNick);
					_nick = _altNick;
				}
				break;
			}
			case "JOIN":
			{
				string channel2 = array[2];
				string user2 = array[0].Substring(1, array[0].IndexOf("!", StringComparison.Ordinal) - 1);
				Fire_UserJoined(new UserJoinedEventArgs(channel2, user2));
				break;
			}
			case "MODE":
			{
				string text2 = array[2];
				if (text2 != Nick)
				{
					string from = (!array[0].Contains("!")) ? array[0].Substring(1) : array[0].Substring(1, array[0].IndexOf("!", StringComparison.Ordinal) - 1);
					string to = array[4];
					string mode = array[3];
					Fire_ChannelModeSet(new ModeSetEventArgs(text2, from, to, mode));
				}
				break;
			}
			case "NICK":
			{
				string oldNick = array[0].Substring(1, array[0].IndexOf("!", StringComparison.Ordinal) - 1);
				string newNick = JoinArray(array, 3);
				Fire_NickChanged(new UserNickChangedEventArgs(oldNick, newNick));
				break;
			}
			case "NOTICE":
			{
				string text4 = array[0];
				string message2 = JoinArray(array, 3);
				if (text4.Contains("!"))
				{
					text4 = text4.Substring(1, array[0].IndexOf('!') - 1);
					Fire_NoticeMessage(new NoticeMessageEventArgs(text4, message2));
				}
				else
				{
					Fire_NoticeMessage(new NoticeMessageEventArgs(_server, message2));
				}
				break;
			}
			case "PRIVMSG":
			{
				string from2 = array[0].Substring(1, array[0].IndexOf('!') - 1);
				string text3 = array[2];
				string message = JoinArray(array, 3);
				if (string.Equals(text3, _nick, StringComparison.CurrentCultureIgnoreCase))
				{
					Fire_PrivateMessage(new PrivateMessageEventArgs(from2, message));
				}
				else
				{
					Fire_ChannelMessage(new ChannelMessageEventArgs(text3, from2, message));
				}
				break;
			}
			case "PART":
			case "QUIT":
			{
				string channel = array[2];
				string user = array[0].Substring(1, data.IndexOf("!") - 1);
				Fire_UserLeft(new UserLeftEventArgs(channel, user));
				Send("NAMES " + array[2]);
				break;
			}
			default:
				if (array.Length > 3)
				{
					Fire_ServerMesssage(JoinArray(array, 3));
				}
				break;
			}
		}

		private static string StripMessage(string message)
		{
			foreach (Match item in new Regex("\u0003(?:\\d{1,2}(?:,\\d{1,2})?)?").Matches(message))
			{
				message = message.Replace(item.Value, "");
			}
			if (message == "")
			{
				return "";
			}
			if (message.Substring(0, 1) == ":" && message.Length > 2)
			{
				return message.Substring(1, message.Length - 1);
			}
			return message;
		}

		private static string JoinArray(string[] strArray, int startIndex)
		{
			return StripMessage(string.Join(" ", strArray, startIndex, strArray.Length - startIndex));
		}

		public void Send(string message)
		{
			writer.WriteLine(message);
			writer.Flush();
		}
	}
}
