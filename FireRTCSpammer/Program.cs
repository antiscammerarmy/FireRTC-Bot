using DSharpPlus;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireRTCBot
{
	
    public partial class Program
    {
		public static Dictionary<string, SoundPlayer> Soundlist = new Dictionary<string, SoundPlayer>();		
		#region FireRTC
		public static ChromeDriver cd;
		//Hide console stuff.

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        //Info storage.
        public static string email;
        public static string password;

        //Have we told the user to press 'accept' on the microphone pop-up?
        private static bool hasAuthenticated = false;
		#endregion
		//Cross-Calling System
		public class CallObject
		{
			public MessageCreateEventArgs e;
			public DiscordMessage msg;
			public DiscordChannel chnl;
			public DiscordEmbed embed;
			public DateTime? _start = null;
			public string number;
		}
		public class CallObjectHistory
		{
			public string user;
			public string chnl;
			public TimeSpan time;
			public string number;
		}
		public static CallObject _CurrentCall;

		public static List<CallObject> CallQueue = new List<CallObject>();
		public static List<CallObjectHistory> CallHistory = new List<CallObjectHistory>();

		public static string CallerID = "";

		private static string GenerateReport(Exception ex) => $"Source: {ex.Source}\nMessage: {ex.Message}\nHelpLink: {ex.HelpLink}\nStack Trace =>\n{ex.StackTrace}\n<=\n";
		public static void Crash(Exception ex, string Application)
		{
			Log(LogType.Error, ex.Message, Application); //Log Error Message to Console
			try
			{
				int filename = 0; while (File.Exists($"error\\{filename}.txt")) { filename++; } // Find a available filename.
				File.WriteAllText($"error\\{filename}.txt", GenerateReport(ex)); //Save Error Report
			}
			catch (Exception) { Log(LogType.Error, "Failed to save report.", "Crash"); } //Save Error-Report
		}
		static void Main(string[] args)
		{
			new Program().RunAsync().GetAwaiter().GetResult();
        }
		public static void InitFireRTC()
		{
			MainForm mf = new MainForm();
			mf.ShowDialog();

			//If the user enters invalid credentials..
			tryAuthenticationAgain:
			cd.Manage().Window.Maximize();
			//Sign in..
			#region Auto-Login
			cd.Url = "https://phone.firertc.com/auth/sign_in";
			cd.FindElement(By.Id("user_email")).SendKeys(email);
			cd.FindElement(By.Id("user_password")).SendKeys(password);
			cd.FindElement(By.Name("commit")).Click();
			Thread.Sleep(3000);
			#endregion
			//After the user has attempted authentication, if the e-mail field to login is still visible, assume we failed the authentication process.

			try
			{
				cd.FindElementById("user_email");
				cd.Close();
				MessageBox.Show("You failed to provide the program with proper credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				MainForm ms = new MainForm();
				ms.ShowDialog();
				goto tryAuthenticationAgain;
			}
			catch { }
			API.ChangeNumber();
			cd.Url = "https://phone.firertc.com/phone";
			//If we haven't already told the user to 'Allow' microphone access, tell them.
			if (!hasAuthenticated)
			{
				MessageBox.Show("Setup Microphone please.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				hasAuthenticated = true;
			}
			//Check if FireRTC unregistered itself.
			try
			{
				var connectButton = cd.FindElementsByClassName("row");
				foreach (IWebElement i in connectButton)
				{
					if (i.GetAttribute("innerText").Contains("Connect"))
					{
						i.Click();
					}
				}
				Thread.Sleep(2000);
			}
			catch { } //Cool, FireRTC didn't unregister itself.
			//Check if the current FireRTC account is banned.
			try
			{
				cd.FindElementById("user_email");
				MessageBox.Show("This FireRTC account has been banned.", "Banned", MessageBoxButtons.OK, MessageBoxIcon.Error);
				goto tryAuthenticationAgain;
			}
			catch { } //The account isn't banned.
					  //Wait for FireRTC to connect.
			try
			{
				cd.FindElementById("button_1"); //Check for the keypad number one. If it exists, we have connected to the phone.
			}
			catch { MessageBox.Show("Couldn't find keypad button!", "", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
		}
		


		public static DiscordClient Client;
		public async Task RunAsync()
		{
			Config.Set();
			#region Init
			Log(LogType.Normal, "Started", Config.BotName);
			foreach (string item in new string[] { "error\\" }) { CheckDirectory(item); }
			Client = new DiscordClient(new DiscordConfig()
			{
				Token = Config.Token_Discord,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				LogLevel = DSharpPlus.LogLevel.Info,
				UseInternalLogHandler = false,
				DiscordBranch = Branch.PTB,
				LargeThreshold = 256,
				EnableCompression = true,
				AutomaticGuildSync = true,
			});
			Client.SetWebSocketClient<WebSocket4NetClient>();
			Log(LogType.Normal, "Bot configured", Config.BotName);
			foreach (var item in Directory.GetFiles("media\\", "*.wav"))
			{
				Soundlist.Add(Path.GetFileNameWithoutExtension(item).ToLower(), new SoundPlayer(item));
				Log(LogType.Info, "New Sound: " + item, "SoundPlayer");
			}
			#endregion Init
			#region Events
			Client.Ready += async (ReadyEventArgs e) => await Events.Ready(Client, e);
			Client.MessageCreated += async (e) => await Events.MessageCreated(Client, e);
			Client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
			Log(LogType.Normal, "Events registered", Config.BotName);
			#endregion Events
			cd = new ChromeDriver();
			InitFireRTC();
			#region Rest
			Log(LogType.Normal, "Connecting", Config.BotName);
			await Client.ConnectAsync();
			Log(LogType.Normal, "Connected", Config.BotName);
			await Client.UpdateStatusAsync(user_status: UserStatus.Online);
			#endregion
			while (true)
			{
				try
				{
					string embedstatus = "Please wait...";
					call:
					await Task.Delay(5000);
					#region Nullcheck
					while (_CurrentCall == null)
					{
						if (CallQueue.Count != 0)
						{
							_CurrentCall = CallQueue[0];
							API.Call(_CurrentCall.number);
							CallQueue.Remove(_CurrentCall);
							_CurrentCall.msg = await _CurrentCall.e.Message.RespondAsync($"Calling next number. **``{CallQueue.Count}``** numbers in queue.", false, _CurrentCall.embed);
						}
					}
					if (_CurrentCall._start == null)
					{
						_CurrentCall._start = DateTime.Now;
					}
					#endregion
					if (_CurrentCall.msg == null)
					{
						_CurrentCall.msg = await _CurrentCall.chnl.SendMessageAsync("", false, _CurrentCall.embed);
					}
					switch (API.Status())
					{
						case API.FireRTCStatus.Online:
							embedstatus = "Hang Up";
							_CurrentCall.embed.Color = 0xf44242;
							break;
						case API.FireRTCStatus.Calling:
							embedstatus = "Calling...";
							_CurrentCall.embed.Color = 0xf4b841;
							break;
						case API.FireRTCStatus.Talking:
							embedstatus = "Talking";
							_CurrentCall.embed.Color = 0xaff441;
							break;
						case API.FireRTCStatus.Unknown:
							embedstatus = "Unknown...";
							_CurrentCall.embed.Color = 0x7e8fb5;
							break;
					}
					var elapsed = ((TimeSpan)(DateTime.Now - _CurrentCall._start));
					#region Message
					_CurrentCall.embed.Description = $@"**Status** {embedstatus}
**Number** {_CurrentCall.number}
**Elapsed Time** {elapsed.Minutes.ToString().PadLeft(2, '0')}:{elapsed.Seconds.ToString().PadLeft(2, '0')}
**Queued Numbers** {CallQueue.Count}";
					await _CurrentCall.msg.EditAsync(_CurrentCall.msg.Content, _CurrentCall.embed);
					#endregion
					if (API.Status() == API.FireRTCStatus.Online)
					{
						CallHistory.Add(new CallObjectHistory()
						{
							chnl = _CurrentCall.e.Channel.Name,
							number = _CurrentCall.number,
							time = elapsed,
							user = _CurrentCall.e.Author.Username
						});
						_CurrentCall = null;
					}
					goto call;
				}
				catch (Exception ex)
				{
					Crash(ex, "CallSystem");
				}
				
			}
		}
		public static async void Shutdown()
		{
			await Client.UpdateStatusAsync(new Game() { Name = "Shutting Down..." }, UserStatus.DoNotDisturb);
			await Client.DisconnectAsync();
			cd.Quit();
			Environment.Exit(0);
		}
		public static void CheckDirectory(string Directory)
		{
			if (!System.IO.Directory.Exists(Directory))
			{
				Log(LogType.Warning, $"Folder '{Directory}' wasn't found! This Folder gets created.");
				System.IO.Directory.CreateDirectory(Directory);
			}
		}
	}
}
