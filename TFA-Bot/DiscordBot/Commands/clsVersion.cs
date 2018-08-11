using System;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using TFABot.Git;

namespace TFABot.DiscordBot.Commands
{
    public class clsVersion : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsVersion()
        {
            MatchCommand = new []{"version"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("```");
            sb.AppendLine(clsGitHead.GetHeadToString());
            sb.Append("```");
            e.Channel.SendMessageAsync(sb.ToString());
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("version");
        }
    }
}
