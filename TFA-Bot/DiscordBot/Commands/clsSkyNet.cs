using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsSkynet : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsSkynet()
        {
            MatchCommand = new []{"skynet"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            e.Channel.SendMessageAsync("Skynet Activated");
        }
        
        public String HelpString
        {
            get
            {
                return null;
            }        
        }
    }
}
