using System;
using System.Collections.Generic;
using TFABot.DiscordBot.Commands;
using System.Linq;
using DSharpPlus.EventArgs;
using System.Text.RegularExpressions;
using System.Text;

namespace TFABot.DiscordBot
{
    public class clsCommands
    {
        List <(string,IBotCommand)> MatchCommand = new List<(string, IBotCommand)>();
        List <(string,IBotCommand)> MatchSubstring = new List<(string, IBotCommand)>();
        List <(Regex,IBotCommand)> MatchRegex = new List<(Regex, IBotCommand)>();
    
        public static clsCommands Instance;
        public static String BotCommandPrefix {get; set;}

    
        public clsCommands()
        {
            Instance = this;
            string prefix;
            if (Program.SettingsList.TryGetValue("BotCommandPrefix", out prefix))
            {
                 BotCommandPrefix = prefix;
            }
        }
        
        
        public void DiscordMessage(MessageCreateEventArgs e)
        {
        
            String Message;
            try
            {
                if (String.IsNullOrEmpty(e.Message.Content)) return;
                
                if (String.IsNullOrEmpty(BotCommandPrefix))
                {
                    Message = e.Message.Content;
                }
                else if (e.Message.Content.StartsWith(BotCommandPrefix) &&
                        (e.Message.Content.Length > BotCommandPrefix.Length))
                {
                    Message = e.Message.Content.Substring(BotCommandPrefix.Length);
                }
                else
                {
                    return;
                }
        
                
                var lowMessage = Message.ToLower();
                var firstword = lowMessage.Split(new []{' '},2,StringSplitOptions.RemoveEmptyEntries);            
                
                foreach (var command in MatchCommand.Where(x =>firstword[0] == x.Item1))
                {
                    command.Item2.Run(e);
                }
                
                foreach (var command in MatchSubstring.Where(x => lowMessage.Contains(x.Item1)))
                {
                    command.Item2.Run(e);
                }
                
                foreach (var command in MatchRegex.Where(x =>x.Item1.IsMatch(Message)))
                {
                    command.Item2.Run(e);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"DiscordMessage Error: {ex.Message}");
            }
        }
        
        
        public void LoadCommandClasses()
        {
            var type = typeof(IBotCommand);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p)&& !p.IsInterface);
                
            foreach (var commandType in types)
            {
               IBotCommand command = (IBotCommand)Activator.CreateInstance(commandType);
               
               if (command.MatchCommand!=null)
                    foreach (var match in command.MatchCommand) { MatchCommand.Add((match,command)); }
               if (command.MatchSubstring!=null)
                    foreach (var match in command.MatchSubstring) { MatchSubstring.Add((match,command)); }
               if (command.MatchRegex != null)     
                    foreach (var match in command.MatchRegex) { MatchRegex.Add((match,command)); }

            }    
        }


        public String GetHelpString(IBotCommand command = null)
        {
            var cd = new clsColumnDisplay();
            cd.Append("```");
            cd.ColumnChar=' ';
            cd.AppendLine($"The Factoid Authority Bot                          Uptime {(DateTime.UtcNow-Program.AppStarted).ToDHMDisplay() }"); 
            cd.AppendCol("Command");
            cd.AppendCol("Args");
            cd.AppendCol("Description");
             
            cd.AppendCharLine('-');
            
            if (command!=null)
            {
                command.HelpString(ref cd);
            }
            else
            {
                foreach (var commandItem in MatchCommand.OrderBy(x=>x.Item1))
                {
                    commandItem.Item2.HelpString(ref cd);
                    cd.NewLine();
                }
            }
            cd.Append("```");
            cd.Append(Program.BotURL);
            return cd.ToString();
        }
    }
}
