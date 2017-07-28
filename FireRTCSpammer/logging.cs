using DSharpPlus;
using System;

namespace FireRTCBot
{
    public partial class Program
    {
        public static void Log(LogType LogType, string Message, string Application = "")
        {
            ConsoleColor LogColor = ConsoleColor.Blue;
			ConsoleColor DLogColor = ConsoleColor.DarkBlue;
			Console.ForegroundColor = ConsoleColor.Black;
            switch (LogType)
            {
                case LogType.Normal:
                    LogColor = ConsoleColor.White;
					DLogColor = ConsoleColor.Gray;
					break;
                case LogType.Other:
                    LogColor = ConsoleColor.Gray;
					DLogColor = ConsoleColor.DarkGray;
					break;
                case LogType.Error:
                    LogColor = ConsoleColor.Red;
					DLogColor = ConsoleColor.DarkRed;
					break;
                case LogType.Warning:
                    LogColor = ConsoleColor.Yellow;
					DLogColor = ConsoleColor.DarkYellow;
					break;
                case LogType.Info:
                    LogColor = ConsoleColor.Cyan;
					DLogColor = ConsoleColor.DarkCyan;
					break;
            }
            Console.ForegroundColor = LogColor;
            Console.Write("{0}/ ", Application.ToString().PadLeft(20));
			Console.ForegroundColor = DLogColor;
			Console.WriteLine(Message);
        }
        public enum LogType : int
        {
            Other = 0,
            Normal = 1,
            Info = 2,
            Warning = 3,
            Error = 4,
        }
        public static void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
            switch (e.Level)
            {
                case LogLevel.Debug: Program.Log(Program.LogType.Normal, e.Message, e.Application); break;
                case LogLevel.Info: Program.Log(Program.LogType.Info, e.Message, e.Application); break;
                case LogLevel.Warning: Program.Log(Program.LogType.Warning, e.Message, e.Application); break;
                case LogLevel.Error: Program.Log(Program.LogType.Error, e.Message, e.Application); break;
                case LogLevel.Critical: Program.Log(Program.LogType.Error, e.Message, e.Application); break;
            }
        }
    }
}
