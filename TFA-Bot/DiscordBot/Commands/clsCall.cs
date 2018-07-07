using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsCall : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsCall()
        {
            MatchCommand = new []{"call"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            clsDialler.call(e);
        }
        
        public String HelpString
        {
            get
            {
                return @"call <user>";
            }        
        }
    }
}
