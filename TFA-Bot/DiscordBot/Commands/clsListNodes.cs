using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsListNodes : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsListNodes()
        {
            MatchCommand = new []{"nodes"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            e.Channel.SendMessageAsync($"```{TFABot.Program.GetNodes()}```");
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("nodes","","List nodes.");
        }
    }
}
