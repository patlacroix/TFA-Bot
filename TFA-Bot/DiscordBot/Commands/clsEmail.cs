using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsEmailCommand : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsEmailCommand()
        {
            MatchCommand = new []{"email"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            clsEmail.email(e);
        }
        
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("email","<user>");
        }
    }
}
