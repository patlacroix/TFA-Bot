using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DiscordBot;

namespace TFABot.Dialler
{
    public class clsDiallerSIP : IDialler
    {
    
        String Username;
        String Password;
        String Host;
        String CallingNumber;
    
        public clsDiallerSIP()
        {
            Username = Program.SettingsList["SIP-Username"];
            Password = Environment.GetEnvironmentVariable("SIP-PASSWORD") ?? Program.SettingsList["SIP-Password"];
            Host = Program.SettingsList["SIP-Host"];
            CallingNumber = Program.SettingsList["SIP-CallingNumber"];
        }
        
        
        public Task CallAsync(String Name, String Number, DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            try
            {
                if (ChBotAlert == null) ChBotAlert = clsBotClient.Instance.Our_BotAlert;

                String sipp = "/app/sipp/sipp";
                String timeout = "30s";
                String dialplanPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Data/dialplan.xml");

                String perms = $"{Host} -au {Username} -ap {Password} -l 1 -m 1 -sf {dialplanPath} -timeout {timeout} -s {Number.Replace(" ", "")}";

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

            }
            catch (Exception ex)
            {
                if (ChBotAlert!=null) ChBotAlert.SendMessageAsync($"Call error: {ex.Message}");
                Console.Write("Call error: " + ex.Message);
                tcs.SetResult(false);
            }
            return tcs.Task;
        }
    }
}
