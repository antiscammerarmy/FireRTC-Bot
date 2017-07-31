
using DSharpPlus;
using INIAddon;
using IniParser.Model;
using OpenQA.Selenium;
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
                    if (cmd.StartsWith(Config.Trigger))
                    {
						if (!Config.Trusted.Contains(e.Author.Id))
						{
							await e.Message.RespondAsync($"⚠ {e.Message.Author.Mention} you aren't trusted. Ask Craftplacer or another trusted user to add you.");
							return;
						}
                        cmd = cmd.Replace(Config.Trigger, "");
						switch (cmd)
						{
							default: await e.Message.RespondAsync("unknown command.");  break;
							case "call":

								string number = Regex.Replace(args[0], "[^0-9]", "");
								if (Program.CurrentCall != null)
								{
									await e.Message.RespondAsync("");
									Program.CallQueue.Add(new Program.CallObject()
									{
										e = e,
										embed = new DiscordEmbed()
										{
											Title = "Call",
											Footer = new DiscordEmbedFooter() { Text = $"Called by {e.Message.Author.Username}" },
											Color = 0xffd85b,
											Author = new DiscordEmbedAuthor() { Name = "FireRTC", IconUrl = "https://pbs.twimg.com/profile_images/530090561030471680/pwyhK_GI_400x400.png" },
											Description = $"**Status:** Calling\n**Number:** {args[0]}"
										},
										number = number,
										msg = await e.Message.RespondAsync("", false, new DiscordEmbed() { Title = "⌛ Another Call is in progress, the number will be called directly after the slot is free." }),
										_start = DateTime.Now,
									});
									return;
								}
								switch (Program.Call(number))
								{
									case Program.CallResult.Fail:
										await e.Message.RespondAsync("Couldn't call {args[0]}, because of an unknown error.");
										break;
									case Program.CallResult.FailNotOnline:
										await e.Message.RespondAsync("The FireRTC is not online.");
										break;
									case Program.CallResult.Calling:
										await Task.Delay(5000);
										Program.CurrentCall = new Program.CallObject()
										{
											e = e,
											embed = new DiscordEmbed()
											{
												Title = "Call",
												Footer = new DiscordEmbedFooter() { Text = $"Called by {e.Message.Author.Username}" },
												Color = 0xffd85b,
												Author = new DiscordEmbedAuthor() { Name = "FireRTC", IconUrl = "https://pbs.twimg.com/profile_images/530090561030471680/pwyhK_GI_400x400.png" },
												Description = $"**Status:** Calling\n**Number:** {args[0]}"
											},
											number = number,
											msg = await e.Message.RespondAsync("", false, new DiscordEmbed() { Title = "Please wait..." }),
											_start = DateTime.Now,
										};
										break;
								} break;
							case "hangup":
								switch (AppDomainInitializer:)
								{
									default:
 break;
								}
								break;
							case "queue":
								string list = "";
								foreach (var item in Program.CallQueue)
								{
									list += "+" + item.number + "\n";
								}
								await e.Message.RespondAsync("",false,new DiscordEmbed() { Title = "Queue", Description = list });
								break;
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