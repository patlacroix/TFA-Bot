using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot;
using DSharpPlus.Entities;

namespace TFABot
{
    public class clsAlarm
    {
        DateTime Opened = DateTime.UtcNow;
        DateTime? TimeDiscord;
        DateTime? TimeCall;
        DateTime? Timeout;
        DateTime? DelayUntil;
        public clsNotificationPolicy notificationPolicy;
        
        public enumAlarmType AlarmType {get; private set;}
        public enum enumAlarmType
        {
            Error,
            Syncing,
            NoResponse,
            Height,
            Latency,
            Network
        };
        
        public clsNode Node {get; private set;}
        public clsNetwork Network {get; private set;}
        public String Message;
        
        List<string> Notes = new List<string>();
       
        //New alarm, from node
        public clsAlarm(enumAlarmType alarmType, String message, clsNode node)
        {
            AlarmType = alarmType;
            Message = message;
            Node = node;
                    
            switch(alarmType)
            {
                case enumAlarmType.NoResponse:
                    Program.NotificationPolicyList.TryGetValue(Node.NodeGroup.Ping,out notificationPolicy); break;
                case enumAlarmType.Height:
                    Program.NotificationPolicyList.TryGetValue(Node.NodeGroup.Height,out notificationPolicy); break;
                case enumAlarmType.Latency:
                    Program.NotificationPolicyList.TryGetValue(Node.NodeGroup.Latency,out notificationPolicy); break;
//                case enumAlarmType.Error:
                case enumAlarmType.Network:
                    Program.NotificationPolicyList.TryGetValue(Node.NodeGroup.NetworkString,out notificationPolicy); break;
            }
        }
        
        //New alarm
        public clsAlarm(enumAlarmType alarmType, String message, TimeSpan delay)
        {
                AlarmType = alarmType;
                Message = message;
                DelayUntil = DateTime.UtcNow.Add(delay);
        }
        
        
        //New alarm from Network
        public clsAlarm(enumAlarmType alarmType, String message, clsNetwork network)
        {
            AlarmType = alarmType;
            Message = message;
            Network = network;
                       
           Program.NotificationPolicyList.TryGetValue(network.StallNotification,out notificationPolicy);
        }
        
        public void Clear(string message = null)
        {
            if (TimeDiscord.HasValue)
            {
                var sb = new StringBuilder();
                if (String.IsNullOrEmpty(message))
                    sb.AppendLine($"{AlarmType} alarm cleared");
                else
                    sb.AppendLine(message);
                    
                foreach( var line in Notes)
                {
                    sb.AppendLine(line);
                }
                Notes.Clear();
                
                clsBotClient.Instance.Our_BotAlert.SendMessageAsync(sb.ToString());
            }
        }
        
        public void Process()
        {
            if (notificationPolicy!=null)
            {            
                if (Program.AlarmState == Program.EnumAlarmState.On)
                {
                    if (!TimeCall.HasValue && notificationPolicy.Call>=0 && DateTime.UtcNow > Opened.AddSeconds(notificationPolicy.Call))
                    {
                        TimeCall = DateTime.UtcNow;
                        clsDialler.CallAlertList();
                    }
                }
    
                if (Program.AlarmState != Program.EnumAlarmState.Off)
                {
                    if (!TimeDiscord.HasValue && notificationPolicy.Discord>=0 && DateTime.UtcNow > Opened.AddSeconds(notificationPolicy.Discord))
                    {
                        TimeDiscord = DateTime.UtcNow;
                        var sb = new StringBuilder();
                        sb.AppendLine(Message);
                        foreach( var line in Notes)
                        {
                            sb.AppendLine(line);
                        }
                        Notes.Clear();
                        clsBotClient.Instance.Our_BotAlert.SendMessageAsync(sb.ToString());
                    }
                }
            }
            else
            {
                if (!TimeDiscord.HasValue)
                {
                    if (!DelayUntil.HasValue || DelayUntil.Value < DateTime.UtcNow)
                    {
                        Program.Bot.Our_BotAlert.SendMessageAsync(Message);
                        TimeDiscord = DateTime.UtcNow;
                    }
                }
            }
        }
        
        public new String ToString()
        {
            return $"{AlarmType.ToString().PadRight(15)} {Node?.Name.PadRight(15) ?? Network?.Name.PadRight(20) ?? ""} {Opened} {Message}";
        }

        internal void AddNote(String text)
        {        
            if (TimeDiscord.HasValue)
            {
                clsBotClient.Instance.Our_BotAlert.SendMessageAsync(text);
            }
            else
            {
                Notes.Add(text);
            }
        }
    }
}
