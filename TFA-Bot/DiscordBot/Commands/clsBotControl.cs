using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;

namespace TFABot.DiscordBot.Commands
{
    public class clsBotControl : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsBotControl()
        {
            MatchCommand = new []{"bot"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            var tolower = e.Message.Content.ToLower();
            var commands = tolower.Split(new []{' '},StringSplitOptions.RemoveEmptyEntries);
            
            if (commands.Length>1)
            {
        
                switch (commands[1])
                {
                    case "reload":
                        e.Channel.SendMessageAsync(TFABot.Program.Spreadsheet.LoadSettings());
                        break;
                    case "restart":
                        Program.SetRunState(Program.enumRunState.Restart);
                        break;
                    case "update":
                        Program.SetRunState(Program.enumRunState.Update);
                        break;
                    case "exit":
                        Program.SetRunState(Program.enumRunState.Stop);
                        break;                     
                   case "previous":
                        Program.SetRunState(Program.enumRunState.PreviousVersion);
                        break;
                   default:
                        e.Channel.SendMessageAsync("unknown command");
                        break;
                }
            }
            else
            {
                e.Channel.SendMessageAsync(clsCommands.Instance.GetHelpString());
            }

        }
        public String HelpString
        {
            get
            {
                return 
@"bot         
bot reload\tReload spreadsheet (app settings require a restart).
bot update\tUpdate to latest bot version.
bot restart\tRestart bot.
bot previous\tSwitch back to previous verion (if available).
bot exit\tStop bot.";

            }
        }        
        
        
    }
}
