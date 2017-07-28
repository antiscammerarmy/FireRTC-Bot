
using DSharpPlus;
using INIAddon;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FireRTCBot
{
    public class Events
    {
        private static Random r = new Random();
        public static async Task Ready(DiscordClient _c, ReadyEventArgs e)
        {
            Program.Log(Program.LogType.Normal, "Ready");
            await _c.UpdateStatusAsync(new Game() { Name = $"{Config.Trigger}help" }, UserStatus.Online);
        }
        public static async Task MessageCreated(DiscordClient _c, MessageCreateEventArgs e)
        {
            Program.Log(Program.LogType.Other, e.Message.Content, e.Message.Author.Username);
            try
            {
                if (!e.Message.Author.IsBot)
                {
                    #region Argument Processing
                    string cmd = "";
                    List<string> args = Regex.Split(e.Message.Content, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").ToList();
                    for (int i = 0; i < args.Count; i++)
                    {
                        if (args[i].StartsWith("\""))
                        {
                            args[i] = args[i].Replace("\"", string.Empty);
                        }
                    }

                    cmd = args[0].ToLower();
					
					args.RemoveAt(0);
                    #endregion
                    if (cmd.StartsWith(Config.Trigger) && Config.Trusted.Contains(e.Author.Id))
                    {
                        cmd = cmd.Replace(Config.Trigger, "");
						switch (cmd)
						{
							default: await e.Message.RespondAsync("unknown command.");  break;
							case "call":
								switch (Program.Call(args[0]))
								{
									case Program.CallResult.Fail:
										await e.Message.RespondAsync("Couldn't call {args[0]}, because of an unknown error.");
										break;
									case Program.CallResult.Calling:
										var embed = new DiscordEmbed()
										{
											Title = "Call",
											Footer = new DiscordEmbedFooter() { Text = "FireRTC", IconUrl = "https://pbs.twimg.com/profile_images/530090561030471680/pwyhK_GI_400x400.png" },
											Color = 0x00FF00,
											Author = new DiscordEmbedAuthor() { IconUrl = e.Author.AvatarUrl, Name = e.Author.Username },
											Description = $"**Status:** Calling\n**Number:** {args[0]}"
										};
										var msg = await e.Message.RespondAsync("", false, embed);
										while (true)
										{
											if (Program.cd.FindElementByClassName("status").Text.Contains("is online"))
											{
												embed.Description = $"**Status:** Hang Up\n**Number:** {args[0]}";
												embed.Color = 0xFF0000;
												await msg.EditAsync("", embed);
											}
										}
										break;
								} break;
							case "hangup": break;
							case "queue": break;
						}
					}
                }
            }
			catch (Exception ex)
			{
				Program.Crash(ex, "Command");
			}
        }
    }
}