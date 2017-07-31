using INIAddon;
using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireRTCBot
{
	public static class Config
	{
		public static FileIniDataParser parser;
		private static IniData DataConfig ;
		public static string Trigger;
		public static ulong BotOwner;
		public static List<ulong> Trusted;
		public static string BotName;
		//TOKEN
        public static string Token_Discord;
		//OTHER
		public static bool DeleteCommands;
		public static bool SendReport;
		public static bool UnknownCommandResponse;


		public static void Set()
		{
			FileIniDataParser parser = new FileIniDataParser();
			IniData DataConfig = INI.RFile("config.ini", parser);
			Trigger = "☎";
			BotOwner = ulong.Parse(DataConfig["config"]["botowner"]);
			Trusted = new List<ulong>() { BotOwner };
			BotName = "FireRTCBot";
			//TOKEN
			Token_Discord = DataConfig["token"]["discord"];
			//OTHER
			DeleteCommands = bool.Parse(INI.RINI(DataConfig, "config", "deleteCmds", "true"));
			SendReport = bool.Parse(INI.RINI(DataConfig, "config", "sendReport", "true"));
			UnknownCommandResponse = bool.Parse(INI.RINI(DataConfig, "config", "unknownCmd", "true"));

		}
	}
}
