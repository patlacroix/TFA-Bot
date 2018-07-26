using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DiscordBot;
using DSharpPlus.EventArgs;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using RestSharp.Authenticators;
using TFABot.Dialler;

namespace TFABot
{
    static public class clsDialler
    {
        static IDialler Dialler = null;
        
        static clsDialler()
        {
                GetSetings();
        }
        
        static public void GetSetings()
        {
            var host = Program.SettingsList["SIP-Host"];
            
            if (!String.IsNullOrEmpty(host))
            {
                if (host.Contains("twilio"))
                    Dialler = new clsDiallerTwilio();
                else
                    Dialler = new clsDiallerSIP();
            }
        }
        
        static public void CallAlertList()
        {
        
            foreach (var user in Program.UserList.Values.Where(x=>x.OnDuty))
            {
                call(user.DiscordName);
            }
        }
        
        
        static public void call(String names, DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
            if (Dialler==null)
            {
                GetSetings();
                if (Dialler==null)
                {
                    if (ChBotAlert == null) ChBotAlert = clsBotClient.Instance.Our_BotAlert;
                    ChBotAlert.SendMessageAsync("SIP not set up");
                    return;
                }
            }
            
            
            foreach (var nameItem in names.Split(new char []{' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                var name = nameItem.ToLower();
                if (!name.EndsWith("all"))
                {
                    clsUser user;
                    if (!Program.UserList.TryGetValue(name,out user))
                    {
                      user = Program.UserList.Values.FirstOrDefault(x=>x.DiscordName.ToLower()==name || x.Name.ToLower()==name);
                    }
                    
                    if (user!=null) 
                        Dialler.CallAsync(user.DiscordName,user.Tel,ChBotAlert);
                    else if (ChBotAlert!=null)
                       ChBotAlert.SendMessageAsync("name not found!");
                }
            }
            
        }
        
        static public void call(MessageCreateEventArgs e)
        {
            var toRing = e.Message.Content.ToLower();
            call(toRing,e.Channel);
        }
      
    }
}
