
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
							default: await e.Message.RespondAsync("❓ Unknown Command.");  break;
							//BASIC CALL FUNCTIONS
							#region call
							case "call":
								foreach (var item in args)
								{
									Program.CallObject co = new Program.CallObject()
									{
										e = e,
										embed = new DiscordEmbed()
										{
											Footer = new DiscordEmbedFooter() { Text = $"Called by {e.Message.Author.Username}" },
											Author = new DiscordEmbedAuthor() { Name = "FireRTC", IconUrl = "https://pbs.twimg.com/profile_images/530090561030471680/pwyhK_GI_400x400.png" },
											Description = $"..."
										},
										number = Regex.Replace(item, "[^0-9]", ""),
										chnl = e.Channel,
									};
									Program.CallQueue.Add(co);
								}
								await e.Message.RespondAsync($"✅ **``{args.Count}``** number(s) added to the queue.");
								break;
							#endregion
							#region hangup
							case "hangup":
								switch (API.HangUp())
								{
									case API.HangUpResult.Success:
										await e.Message.RespondAsync("Hanging up current call...");
										break;
									case API.HangUpResult.NotInCall:
										await e.Message.RespondAsync("⚠ The Bot isn't calling someone.");
										break;
								}
								break;
							#endregion
							#region dialpad
							case "dialpad":
								API.Dialpad(args[0], true);
								break;
							#endregion
							#region reload
							case "reload":
								if (e.Author.Id == Config.BotOwner)
								{
									API.Reload();
								}
								else
								{
									await e.Message.RespondAsync("You aren't the Owner of this bot.");
								}
								break;
							#endregion
							#region changeid
							case "changeid":
								var msg = await e.Message.RespondAsync("⌛ Wait...");
								API.ChangeNumber();
								await msg.EditAsync($"✅ Changed Caller ID to ``{Program.CallerID}``!");
								break;
							#endregion
							//QUEUE
							#region queue
							case "queue":
								if (Program.CallQueue.Count != 0)
								{
									string list = "";
									foreach (var item in Program.CallQueue)
									{
										list += "+" + item.number + "\n";
									}
									await e.Message.RespondAsync("", false, new DiscordEmbed() { Title = "Queue", Description = list });
								}
								else
								{
									await e.Message.RespondAsync("The Queue is empty.");
								}
								break;
							#endregion
							#region clearqueue
							case "clearqueue":
								Program.CallQueue.Clear();
								await e.Message.RespondAsync("✅ Cleared all numbers in the queue.");
								break;
							#endregion
							#region history
							case "history":
								DiscordEmbed result = new DiscordEmbed()
								{
									Title = $"History ({Program.CallHistory})",
									Fields = new List<DiscordEmbedField>()
								};
								foreach (var item in Program.CallHistory)
								{
									result.Fields.Add(new DiscordEmbedField()
									{
										Name = $"{item.number}",
										Value = $@"**Called by** {item.user}
**Total Time** {item.time.Minutes.ToString().PadLeft(2, '0')}:{item.time.Seconds.ToString().PadLeft(2, '0')}
**Channel** {item.chnl}"
									});
								}
								await e.Message.RespondAsync("", false, result);
								break;
							#endregion
							//BOT-RELATED
							#region stop
							case "stop":
								if (e.Author.Id == Config.BotOwner)
								{
									Program.Shutdown();
								}
								else
								{
									await e.Message.RespondAsync("You aren't the Owner of this bot.");
								}
								break;
							#endregion
							//EXTRA
							#region sound
							case "sound":
								switch (args[0])
								{
									case "LIST":
										string soundlist = "";
										foreach (var item in Program.Soundlist)
										{
											soundlist += item.Key + "\n";
										}
										await e.Message.RespondAsync(soundlist);
										break;
									case "STOP":
										foreach (var item in Program.Soundlist)
										{
											item.Value.Stop();
										}
										break;
									default:
										if (Program.Soundlist.ContainsKey(args[0].ToLower()))
										{
											Program.Soundlist[args[0].ToLower()].Play();
										}
										else
										{
											await e.Message.RespondAsync("Sound not found.");
										}
										break;
								}
								break;
							#endregion
							#region help
							case "help":
								await e.Message.RespondAsync(@"```http
p!call <number> [number1] : Call somebody
p!hangup                  : Hangup the current call.
p!dialpad <numbers>       : Press numbers on the dialpad.
p!changeid                : Change the Caller ID.
--------------------------+------------------------------------
p!queue                   : See what number(s) get called next.
p!clearqueue			  : Too Many Numbers?
p!history                 : Hey Guys what number did we call?
--------------------------+------------------------------------
p!sound <sound|LIST|STOP> : *windows xp startup*
--------------------------+------------------------------------
p!reload       [DEV-ONLY] : Reload the page / FireRTC
p!stop         [DEV-ONLY] : Stop the bot.
```");  
								break;
							#endregion
						}
					}
                }
            }
			catch (Exception ex)
			{
				Program.Crash(ex, "Command");
				await e.Message.RespondAsync("Error occured. (See error\\ for more details)");
			}
        }
    }
}