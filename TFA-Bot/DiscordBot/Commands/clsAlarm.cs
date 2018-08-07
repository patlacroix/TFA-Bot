using System;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using static TFABot.Program;

namespace TFABot.DiscordBot.Commands
{
    public class clsAlarm : IBotCommand
    {
        Regex regex_timeout = new Regex(@"(?<=\s)\d*[mhs]");
        
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
            
            TimeSpan? timeout = null;
            var regmatch = regex_timeout.Match(lower);
            if (regmatch.Success)
            {
                var val = int.Parse(regmatch.Value.Substring(0,regmatch.Value.Length-2));
                if (regmatch.Value.EndsWith("h")) timeout= new TimeSpan(val,0,0);
                else if (regmatch.Value.EndsWith("m")) timeout= new TimeSpan(0,val,0);
                else if (regmatch.Value.EndsWith("s")) timeout= new TimeSpan(0,0,val);
            }
            
                    
            if (lower.Contains("off"))
            {
                Program.SetAlarmState(EnumAlarmState.Off,timeout);
            }
            else if (lower.Contains("on"))
            {
                Program.SetAlarmState(EnumAlarmState.On,timeout);
            }
            else if (lower.Contains("silent"))
            {
                Program.SetAlarmState(EnumAlarmState.Silent,timeout);
            }
            else if (lower.Contains("list"))
            {
                e.Channel.SendMessageAsync(Program.AlarmManager.ToString());
            }
            else
            {
                e.Channel.SendMessageAsync(clsCommands.Instance.GetHelpString(this));
            }
                
            if (Program.AlarmStateTimeout.HasValue)
                e.Channel.SendMessageAsync($"Alarm State: {AlarmState.ToString()} Resets in: {(DateTime.UtcNow - Program.AlarmStateTimeout.Value):hh:mm:ss}");
            else
                e.Channel.SendMessageAsync($"Alarm State: {AlarmState.ToString()}");
        }
        
        public String HelpString
        {
            get
            {
                return 
@"alarm\tGet state.
alarm on [<int><h,m,s>]\tActive.
alarm off\tNo Alarms.
alarm silent [<int><h,m,s>]\tDiscord warnings only.
alarm list\tList active alarms.";

            }
        }
        
    }
}
