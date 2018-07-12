using System;
using System.Text;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using TFABot.Git;

namespace TFABot.DiscordBot.Commands
{
    public class clsGitCommand : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsGitCommand()
        {
            MatchCommand = new []{"git"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("```");
            try
            {
                using (var git = new clsGit())
                {
                    sb.AppendLine(git.ToString());
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Git unavailabe {ex.Message}");
            }
            
            sb.Append("```");
            e.Channel.SendMessageAsync(sb.ToString());
        }
        
        public String HelpString
        {
            get
            {
                return "git\ngit\t<branch/commit>\tCheckout git";
            }
        }
    }
}
