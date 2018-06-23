using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using static TFABot.Program;

namespace TFABot.DiscordBot.Commands
{
    public class clsAlarm : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsAlarm()
        {
            MatchCommand = new []{"alarm"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
        
            var lower = e.Message.Content.ToLower();
        
            if (lower.Contains("off"))
            {
                Program.AlarmState = EnumAlarmState.Off;
            }
            else if (lower.Contains("on"))
            {
                Program.AlarmState = EnumAlarmState.On;
            }
            else if (lower.Contains("silent"))
            {
                Program.AlarmState = EnumAlarmState.On;
            }
            else if (lower.Contains("list"))
            {
                e.Channel.SendMessageAsync(Program.AlarmManager.ToString());
            }
            else
            {
                e.Channel.SendMessageAsync(clsCommands.Instance.GetHelpString(this));
            }
                
            e.Channel.SendMessageAsync($"Alarm State: {AlarmState.ToString()}");
        }
        
        public String HelpString
        {
            get
            {
                return 
@"alarm\tGet state.
alarm on\tActive.
alarm off\tNo Alarms.
alarm silent\tDiscord warnings only.
alarm list\tList active alarms.";

            }
        }
        
    }
}
