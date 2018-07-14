using System;
using System.Linq;
using System.Reflection;
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
        
            var msgSplit = e.Message.Content.Split(new char[]{' '});
        
            var sb = new StringBuilder();
            sb.Append("```");
            try
            {
                using (var git = new clsGit())
                {
                    if (msgSplit.Length>1) git.Switch(msgSplit[1]);
                    sb.AppendLine(git.ToString());
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error: {ex.Message}");
            }
            
            sb.Append("```");
            
            
            var GitCommit = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GitCommit), false).Cast<GitCommit>().First();
            Console.WriteLine($"Assembly {GitCommit.Hash}");
            
            if (msgSplit.Length>1) sb.AppendLine("\"bot update\" required to pull branch.");
            
            e.Channel.SendMessageAsync(sb.ToString());
        }
        
        public String HelpString
        {
            get
            {
                return "git\ngit <branch/commit>\tCheckout";
            }
        }
    }
}
