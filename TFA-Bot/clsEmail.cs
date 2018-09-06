using System;
using System.Net;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using DSharpPlus.EventArgs;
using System.Linq;

namespace TFABot
{
    public class clsEmail
    {
        static String EmailFromAddress;
        static String SMTPHost;
        static int SMTPPort;
        static String SMTPUsername;
        static String SMTPPassword;

        public clsEmail()
        {
        }
        
        static public void GetSettings()
        {
            if (Program.SettingsList.TryGetValue("Email-SMTPHost", out SMTPHost))
            {
                Program.SettingsList.TryGetValue("Email-Username", out SMTPUsername);
                Program.SettingsList.TryGetValue("Email-Password", out SMTPPassword);
                Program.SettingsList.TryGetValue("Email-FromAddress", out EmailFromAddress);
                
                string port;
                if (Program.SettingsList.TryGetValue("Email-SMTPPort", out port))
                {
                    int.TryParse(port, out SMTPPort);
                }
            }
        }
        
        
        static void SendEmail(String To, String Subject, String Message,DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
           try
           {
                var message = new MimeMessage ();
                message.From.Add (new MailboxAddress (EmailFromAddress));
                message.To.Add (new MailboxAddress (To));
                message.Subject = Subject;
    
                message.Body = new TextPart ("plain") { Text = Message  };
    
                using (var client = new SmtpClient ()) {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s,c,h,e) => true;
    
                    client.Connect (SMTPHost, SMTPPort, false);
    
                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate (SMTPUsername, SMTPPassword);
                    client.Timeout = 2000;
                    client.Send (message);
                    client.Disconnect (true);
                    
                    if (ChBotAlert!=null)
                           ChBotAlert.SendMessageAsync($"Sent e-mail {To}");
                    
                }
            }
            catch (Exception ex)
            {
                if (ChBotAlert!=null)
                           ChBotAlert.SendMessageAsync($"Send e-mail error {ex.Message}");
            }
        }
        
        
        static public void EmailAlertList()
        {
            string alarmMessage = $"{Program.BotName} Alarm";
            foreach (var user in Program.UserList.Values.Where(x=>x.OnDuty))
            {
                SendEmail(user.email,alarmMessage,alarmMessage);
            }
        }
        
        
        static public void email(String names, DSharpPlus.Entities.DiscordChannel ChBotAlert = null)
        {
        
            string alarmMessage = $"{Program.BotName} Alarm (manual Discord request)";
        
            foreach (var nameItem in names.Split(new char []{' '}, StringSplitOptions.RemoveEmptyEntries))
            {
                var name = nameItem.ToLower();
                if (!name.EndsWith("mail")) 
                {
                    clsUser user;
                    if (!Program.UserList.TryGetValue(name,out user))
                    {
                      user = Program.UserList.Values.FirstOrDefault(x=>x.DiscordName.ToLower()==name || x.Name.ToLower()==name);
                    }
                    
                    if (user!=null) 
                        SendEmail(user.email,alarmMessage,alarmMessage,ChBotAlert);
                    else if (ChBotAlert!=null)
                       ChBotAlert.SendMessageAsync("name not found!");
                }
            }
            
        }
        
        static public void email(MessageCreateEventArgs e)
        {
            var toRing = e.Message.Content.ToLower();
            email(toRing,e.Channel);
        }
        
        
    }
}


