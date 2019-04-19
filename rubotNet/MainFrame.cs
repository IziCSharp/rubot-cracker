using IniFileClass;
using Microsoft.VisualBasic;
using Other;
using rb_twitch;
using rb_youtube;
using rubotNet.Chatlist;
using rubotNet.chYoutube;
using rubotNet.Logs;
using rubotNet.UrlsProxy;
using rubotNet.Views;
using rubotNet.Waa;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timers;

namespace rubotNet
{
	public class MainFrame : UserControl
	{
		private IniFile MyIni = new IniFile("Settings.ini");

		private bool working;

		public string login;

		public string autostart;

		public Random proxyRand = new Random();

		private IContainer components;

		private ComboBox typeProxy;

		private ComboBox uploadProxies;

		private ComboBox serviceType;

		private Label label2;

		private TextBox channelText;

		private GroupBox groupBox3;

		private Label label3;

		private NumericUpDown timeoutThreads;

		private Label label1;

		private NumericUpDown setNumberThreads;

		private StatusStrip statusStrip1;

		private ToolStripStatusLabel statusThreads;

		private ToolStripStatusLabel methodLabel;

		private ToolStripStatusLabel statusStream;

		private ToolStripStatusLabel countViewers;

		private System.Windows.Forms.Timer progressUpdate;

		private ToolTip toolTip1;

		private GroupBox groupBox1;

		private ComboBox comboMethods;

		private Label label4;

		private ComboBox comboBox1;

		private Label labelCountWords;

		private Label labelCountChatAccs;

		private Label label6;

		private NumericUpDown numericChatWrite;

		private Button writeChatButton;

		private Label label8;

		private NumericUpDown numericAccsTwitch;

		private Button connectChatTwitch;

		private Label accountsSubs;

		private Button uploadAccsForSubs;

		private Button followButtonTwitch;

		private TabControl tabControl1;

		private TabPage tabPage1;

		private TabPage tabPage2;

		private TabPage tabPage3;

		private TabPage tabPage4;

		private Button saveWords;

		private RichTextBox richWords;

		private Label label5;

		private Panel panelLogs;

		private Panel panelChat;

		private Button customConnect;

		private ComboBox qYoutube;

		private Label label7;

		private Button startBoostButton;

		private CheckBox aupCheck;

		private NumericUpDown timerUpdate;

		private Label label9;

		private Panel panelUrlProxy;

		private CheckBox oldTwitch;

		private NumericUpDown threadsViews;

		private Button twitchViews;

		private Button killChromeChat;

		private Button killChromeViews;

		private TextBox urlTextBox;

		private Label label10;
        private Button button1;
        private Button button2;
        private Button startEngineViews;

		public MainFrame()
		{
			InitializeComponent();
			Log value = new Log();
			panelLogs.Controls.Add(value);
			ChatList value2 = new ChatList();
			panelChat.Controls.Add(value2);
			UrlListProxy value3 = new UrlListProxy();
			panelUrlProxy.Controls.Add(value3);
			if (File.Exists("url_list.txt"))
			{
				UrlListProxy.richTextBox1.LoadFile("url_list.txt", RichTextBoxStreamType.PlainText);
			}
			login = MyIni.Read("Login");
			autostart = MyIni.Read("Autostart");
		}

		private void MainFrame_Load(object sender, EventArgs e)
		{
			if (autostart == "True")
			{
				channelText.Text = MyIni.Read("Channel");
				serviceType.SelectedIndex = 0;
				typeProxy.SelectedIndex = Convert.ToInt32(MyIni.Read("TypeProxy"));
				uploadProxies.SelectedIndex = Convert.ToInt32(MyIni.Read("ProxyLoad"));
				setNumberThreads.Value = Convert.ToInt32(MyIni.Read("Threads"));
				timeoutThreads.Value = Convert.ToInt32(MyIni.Read("TimeoutThreads"));
				comboMethods.SelectedIndex = Convert.ToInt32(MyIni.Read("Method"));
				aupCheck.Checked = Convert.ToBoolean(MyIni.Read("AutoUpdate"));
				timerUpdate.Value = Convert.ToDecimal(MyIni.Read("AutoUpdateTimer"));
				Thread.Sleep(2000);
				startBoostButton.PerformClick();
			}
			if (autostart == "False")
			{
				channelText.Text = MyIni.Read("Channel");
				typeProxy.SelectedIndex = 0;
			}
		}

