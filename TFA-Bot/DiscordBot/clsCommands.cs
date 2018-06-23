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
    
        public clsCommands()
        {
            Instance = this;
        }
        
        
        public void DiscordMessage(MessageCreateEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(e.Message.Content)) return;
                
                var lowMessage = e.Message.Content.ToLower();
                var firstword = lowMessage.Split(new []{' '},2,StringSplitOptions.RemoveEmptyEntries);            
                
                foreach (var command in MatchCommand.Where(x =>firstword[0].StartsWith(x.Item1)))
                {
                    command.Item2.Run(e);
                }
                
                foreach (var command in MatchSubstring.Where(x => lowMessage.Contains(x.Item1)))
                {
                    command.Item2.Run(e);
                }
                
                foreach (var command in MatchRegex.Where(x =>x.Item1.IsMatch(e.Message.Content)))
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


        public String GetHelpString(IBotCommand command)
        {
            var sb = new StringBuilder();
            sb.Append("```");
            HelpString(sb,command);
            sb.Append("```");
            return sb.ToString();
        }

        public String GetHelpString()
        {
            var sb = new StringBuilder();
            sb.Append("```");
            sb.AppendLine($"The Factoid Authority Bot                             {(DateTime.UtcNow-Program.AppStarted).ToDHMDisplay() }");
            sb.AppendLine("-----------------------------------------------------------------------");
            
            foreach (var command in MatchCommand.Where(x=>x.Item2.HelpString!=null).OrderBy(x=>x.Item1))
            {
                HelpString(sb,command.Item2);
            }
            sb.Append("```");
            return sb.ToString();
                
        }
        
        void HelpString(StringBuilder sb, IBotCommand command)
        {
                foreach (var lines in command.HelpString.Split(new []{'\n'}))
                {
                    var tabSplit = lines.Split(new string[]{@"\t"},2,StringSplitOptions.RemoveEmptyEntries);
                    sb.Append(tabSplit[0].PadRight(30));
                    if (tabSplit.Length>1) sb.Append(tabSplit[1]);
                    sb.AppendLine();
                }
        }
        
        
    }
}
