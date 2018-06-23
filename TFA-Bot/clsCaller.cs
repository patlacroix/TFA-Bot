﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DiscordBot;
using DSharpPlus.EventArgs;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace TFABot
{
    static public class clsCaller
    {
        
        static clsCaller()
        {
                
        }
        
        static public void CallAlertList()
        {
            foreach (var user in Program.UserList.Values.Where(x=>x.OnDuty))
            {
                call(user.DiscordName);
            }           
        }
        
        
        static public void call(String name, DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
            name = name.ToLower();
            
            clsUser user;
            if (!Program.UserList.TryGetValue(name,out user))
            {
              user = Program.UserList.Values.FirstOrDefault(x=>x.DiscordName==name);
            }
            
            if (user!=null) call(user.DiscordName,user.Tel,ChBotAlert);
            
        }
        
        static public void call(MessageCreateEventArgs e)
        {
            var toRing = e.Message.Content.ToLower();
            call (toRing,e.Channel);
        }

        static public Task call(String Name, String Number, DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {

			try
			{

				if (ChBotAlert == null) ChBotAlert = clsBotClient.Instance.Our_BotAlert;

				String sipp = "/app/sipp/sipp";
				String username = Program.SettingsList["SIP-Username"];
                String password = Environment.GetEnvironmentVariable("SIP-PASSWORD") ?? Program.SettingsList["SIP-Password"];
                
				String host = Program.SettingsList["SIP-Host"];
				String timeout = "30s";
				String dialplanPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"dialplan.xml");

				String perms = $"{host} -au {username} -ap {password} -l 1 -m 1 -sf {dialplanPath} -timeout {timeout} -s {Number.Replace(" ", "")}";
				//sipp voiceless.aa.net.uk -au +443333402005 -ap tinydancer -l 1 -m 1 -sf /root/dialplan.xml -timeout 20s -s "+447973665024"
				var tcs = new TaskCompletionSource<bool>();

				var process = new Process
				{
					StartInfo = { FileName = sipp,
							  Arguments = perms,
							  UseShellExecute = false
                           //   RedirectStandardOutput = true,
                            //  RedirectStandardError = true
                             },
					EnableRaisingEvents = true

				};

				process.Exited += (sender, args) =>
				{
					ChBotAlert.SendMessageAsync($"{Name} Call Ended");
					tcs.SetResult(true);
					process.Dispose();
				};

				ChBotAlert.SendMessageAsync($"Calling {Name} {Number}");
				process.Start();
				//while (!process.StandardError.EndOfStream) {
				//       e.Channel.SendMessageAsync(process.StandardError.ReadLine());    
				//}

				//while (!process.StandardOutput.EndOfStream) {
				//       e.Channel.SendMessageAsync(process.StandardOutput.ReadLine());    
				//}

                
				return tcs.Task;
			}
			catch (Exception ex)
			{
				Console.Write("Call error: " + ex.Message);
				return null;
			}
        }
      
    }
}
