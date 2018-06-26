using System;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

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
            sb.AppendLine($"Running  Version:\t{clsVersionControl.Version??"Not Found"} {clsVersionControl.VersionDateTime??""}");
            sb.AppendLine($"Latest   Version: {clsVersionControl.GetLatestTag()}");
            sb.AppendLine($"Previous Installed Version:\t{clsVersionControl.PreviousVersion??"Not Found"}");
            
            e.Channel.SendMessageAsync(sb.ToString());
            
            clsVersionControl.GetLatestTag();
            
        }
        
        public String HelpString
        {
            get
            {
                return "version";
            }
        }
    }
}
