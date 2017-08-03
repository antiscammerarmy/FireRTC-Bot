using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace FireRTCBot
{
	public static class API
	{
		private static ChromeDriver cd = Program.cd;
		//VARIABLES
		public static string StatusBox() => Program.cd.FindElementByClassName("status").Text;
		public static FireRTCStatus Status()
		{
			var statusBox = StatusBox().ToLower();
			if (statusBox.Contains("calling"))
				return FireRTCStatus.Calling;
			else if (statusBox.Contains("speaking"))
				return FireRTCStatus.Talking;
			else if (statusBox.Contains("is online") && API.IsOnline)
				return FireRTCStatus.Online;
			else
				return FireRTCStatus.Unknown;
		}
		//BOOLEANS
		public static bool IsElementPresent(By By)
		{
			try
			{
				cd.FindElement(By);
				return true;
			}
			catch (NoSuchElementException)
			{
				return false;
			}
		}
		public static bool IsElementPresent(By By, string Attribute, string Value)
		{
			try
			{
				foreach (var item in cd.FindElements(By))
				{
					if (item.GetAttribute(Attribute) == Value)
					{
						return true;
					}
				}
				return false;
			}
			catch (NoSuchElementException)
			{
				return false;
			}
		}
		public static bool IsOnline => IsElementPresent(By.Id("button_1"));
		public static bool IsUnregistered => (StatusBox() == "unregistered");
		//VOIDS
		public static HangUpResult HangUp()
		{
			if(Status() == FireRTCStatus.Calling)
			{
				SearchElement(By.ClassName("btn"), "data-action", "cancel").Click();
				return HangUpResult.Success;
			}
			else if (Status() == FireRTCStatus.Online)
			{
				SearchElement(By.ClassName("btn"), "data-action", "hangup").Click();
				return HangUpResult.Success;
			}
			return HangUpResult.NotInCall;
		}
		public static CallResult Call(string number)
		{
			UnRegisterCheck();
			try
			{
				cd.FindElementById("button_1"); //Check for the keypad number one. If it exists, we have connected to the phone.
			}
			catch { return CallResult.FailNotOnline; }
			number = Regex.Replace(number, "[^0-9]", "");
			Dialpad(number);
			var d = cd.FindElements(By.ClassName("btn"));
			foreach (IWebElement e in d)
			{
				if (e.GetAttribute("data-action") == "call")
				{
					e.Click();
					Thread.Sleep(2500);
					return CallResult.Calling;
				}
			}
			return CallResult.Fail;
		}
		public static void Dialpad(string number, bool delay = false)
		{
			if (IsElementPresent(By.ClassName("btn"), "data-action", "dialer"))
			{
				SearchElement(By.ClassName("btn"), "data-action", "dialer").Click();
			}
			Thread.Sleep(500);
			PressDialpadButton(number, delay);
			Thread.Sleep(500);
			if (IsElementPresent(By.ClassName("btn btn-media"), "data-action", "media"))
			{
				SearchElement(By.ClassName("btn btn-media"), "data-action", "media").Click();
			}
		}
		public static void ChangeNumber()
		{
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
			Program.CallerID = largeNumber;
			cd.FindElement(By.Id("address_ua_config_display_name")).Clear();
			cd.FindElement(By.Id("address_ua_config_display_name")).SendKeys(largeNumber);
			var c = cd.FindElements(By.ClassName("form-group"));
			foreach (IWebElement element in c)
			{
				element.Click();
			}
			cd.Url = "https://phone.firertc.com/phone";
		}
		public static void PressDialpadButton(string number, bool delay = false)
		{
			foreach (char ch in number)
			{
				cd.FindElement(By.Id("button_" + ch)).Click();
				if (delay)
				{
					Thread.Sleep(1000);
				}
			}
		}
		public static void Confirm()
		{
			try { cd.SwitchTo().Alert().Accept(); } catch {}
			cd.SwitchTo().DefaultContent();
		}
		public static void Reload()
		{
			Program.cd.Url = "about:blank";
			API.Confirm();
			Program.cd.Url = "https://phone.firertc.com/phone";
		}
		public static void UnRegisterCheck()
		{ if (IsUnregistered) Reload(); }
		//ENUMS
		public enum HangUpResult
		{
			Success,
			NotInCall,
		}
		public enum CallResult
		{
			Fail,
			FailNotOnline,
			Calling
		}
		public enum FireRTCStatus
		{
			Online,
			Calling,
			Talking,
			Unknown,
		}
		//WEB-ELEMENTS
		public static IWebElement SearchElement(By By)
		{
			try
			{
				return cd.FindElement(By);
			}
			catch (NoSuchElementException)
			{
				return null;
			}
		}
		public static IWebElement SearchElement(By By, string Attribute, string Value)
		{
			try
			{
				foreach (var item in cd.FindElements(By))
				{
					if (item.GetAttribute(Attribute) == Value)
					{
						return item;
					}
				}
				return null;
			}
			catch (NoSuchElementException)
			{
				return null;
			}
		}
	}
}
