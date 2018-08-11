using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsFembot : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsFembot()
        {
            MatchSubstring = new []{"fembot"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            e.Channel.SendMessageAsync("*ANGRY BOT :rage: ");
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            return;
        }        
    }
}
