using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    
    public interface IBotCommand
    {
        String[] MatchCommand {get;}
        String[] MatchSubstring {get;}
        Regex[] MatchRegex {get;}

    
        void Run(MessageCreateEventArgs e);
        
        void HelpString (ref clsColumnDisplay columnDisplay);
        
    }
}
