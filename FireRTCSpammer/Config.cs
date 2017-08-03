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
		private static IniData DataConfig;
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
			Trigger = "p!";
			BotOwner = ulong.Parse(DataConfig["config"]["botowner"]);
			Trusted = new List<ulong>()
			{
				BotOwner,
				325388797468737537, //R3DF0X39
				162865565378150410, //Deeveeaar
				310581249259339776, //Fyry
				128541913803390976, //Graut
				134048929208598528, //Lord Kill
				303208430963785728, //Mr.Awesome
				214989087541690369, //Colin
				202220288459538442, //awesomeness
				266480849065738241, //Angel
				206969269240594433, //Solor
				322841011917291520, //aidan5mithUK
				196682639606808576, //Daan
				150999292083961856, //Lars
				94618448146993152,  //Lucas
			};
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
