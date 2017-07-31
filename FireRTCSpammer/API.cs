using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireRTCSpammer
{
	public class API
	{
		private static ChromeDriver cd;
		public API(ChromeDriver cd)
		{
			API.cd = cd;
		}
		//BOOLEANS
		public static bool IsElementPresent(By by)
		{
			try
			{
				cd.FindElement(by);
				return true;
			}
			catch (NoSuchElementException)
			{
				return false;
			}
		}
		public static bool isOnline = IsElementPresent(By.Id("button_1"));
		//VOIDS
		public static HangUpResult HangUp()
		{
			foreach (IWebElement e2 in cd.FindElements(By.ClassName("btn")))
			{
				if (e2.GetAttribute("data-action") == "hangup")
				{
					e2.Click();
					return HangUpResult.Success;
				}
			}
			return HangUpResult.NotInCall;
		}
		//ENUMS
		public enum HangUpResult
		{
			Success,
			NotInCall,
		}
	}
}
