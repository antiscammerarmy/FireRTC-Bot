using DSharpPlus;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FireRTCBot
{
	
    public partial class Program
    {
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
			try
			{
				new Program().RunAsync().GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				Crash(ex, "Main");
			}
        }
		public static void InitFireRTC()
		{

			//Open the main user interface.
			MainForm mf = new MainForm();
			mf.ShowDialog();

			//If the user enters invalid credentials..
			tryAuthenticationAgain:
			cd.Manage().Window.Maximize();
			//Sign in..
			cd.Url = "https://phone.firertc.com/auth/sign_in";
			cd.FindElement(By.Id("user_email")).SendKeys(email);
			cd.FindElement(By.Id("user_password")).SendKeys(password);
			cd.FindElement(By.Name("commit")).Click();
			Thread.Sleep(3000);
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
			catch { } //Good, the user authenticated successfully.
			//We will go here after every call completion.
			startSpam:

			//Go to FireRTC's setting page.
			cd.Url = "https://phone.firertc.com/settings";

			//Wait a little bit and accept the alert that comes up.
			Thread.Sleep(1000);
			try { cd.SwitchTo().Alert().Accept(); } catch { } //This is a try-catch because the first call we go through, there will be no alert.
			//Generate a random phone number.
			Random rand = new Random();
			List<int> randomNumbers = new List<int>();
			string largeNumber = "";
			for (int i = 0; i < 10; i++)
			{
				if (i == 0)
				{
					//Start the first number in the phone number with at least a one. Otherwise, FireRTC will reject the new number.
					randomNumbers.Add(rand.Next(1, 10));
				}
				else
				{
					//If the number isn't the first number, add any number, including zero.
					randomNumbers.Add(rand.Next(0, 10));
				}
			}
			//Form everything together to create our fake phone number.
			foreach (int i in randomNumbers)
			{
				largeNumber += i.ToString();
			}
			//Send the spoofed number to FireRTC.
			cd.FindElement(By.Id("address_ua_config_display_name")).Clear();
			cd.FindElement(By.Id("address_ua_config_display_name")).SendKeys(largeNumber);
			var c = cd.FindElements(By.ClassName("form-group"));
			foreach (IWebElement element in c)
			{
				element.Click();
			}
			//Go to the phone.
			cd.Url = "https://phone.firertc.com/phone";
			//If we haven't already told the user to 'Allow' microphone access, tell them.
			if (!hasAuthenticated)
			{
				MessageBox.Show("Please click 'Allow' on the Chrome window to allow microphone access (if it shows up), then press 'OK' here. (You will only need to do this once)", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
			while (true)
			{
				try
				{
					cd.FindElementById("button_1"); //Check for the keypad number one. If it exists, we have connected to the phone.
					break;
				} catch
				{ }
			}
		}
		public static CallResult Call(string number)
		{
			number = number.Replace("+","");
			number = number.Replace("-", "");
			number = number.Replace(" ", "");
			//FireRTC wouldn't let us type directly in the phone text field, so this just presses the corresponding numbers on the number pad.
			foreach (char ch in number)
			{
				cd.FindElement(By.Id("button_" + ch)).Click();
			}

			//Call the victim!
			var d = cd.FindElements(By.ClassName("btn"));
			CallResult result = CallResult.Fail;
			foreach (IWebElement e in d)
			{
				if (e.GetAttribute("data-action") == "call")
				{
					e.Click();
					result = CallResult.Calling;
				}
			}
			return result;
		}
		public enum CallResult
		{
			Fail,
			Calling
		}


		public static DiscordClient Client;
		public async Task RunAsync()
		{
			Console.Title = Config.BotName;

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
			//=========================================================
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
