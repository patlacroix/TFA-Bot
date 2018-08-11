using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsGoodAfternoon : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsGoodAfternoon()
        {
            MatchSubstring = new []{"afternoon"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            e.Channel.SendMessageAsync("Good Afternoon! :sun_with_face:");
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            return;
        }
    }
}
