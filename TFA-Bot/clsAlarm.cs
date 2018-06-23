using System;
using System.Linq;
using DiscordBot;

namespace TFABot
{
    public class clsAlarm
    {
        DateTime Opened = DateTime.UtcNow;
        DateTime? TimeDiscord;
        DateTime? TimeCall;
        DateTime? Timeout;
        
        public clsNotificationPolicy notificationPolicy;
        
        public enumAlarmType AlarmType {get; private set;}
        public enum enumAlarmType
        {
            Error,
            NoResponse,
            Height,
            Latency,
            Network
        };
        
        public clsNode Node {get; private set;}
        public clsNetwork Network {get; private set;}
        public String Message;
               
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
        
        //New alarm from Network
        public clsAlarm(enumAlarmType alarmType, String message, clsNetwork network)
        {
            AlarmType = alarmType;
            Message = message;
            Network = network;
                       
           Program.NotificationPolicyList.TryGetValue(network.StallNotification,out notificationPolicy);
        }
        

        ////New alarm 
        //public clsAlarm(enumAlarmType alarmType, String message)
        //{
            
        //}
        
        public void Clear(string message = null)
        {
            
            if (TimeDiscord.HasValue)
            {
                if (String.IsNullOrEmpty(message)) message = $"{AlarmType} alarm cleared";
                clsBotClient.Instance.Our_BotAlert.SendMessageAsync(message);
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
                        clsCaller.CallAlertList();
                    }
                }
    
                if (Program.AlarmState != Program.EnumAlarmState.Off)
                {
                    if (!TimeDiscord.HasValue && notificationPolicy.Discord>=0 && DateTime.UtcNow > Opened.AddSeconds(notificationPolicy.Discord))
                    {
                        TimeDiscord = DateTime.UtcNow;
                        clsBotClient.Instance.Our_BotAlert.SendMessageAsync(Message);
                    }
                }
            }
            else
            {
                if (!TimeDiscord.HasValue)
                {
                    Program.Bot.Our_BotAlert.SendMessageAsync(Message);
                    TimeDiscord = DateTime.UtcNow;
                }
            }
        }
        
        public new String ToString()
        {
            return $"{AlarmType.ToString().PadRight(15)} {Node?.Name.PadRight(15) ?? Network?.Name.PadRight(20) ?? ""} {Opened} {Message}";
        }
        
    }
}
