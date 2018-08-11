using System;
using System.IO;
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
                   case "debug":
                        var reg = new Regex(@".*:\d{1,5}").Match(commands[2]);
                        if (reg.Success)
                        {
                            var filename = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),"mono_args.txt");
                            File.WriteAllText(filename, $"--debug --debugger-agent=transport=dt_socket,address={reg.Value},server=y,suspend=y");
                            Program.SetRunState(Program.enumRunState.MonoArgs);
                        }
                        else
                            e.Channel.SendMessageAsync("incorrect host:port");
                        
                        
                        break;
                   default:
                        e.Channel.SendMessageAsync("unknown 'bot' command.");
                        break;
                }
            }
            else
            {
                e.Channel.SendMessageAsync(clsCommands.Instance.GetHelpString());
            }

        }
        public void HelpString (ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol("bot reload","","Reload spreadsheet (app settings require a restart).");
            columnDisplay.AppendCol("bot update","","Update to latest bot version (if available).");
            columnDisplay.AppendCol("bot restart","","Restart bot.");
            columnDisplay.AppendCol("bot previous","","Switch back to previous version (if available).");
            columnDisplay.AppendCol("bot debug","<host:port>","Debug on remote IDE.");
            columnDisplay.AppendCol("bot exit","","Stop bot (ends docker instance).");
        }
        
    }
}
