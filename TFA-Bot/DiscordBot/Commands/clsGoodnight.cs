using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsGoodnight : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsGoodnight()
        {
            MatchCommand = new [] {"night"};
            MatchSubstring = new []{"Night!","goodnight","good night"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            e.Channel.SendMessageAsync("Goodnight! :sleeping:");
        }

        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
                return;
        }
    }
}
