using System;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsHelp : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsHelp()
        {
            MatchCommand = new []{"help"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
              e.Channel.SendMessageAsync(clsCommands.Instance.GetHelpString());
        }
        public String HelpString
        {
            get
            {
                return @"help";
            }
        }        
        
    }
}
