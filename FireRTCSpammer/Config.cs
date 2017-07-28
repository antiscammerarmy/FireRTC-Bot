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
		public static FileIniDataParser parser = new FileIniDataParser();
		private static IniData DataConfig = INI.RFile("config.ini", parser);
		public static string Trigger = "☎";
		public static ulong BotOwner = ulong.Parse(DataConfig["config"]["botowner"]);
		public static List<ulong> Trusted = new List<ulong>() { BotOwner };
		public static string BotName = "FireRTCBot";
		//TOKEN
        public static string Token_Discord = DataConfig["token"]["discord"];
		//OTHER
		public static bool DeleteCommands = bool.Parse(INI.RINI(DataConfig, "config", "deleteCmds", "true"));
		public static bool SendReport = bool.Parse(INI.RINI(DataConfig, "config", "sendReport", "true"));
		public static bool UnknownCommandResponse = bool.Parse(INI.RINI(DataConfig, "config", "unknownCmd", "true"));
	}
}