		private void saveAsButton_Click(object sender, EventArgs e)
		{
			MyIni.Write("Channel", channelText.Text);
			MyIni.Write("TypeProxy", typeProxy.SelectedIndex.ToString());
			MyIni.Write("ProxyLoad", uploadProxies.SelectedIndex.ToString());
			MyIni.Write("Threads", setNumberThreads.Value.ToString());
			MyIni.Write("Method", comboMethods.SelectedIndex.ToString());
			MyIni.Write("AutoUpdate", aupCheck.Checked.ToString());
			MyIni.Write("AutoUpdateTimer", timerUpdate.Value.ToString());
			MessageBox.Show(this, "All settings saved. Change in Settings.ini parameter Autostart:True or Autostart:False", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		private void progressUpdate_Tick(object sender, EventArgs e)
		{
			if (serviceType.SelectedIndex == 0)
			{
				statusThreads.Text = "Threads: " + TwitchBot.countWorkThreads.ToString();
				countViewers.Text = "Viewers: " + TimerMethod.countViewers;
				statusStream.Text = "Status stream: " + TimerMethod.statusStream;
			}
			if (serviceType.SelectedIndex == 1)
			{
				statusThreads.Text = "Threads: " + YoutubeBot.countWorkThreads.ToString();
			}
		}

		private void uploadProxies_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = uploadProxies.SelectedIndex;
		
			if (uploadProxies.SelectedIndex == 1)
			{
				Otherparams.GetProxyFile(login);
			}
			if (uploadProxies.SelectedIndex == 2)
			{
				TimerMethod.AuPu_Stop();
				Otherparams.GetProxyUrls(login);
				if (aupCheck.Checked)
				{
					TimerMethod.AuPu_Start(login, Convert.ToInt32(timerUpdate.Value));
					Log.richLog.Text = "Uploaded proxies from URL: " + Otherparams.smartProxyList.Length + " . Update timer: " + timerUpdate.Value.ToString() + " minutes.";
				}
			}
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			MyIni.Write("Channel", channelText.Text);
			if (working)
			{
				if (serviceType.SelectedIndex == 0)
				{
					TwitchBot.StopAll();
					startBoostButton.Text = "Start";
					channelText.Enabled = true;
					setNumberThreads.Enabled = true;
				}
				if (serviceType.SelectedIndex == 1)
				{
					YoutubeBot.StopAll();
					startBoostButton.Text = "Start";
					channelText.Enabled = true;
					setNumberThreads.Enabled = true;
				}
			}
			else
			{
				if (channelText.Text == "")
				{
					MessageBox.Show(this, "Set channel name", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				if (Otherparams.smartProxyList == null || Otherparams.smartProxyList.Length == 0)
				{
					MessageBox.Show(this, "Upload proxies", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				if (typeProxy.SelectedIndex == -1)
				{
					MessageBox.Show(this, "Set type proxy", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				if (serviceType.SelectedIndex == -1)
				{
					MessageBox.Show(this, "Set type service", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				if (comboMethods.SelectedIndex == -1)
				{
					MessageBox.Show(this, "Set type boost method", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				startBoostButton.Text = "Stop";
				channelText.Enabled = false;
				setNumberThreads.Enabled = false;
				if (serviceType.SelectedIndex == 0)
				{
					if (comboMethods.SelectedIndex == -1)
					{
						MessageBox.Show(this, "Set Twitch method", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						return;
					}
					channelText.Text = channelText.Text.ToLower();
					TwitchBot.StartAll(login, channelText.Text, Convert.ToInt32(setNumberThreads.Value), 200, typeProxy.SelectedIndex, comboMethods.SelectedIndex, oldTwitch.Checked, Convert.ToInt32(timeoutThreads.Value * 1000m), UrlListProxy.richTextBox1.Text);
					TimerMethod.OnOff_Start(channelText.Text);
				}
				if (serviceType.SelectedIndex == 1)
				{
					YoutubeBot.StartAll(login, channelText.Text, Convert.ToInt32(setNumberThreads.Value), 200, typeProxy.SelectedIndex, comboMethods.SelectedIndex, qYoutube.SelectedIndex, UrlListProxy.richTextBox1.Text);
				}
			}
			working = !working;
		}

		private void twitchMethods_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboMethods.SelectedIndex == 0)
			{
				timeoutThreads.Value = 2m;
			}
			if (comboMethods.SelectedIndex == 1)
			{
				timeoutThreads.Value = 10m;
			}
		}

	

		private void comboBoxAccs_SelectedIndexChanged(object sender, EventArgs e)
		{
            WordsAndAccs.GetMyAccs(serviceType.SelectedIndex);
            labelCountChatAccs.Text = "Accounts: " + WordsAndAccs.accountsChatList.Count;
            numericAccsTwitch.Enabled = true;
            numericAccsTwitch.Value = WordsAndAccs.accountsChatList.Count;
        }

		private void writeChatButton_Click(object sender, EventArgs e)
		{
			if (writeChatButton.Text == "Write in chat")
			{
				if (channelText.Text == "")
				{
					Interaction.MsgBox("Set channel name", MsgBoxStyle.Information);
					return;
				}
				if (WordsAndAccs.wordsList == null || WordsAndAccs.wordsList.Count == 0)
				{
					Interaction.MsgBox("Upload words list", MsgBoxStyle.Information);
					return;
				}
				if (WordsAndAccs.accountsChatList == null || WordsAndAccs.accountsChatList.Count == 0)
				{
					Interaction.MsgBox("Upload accounts list", MsgBoxStyle.Information);
					return;
				}
				if (serviceType.SelectedIndex == -1)
				{
					Interaction.MsgBox("Set service", MsgBoxStyle.Information);
					return;
				}
				if (serviceType.SelectedIndex == 0)
				{
					TimerMethod.ChatBotTwitchWrite_Start(Convert.ToInt32(numericChatWrite.Value), channelText.Text);
					writeChatButton.Text = "Stop";
				}
				if (serviceType.SelectedIndex == 1)
				{
					TimerMethod.ChatBotYoutube_Start(Convert.ToInt32(numericChatWrite.Value), channelText.Text);
					writeChatButton.Text = "Stop";
				}
			}
			else
			{
				if (serviceType.SelectedIndex == 0)
				{
					TimerMethod.ChatBotTwitchWrite_Stop();
					writeChatButton.Text = "Write in chat";
				}
				if (serviceType.SelectedIndex == 1)
				{
					ChatYoutube.stopChatYoutube();
					TimerMethod.ChatBotYoutube_Stop();
					writeChatButton.Text = "Write in chat";
				}
			}
		}

		private void connectChatTwitch_Click(object sender, EventArgs e)
		{
			if (connectChatTwitch.Text == "Connect in chat")
			{
				if (channelText.Text == "")
				{
					Interaction.MsgBox("Set channel name", MsgBoxStyle.Information);
					return;
				}
				if (WordsAndAccs.accountsChatList == null || WordsAndAccs.accountsChatList.Count == 0)
				{
					Interaction.MsgBox("Upload accounts list", MsgBoxStyle.Information);
					return;
				}
				TimerMethod.ChatBotTwitchConnect_Start(channelText.Text, Convert.ToInt32(numericAccsTwitch.Value));
				connectChatTwitch.Text = "Working...";
				connectChatTwitch.Enabled = false;
				numericAccsTwitch.Enabled = false;
			}
		}

		private void uploadAccsForSubs_Click(object sender, EventArgs e)
		{
			Otherparams.GetMyAccsSubTwitch();
			accountsSubs.Text = "Accounts: " + Otherparams.accsListSubTwitch.Length;
		}

		private void followButtonTwitch_Click(object sender, EventArgs e)
		{
			if (followButtonTwitch.Text == "Subscribe")
			{
				if (channelText.Text == "")
				{
					Interaction.MsgBox("Set channel name", MsgBoxStyle.Information);
					return;
				}
				if (Otherparams.accsListSubTwitch == null || Otherparams.accsListSubTwitch.Length == 0)
				{
					Interaction.MsgBox("Upload accounts list", MsgBoxStyle.Information);
					return;
				}
				TimerMethod.TwitchFollow_Start(channelText.Text);
				followButtonTwitch.Text = "Stop";
			}
			else
			{
				TimerMethod.TwitchFollow_Stop();
				followButtonTwitch.Text = "Subcribe";
			}
		}

		private void saveWords_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.DefaultExt = "*.txt";
			saveFileDialog.Filter = "TXT Files|*.txt";
			if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName.Length > 0)
			{
				File.WriteAllText(saveFileDialog.FileName, richWords.Text, Encoding.UTF8);
			}
		}

		private void serviceType_SelectedIndexChanged(object sender, EventArgs e)
		{
			int selectedIndex = serviceType.SelectedIndex;
			if (serviceType.SelectedIndex == 0)
			{
				connectChatTwitch.Enabled = true;
				comboMethods.Enabled = true;
			
				qYoutube.Enabled = false;
			}
			if (serviceType.SelectedIndex == 1)
			{
				connectChatTwitch.Enabled = false;
				comboMethods.Enabled = true;
		
				qYoutube.Enabled = true;
				qYoutube.SelectedIndex = 2;
			}
			if (serviceType.SelectedIndex == 2)
			{
				comboMethods.Enabled = false;
			
				qYoutube.Enabled = false;
			}
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void twitchViews_Click(object sender, EventArgs e)
		{
			if (working)
			{
				twitchViews.Text = "Start views";
				ViewsBot.StopAll();
			}
			else
			{
				if (urlTextBox.Text == "")
				{
					MessageBox.Show(this, "Set url", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				if (Otherparams.smartProxyList == null || Otherparams.smartProxyList.Length == 0)
				{
					MessageBox.Show(this, "Upload proxies", "Alert", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					return;
				}
				Otherparams.GetUA();
				ViewsBot.startViews(urlTextBox.Text, typeProxy.SelectedIndex, Convert.ToInt32(threadsViews.Value));
				twitchViews.Text = "Stop views";
			}
			working = !working;
		}

		private void killChromeChat_Click(object sender, EventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("chromedriver");
			foreach (Process process in processesByName)
			{
				process.Kill();
			}
		}

		private void killChromeViews_Click(object sender, EventArgs e)
		{
			Process[] processesByName = Process.GetProcessesByName("chromedriver");
			foreach (Process process in processesByName)
			{
				process.Kill();
			}
		}

		private void startEngineViews_Click(object sender, EventArgs e)
		{
			string arguments = urlTextBox.Text + " " + typeProxy.SelectedIndex.ToString() + " " + Otherparams.smartProxyList[proxyRand.Next(Otherparams.smartProxyList.Length)] + " Mozilla/5.0 (Linux; Android 6.0.1; SM-G532G Build/MMB29T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.83 Mobile Safari/537.36";
			Process process = new Process();
			ProcessStartInfo startInfo = process.StartInfo;
			startInfo.FileName = Application.StartupPath + "\\rubotEngine.exe";
			startInfo.Arguments = arguments;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardError = false;
			startInfo.RedirectStandardOutput = true;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.Start();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.typeProxy = new System.Windows.Forms.ComboBox();
            this.uploadProxies = new System.Windows.Forms.ComboBox();
            this.serviceType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.channelText = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.oldTwitch = new System.Windows.Forms.CheckBox();
            this.qYoutube = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboMethods = new System.Windows.Forms.ComboBox();
            this.startBoostButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.timeoutThreads = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.setNumberThreads = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusThreads = new System.Windows.Forms.ToolStripStatusLabel();
            this.methodLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStream = new System.Windows.Forms.ToolStripStatusLabel();
            this.countViewers = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressUpdate = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panelUrlProxy = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.timerUpdate = new System.Windows.Forms.NumericUpDown();
            this.aupCheck = new System.Windows.Forms.CheckBox();
            this.labelCountWords = new System.Windows.Forms.Label();
            this.labelCountChatAccs = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numericChatWrite = new System.Windows.Forms.NumericUpDown();
            this.writeChatButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.numericAccsTwitch = new System.Windows.Forms.NumericUpDown();
            this.connectChatTwitch = new System.Windows.Forms.Button();
            this.accountsSubs = new System.Windows.Forms.Label();
            this.uploadAccsForSubs = new System.Windows.Forms.Button();
            this.followButtonTwitch = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.killChromeChat = new System.Windows.Forms.Button();
            this.customConnect = new System.Windows.Forms.Button();
            this.panelChat = new System.Windows.Forms.Panel();
            this.saveWords = new System.Windows.Forms.Button();
            this.richWords = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.startEngineViews = new System.Windows.Forms.Button();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.killChromeViews = new System.Windows.Forms.Button();
            this.threadsViews = new System.Windows.Forms.NumericUpDown();
            this.twitchViews = new System.Windows.Forms.Button();
            this.panelLogs = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setNumberThreads)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerUpdate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChatWrite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAccsTwitch)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadsViews)).BeginInit();
            this.SuspendLayout();
            // 
            // typeProxy
            // 
            this.typeProxy.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.typeProxy.FormattingEnabled = true;
            this.typeProxy.Items.AddRange(new object[] {
            "HTTP(s)",
            "SOCKS4",
            "SOCKS5"});
            this.typeProxy.Location = new System.Drawing.Point(9, 24);
            this.typeProxy.Name = "typeProxy";
            this.typeProxy.Size = new System.Drawing.Size(102, 25);
            this.typeProxy.TabIndex = 74;
            this.typeProxy.Text = "Type proxies";
            this.typeProxy.SelectedIndexChanged += new System.EventHandler(this.typeProxy_SelectedIndexChanged);
            // 
            // uploadProxies
            // 
            this.uploadProxies.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.uploadProxies.FormattingEnabled = true;
            this.uploadProxies.Items.AddRange(new object[] {
            "Rubot proxies",
            "My proxies",
            "Proxy from URL"});
            this.uploadProxies.Location = new System.Drawing.Point(120, 24);
            this.uploadProxies.Name = "uploadProxies";
            this.uploadProxies.Size = new System.Drawing.Size(102, 25);
            this.uploadProxies.TabIndex = 75;
            this.uploadProxies.Text = "Proxies";
            this.uploadProxies.SelectedIndexChanged += new System.EventHandler(this.uploadProxies_SelectedIndexChanged);
            // 
            // serviceType
            // 
            this.serviceType.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.serviceType.FormattingEnabled = true;
            this.serviceType.Items.AddRange(new object[] {
            "Twitch",
            "Youtube",
            "Chaturbate"});
            this.serviceType.Location = new System.Drawing.Point(247, 29);
            this.serviceType.Name = "serviceType";
            this.serviceType.Size = new System.Drawing.Size(151, 25);
            this.serviceType.TabIndex = 76;
            this.serviceType.Text = "Service";
            this.serviceType.SelectedIndexChanged += new System.EventHandler(this.serviceType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label2.Location = new System.Drawing.Point(3, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 17);
            this.label2.TabIndex = 77;
            this.label2.Text = "Channel name or ID:";
            // 
            // channelText
            // 
            this.channelText.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.channelText.Location = new System.Drawing.Point(6, 29);
            this.channelText.Name = "channelText";
            this.channelText.Size = new System.Drawing.Size(232, 25);
            this.channelText.TabIndex = 78;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.oldTwitch);
            this.groupBox3.Controls.Add(this.qYoutube);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.comboMethods);
            this.groupBox3.Controls.Add(this.startBoostButton);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.timeoutThreads);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.setNumberThreads);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.groupBox3.Location = new System.Drawing.Point(6, 189);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(601, 161);
            this.groupBox3.TabIndex = 80;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "2. Settings";
            // 
            // oldTwitch
            // 
            this.oldTwitch.AutoSize = true;
            this.oldTwitch.Location = new System.Drawing.Point(502, 32);
            this.oldTwitch.Name = "oldTwitch";
            this.oldTwitch.Size = new System.Drawing.Size(91, 21);
            this.oldTwitch.TabIndex = 87;
            this.oldTwitch.Text = "OLD Twitch";
            this.oldTwitch.UseVisualStyleBackColor = true;
            // 
            // qYoutube
            // 
            this.qYoutube.Enabled = false;
            this.qYoutube.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.qYoutube.FormattingEnabled = true;
            this.qYoutube.Items.AddRange(new object[] {
            "144",
            "240",
            "360",
            "480",
            "720 (30)",
            "720 (60)",
            "1080",
            "1080(60)"});
            this.qYoutube.Location = new System.Drawing.Point(373, 92);
            this.qYoutube.Name = "qYoutube";
            this.qYoutube.Size = new System.Drawing.Size(123, 25);
            this.qYoutube.TabIndex = 86;
            this.qYoutube.Text = "Quality";
            this.qYoutube.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label7.Location = new System.Drawing.Point(274, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 17);
            this.label7.TabIndex = 85;
            this.label7.Text = "Quality (YT):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label5.Location = new System.Drawing.Point(274, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 17);
            this.label5.TabIndex = 84;
            this.label5.Text = "Boost method:";
            // 
            // comboMethods
            // 
            this.comboMethods.Enabled = false;
            this.comboMethods.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.comboMethods.FormattingEnabled = true;
            this.comboMethods.Items.AddRange(new object[] {
            "Hard mode",
            "Slow mode"});
            this.comboMethods.Location = new System.Drawing.Point(373, 30);
            this.comboMethods.Name = "comboMethods";
            this.comboMethods.Size = new System.Drawing.Size(123, 25);
            this.comboMethods.TabIndex = 83;
            this.comboMethods.Text = "Select method";
            this.comboMethods.SelectedIndexChanged += new System.EventHandler(this.twitchMethods_SelectedIndexChanged);
            // 
            // startBoostButton
            // 
            this.startBoostButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.startBoostButton.Location = new System.Drawing.Point(131, 124);
            this.startBoostButton.Name = "startBoostButton";
            this.startBoostButton.Size = new System.Drawing.Size(275, 34);
            this.startBoostButton.TabIndex = 79;
            this.startBoostButton.Text = "Start";
            this.startBoostButton.UseVisualStyleBackColor = true;
            this.startBoostButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label4.Location = new System.Drawing.Point(274, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 17);
            this.label4.TabIndex = 79;
            this.label4.Text = "User-Agents:";
            // 
            // comboBox1
            // 
            this.comboBox1.Enabled = false;
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Web (Chrome)",
            "Android",
            "iOS"});
            this.comboBox1.Location = new System.Drawing.Point(373, 61);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(123, 25);
            this.comboBox1.TabIndex = 78;
            this.comboBox1.Text = "Platform";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label3.Location = new System.Drawing.Point(6, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(176, 17);
            this.label3.TabIndex = 65;
            this.label3.Text = "Threads (active connections):";
            // 
            // timeoutThreads
            // 
            this.timeoutThreads.AutoSize = true;
            this.timeoutThreads.DecimalPlaces = 1;
            this.timeoutThreads.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.timeoutThreads.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.timeoutThreads.Location = new System.Drawing.Point(188, 62);
            this.timeoutThreads.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.timeoutThreads.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.timeoutThreads.Name = "timeoutThreads";
            this.timeoutThreads.Size = new System.Drawing.Size(55, 25);
            this.timeoutThreads.TabIndex = 63;
            this.timeoutThreads.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label1.Location = new System.Drawing.Point(6, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 17);
            this.label1.TabIndex = 62;
            this.label1.Text = "Timeout connections:";
            // 
            // setNumberThreads
            // 
            this.setNumberThreads.AutoSize = true;
            this.setNumberThreads.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.setNumberThreads.Location = new System.Drawing.Point(188, 31);
            this.setNumberThreads.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.setNumberThreads.Name = "setNumberThreads";
            this.setNumberThreads.Size = new System.Drawing.Size(52, 25);
            this.setNumberThreads.TabIndex = 28;
            this.setNumberThreads.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusThreads,
            this.methodLabel,
            this.statusStream,
            this.countViewers});
            this.statusStrip1.Location = new System.Drawing.Point(0, 523);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(630, 22);
            this.statusStrip1.TabIndex = 82;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusThreads
            // 
            this.statusThreads.Name = "statusThreads";
            this.statusThreads.Size = new System.Drawing.Size(153, 17);
            this.statusThreads.Spring = true;
            this.statusThreads.Text = "Threads:";
            this.statusThreads.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // methodLabel
            // 
            this.methodLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.methodLabel.Name = "methodLabel";
            this.methodLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.methodLabel.Size = new System.Drawing.Size(153, 17);
            this.methodLabel.Spring = true;
            this.methodLabel.Text = "Method:";
            this.methodLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusStream
            // 
            this.statusStream.Name = "statusStream";
            this.statusStream.Size = new System.Drawing.Size(153, 17);
            this.statusStream.Spring = true;
            this.statusStream.Text = "Status: UNKNOW";
            // 
            // countViewers
            // 
            this.countViewers.Name = "countViewers";
            this.countViewers.Size = new System.Drawing.Size(153, 17);
            this.countViewers.Spring = true;
            this.countViewers.Text = "Viewers: 0";
            // 
            // progressUpdate
            // 
            this.progressUpdate.Enabled = true;
            this.progressUpdate.Interval = 200;
            this.progressUpdate.Tick += new System.EventHandler(this.progressUpdate_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 10000;
            this.toolTip1.InitialDelay = 100;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Help";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panelUrlProxy);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.timerUpdate);
            this.groupBox1.Controls.Add(this.aupCheck);
            this.groupBox1.Controls.Add(this.typeProxy);
            this.groupBox1.Controls.Add(this.uploadProxies);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(601, 177);
            this.groupBox1.TabIndex = 83;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1. Upload Proxies";
            // 
            // panelUrlProxy
            // 
            this.panelUrlProxy.Location = new System.Drawing.Point(9, 56);
            this.panelUrlProxy.Name = "panelUrlProxy";
            this.panelUrlProxy.Size = new System.Drawing.Size(586, 118);
            this.panelUrlProxy.TabIndex = 81;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(358, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 17);
            this.label9.TabIndex = 80;
            this.label9.Text = "minutes";
            // 
            // timerUpdate
            // 
            this.timerUpdate.Location = new System.Drawing.Point(320, 25);
            this.timerUpdate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timerUpdate.Name = "timerUpdate";
            this.timerUpdate.Size = new System.Drawing.Size(37, 25);
            this.timerUpdate.TabIndex = 79;
            this.timerUpdate.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // aupCheck
            // 
            this.aupCheck.AutoSize = true;
            this.aupCheck.Location = new System.Drawing.Point(231, 26);
            this.aupCheck.Name = "aupCheck";
            this.aupCheck.Size = new System.Drawing.Size(95, 21);
            this.aupCheck.TabIndex = 78;
            this.aupCheck.Text = "Autoupdate";
            this.aupCheck.UseVisualStyleBackColor = true;
            // 
            // labelCountWords
            // 
            this.labelCountWords.AutoSize = true;
            this.labelCountWords.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelCountWords.Location = new System.Drawing.Point(517, 12);
            this.labelCountWords.Name = "labelCountWords";
            this.labelCountWords.Size = new System.Drawing.Size(60, 17);
            this.labelCountWords.TabIndex = 60;
            this.labelCountWords.Text = "Words: 0";
            // 
            // labelCountChatAccs
            // 
            this.labelCountChatAccs.AutoSize = true;
            this.labelCountChatAccs.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.labelCountChatAccs.Location = new System.Drawing.Point(517, 48);
            this.labelCountChatAccs.Name = "labelCountChatAccs";
            this.labelCountChatAccs.Size = new System.Drawing.Size(74, 17);
            this.labelCountChatAccs.TabIndex = 62;
            this.labelCountChatAccs.Text = "Accounts: 0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label6.Location = new System.Drawing.Point(378, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 17);
            this.label6.TabIndex = 63;
            this.label6.Text = "Periodicity of spelling:";
            // 
            // numericChatWrite
            // 
            this.numericChatWrite.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.numericChatWrite.Location = new System.Drawing.Point(557, 80);
            this.numericChatWrite.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericChatWrite.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericChatWrite.Name = "numericChatWrite";
            this.numericChatWrite.Size = new System.Drawing.Size(50, 25);
            this.numericChatWrite.TabIndex = 64;
            this.numericChatWrite.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // writeChatButton
            // 
            this.writeChatButton.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.writeChatButton.Location = new System.Drawing.Point(381, 112);
            this.writeChatButton.Name = "writeChatButton";
            this.writeChatButton.Size = new System.Drawing.Size(226, 25);
            this.writeChatButton.TabIndex = 65;
            this.writeChatButton.Text = "Write in chat";
            this.writeChatButton.UseVisualStyleBackColor = true;
            this.writeChatButton.Click += new System.EventHandler(this.writeChatButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label8.Location = new System.Drawing.Point(388, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(124, 17);
            this.label8.TabIndex = 66;
            this.label8.Text = "How many connect?";
            // 
            // numericAccsTwitch
            // 
            this.numericAccsTwitch.Enabled = false;
            this.numericAccsTwitch.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.numericAccsTwitch.Location = new System.Drawing.Point(557, 163);
            this.numericAccsTwitch.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericAccsTwitch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericAccsTwitch.Name = "numericAccsTwitch";
            this.numericAccsTwitch.Size = new System.Drawing.Size(50, 25);
            this.numericAccsTwitch.TabIndex = 67;
            this.numericAccsTwitch.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // connectChatTwitch
            // 
            this.connectChatTwitch.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.connectChatTwitch.Location = new System.Drawing.Point(381, 194);
            this.connectChatTwitch.Name = "connectChatTwitch";
            this.connectChatTwitch.Size = new System.Drawing.Size(226, 25);
            this.connectChatTwitch.TabIndex = 68;
            this.connectChatTwitch.Text = "Connect in chat";
            this.connectChatTwitch.UseVisualStyleBackColor = true;
            this.connectChatTwitch.Click += new System.EventHandler(this.connectChatTwitch_Click);
            // 
            // accountsSubs
            // 
            this.accountsSubs.AutoSize = true;
            this.accountsSubs.Location = new System.Drawing.Point(6, 16);
            this.accountsSubs.Name = "accountsSubs";
            this.accountsSubs.Size = new System.Drawing.Size(74, 17);
            this.accountsSubs.TabIndex = 57;
            this.accountsSubs.Text = "Accounts: 0";
            // 
            // uploadAccsForSubs
            // 
            this.uploadAccsForSubs.Location = new System.Drawing.Point(9, 46);
            this.uploadAccsForSubs.Name = "uploadAccsForSubs";
            this.uploadAccsForSubs.Size = new System.Drawing.Size(121, 30);
            this.uploadAccsForSubs.TabIndex = 56;
            this.uploadAccsForSubs.Text = "Upload accounts";
            this.uploadAccsForSubs.UseVisualStyleBackColor = true;
            this.uploadAccsForSubs.Click += new System.EventHandler(this.uploadAccsForSubs_Click);
            // 
            // followButtonTwitch
            // 
            this.followButtonTwitch.Location = new System.Drawing.Point(9, 82);
            this.followButtonTwitch.Name = "followButtonTwitch";
            this.followButtonTwitch.Size = new System.Drawing.Size(121, 30);
            this.followButtonTwitch.TabIndex = 0;
            this.followButtonTwitch.Text = "Subscribe";
            this.followButtonTwitch.UseVisualStyleBackColor = true;
            this.followButtonTwitch.Click += new System.EventHandler(this.followButtonTwitch_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.tabControl1.Location = new System.Drawing.Point(6, 60);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(621, 385);
            this.tabControl1.TabIndex = 94;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(613, 355);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Viewers Mode";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.killChromeChat);
            this.tabPage2.Controls.Add(this.customConnect);
            this.tabPage2.Controls.Add(this.panelChat);
            this.tabPage2.Controls.Add(this.saveWords);
            this.tabPage2.Controls.Add(this.richWords);
            this.tabPage2.Controls.Add(this.connectChatTwitch);
            this.tabPage2.Controls.Add(this.numericAccsTwitch);
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.writeChatButton);
            this.tabPage2.Controls.Add(this.labelCountWords);
            this.tabPage2.Controls.Add(this.numericChatWrite);
            this.tabPage2.Controls.Add(this.labelCountChatAccs);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(613, 355);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Chat-Bots";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // killChromeChat
            // 
            this.killChromeChat.Location = new System.Drawing.Point(381, 238);
            this.killChromeChat.Name = "killChromeChat";
            this.killChromeChat.Size = new System.Drawing.Size(226, 30);
            this.killChromeChat.TabIndex = 74;
            this.killChromeChat.Text = "Kill chromedriver.exe";
            this.killChromeChat.UseVisualStyleBackColor = true;
            this.killChromeChat.Click += new System.EventHandler(this.killChromeChat_Click);
            // 
            // customConnect
            // 
            this.customConnect.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.customConnect.Location = new System.Drawing.Point(3, 308);
            this.customConnect.Name = "customConnect";
            this.customConnect.Size = new System.Drawing.Size(144, 25);
            this.customConnect.TabIndex = 73;
            this.customConnect.Text = "Custom connect";
            this.customConnect.UseVisualStyleBackColor = true;
            // 
            // panelChat
            // 
            this.panelChat.Location = new System.Drawing.Point(3, 7);
            this.panelChat.Name = "panelChat";
            this.panelChat.Size = new System.Drawing.Size(144, 295);
            this.panelChat.TabIndex = 72;
            // 
            // saveWords
            // 
            this.saveWords.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.saveWords.Location = new System.Drawing.Point(157, 308);
            this.saveWords.Name = "saveWords";
            this.saveWords.Size = new System.Drawing.Size(215, 25);
            this.saveWords.TabIndex = 71;
            this.saveWords.Text = "Save text";
            this.saveWords.UseVisualStyleBackColor = true;
            this.saveWords.Click += new System.EventHandler(this.saveWords_Click);
            // 
            // richWords
            // 
            this.richWords.Location = new System.Drawing.Point(157, 7);
            this.richWords.Name = "richWords";
            this.richWords.Size = new System.Drawing.Size(215, 295);
            this.richWords.TabIndex = 70;
            this.richWords.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.followButtonTwitch);
            this.tabPage3.Controls.Add(this.uploadAccsForSubs);
            this.tabPage3.Controls.Add(this.accountsSubs);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(613, 355);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Followers Mode";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.startEngineViews);
            this.tabPage4.Controls.Add(this.urlTextBox);
            this.tabPage4.Controls.Add(this.label10);
            this.tabPage4.Controls.Add(this.killChromeViews);
            this.tabPage4.Controls.Add(this.threadsViews);
            this.tabPage4.Controls.Add(this.twitchViews);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(613, 355);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Views Mode";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // startEngineViews
            // 
            this.startEngineViews.Location = new System.Drawing.Point(463, 15);
            this.startEngineViews.Name = "startEngineViews";
            this.startEngineViews.Size = new System.Drawing.Size(129, 25);
            this.startEngineViews.TabIndex = 97;
            this.startEngineViews.Text = "View Boost 2";
            this.startEngineViews.UseVisualStyleBackColor = true;
            this.startEngineViews.Visible = false;
            this.startEngineViews.Click += new System.EventHandler(this.startEngineViews_Click);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.urlTextBox.Location = new System.Drawing.Point(46, 16);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(232, 25);
            this.urlTextBox.TabIndex = 96;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 17);
            this.label10.TabIndex = 76;
            this.label10.Text = "URL:";
            // 
            // killChromeViews
            // 
            this.killChromeViews.Location = new System.Drawing.Point(205, 296);
            this.killChromeViews.Name = "killChromeViews";
            this.killChromeViews.Size = new System.Drawing.Size(173, 25);
            this.killChromeViews.TabIndex = 75;
            this.killChromeViews.Text = "Kill chromedriver.exe";
            this.killChromeViews.UseVisualStyleBackColor = true;
            this.killChromeViews.Click += new System.EventHandler(this.killChromeViews_Click);
            // 
            // threadsViews
            // 
            this.threadsViews.Enabled = false;
            this.threadsViews.Location = new System.Drawing.Point(284, 16);
            this.threadsViews.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.threadsViews.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threadsViews.Name = "threadsViews";
            this.threadsViews.Size = new System.Drawing.Size(38, 25);
            this.threadsViews.TabIndex = 1;
            this.threadsViews.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // twitchViews
            // 
            this.twitchViews.Location = new System.Drawing.Point(328, 16);
            this.twitchViews.Name = "twitchViews";
            this.twitchViews.Size = new System.Drawing.Size(129, 25);
            this.twitchViews.TabIndex = 0;
            this.twitchViews.Text = "View Boost";
            this.twitchViews.UseVisualStyleBackColor = true;
            this.twitchViews.Click += new System.EventHandler(this.twitchViews_Click);
            // 
            // panelLogs
            // 
            this.panelLogs.Location = new System.Drawing.Point(6, 448);
            this.panelLogs.Name = "panelLogs";
            this.panelLogs.Size = new System.Drawing.Size(617, 72);
            this.panelLogs.TabIndex = 95;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(385, 40);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 25);
            this.button1.TabIndex = 75;
            this.button1.Text = "Аккаунты";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(385, 9);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(129, 25);
            this.button2.TabIndex = 76;
            this.button2.Text = "World";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelLogs);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.channelText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serviceType);
            this.Controls.Add(this.statusStrip1);
            this.Name = "MainFrame";
            this.Size = new System.Drawing.Size(630, 545);
            this.Load += new System.EventHandler(this.MainFrame_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setNumberThreads)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerUpdate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericChatWrite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericAccsTwitch)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadsViews)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private void typeProxy_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            WordsAndAccs.GetMyAccs(serviceType.SelectedIndex);
            labelCountChatAccs.Text = "Accounts: " + WordsAndAccs.accountsChatList.Count;
            numericAccsTwitch.Enabled = true;
            numericAccsTwitch.Value = WordsAndAccs.accountsChatList.Count;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WordsAndAccs.GetMyWords();
            labelCountWords.Text = "Words: " + WordsAndAccs.wordsList.Count;
        }
    }
}
