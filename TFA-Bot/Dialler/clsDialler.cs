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
using System.Threading;

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
            
            var dialList = new List<Task>();
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
                        dialList.Add(Dialler.CallAsync(user.DiscordName,user.Tel,ChBotAlert));
                    else if (ChBotAlert!=null)
                       ChBotAlert.SendMessageAsync("name not found!");
                }
            }
            
            //We had some issues with calling too many numbers at once, so we throttle it to 5 at a time.
            if (dialList.Count>0) StartAndWaitAllThrottledAsync(dialList,5,61000);
        }
        
        static public void call(MessageCreateEventArgs e)
        {
            var toRing = e.Message.Content.ToLower();
            call(toRing,e.Channel);
        }
        
        public static async Task StartAndWaitAllThrottledAsync(IEnumerable<Task> tasksToRun, int maxTasksToRunInParallel, int timeoutInMilliseconds, CancellationToken cancellationToken = new CancellationToken())
        {
            // Convert to a list of tasks so that we don't enumerate over it multiple times needlessly.
            var tasks = tasksToRun.ToList();
         
            using (var throttler = new SemaphoreSlim(maxTasksToRunInParallel))
            {
                var postTaskTasks = new List<Task>();
         
                // Have each task notify the throttler when it completes so that it decrements the number of tasks currently running.
                tasks.ForEach(t => postTaskTasks.Add(t.ContinueWith(tsk => throttler.Release())));
         
                // Start running each task.
                foreach (var task in tasks)
                {
                    // Increment the number of tasks currently running and wait if too many are running.
                    await throttler.WaitAsync(timeoutInMilliseconds, cancellationToken);
         
                    cancellationToken.ThrowIfCancellationRequested();
                    task.Start();
                }
         
                // Wait for all of the provided tasks to complete.
                // We wait on the list of "post" tasks instead of the original tasks, otherwise there is a potential race condition where the throttler&#39;s using block is exited before some Tasks have had their "post" action completed, which references the throttler, resulting in an exception due to accessing a disposed object.
                await Task.WhenAll(postTaskTasks.ToArray());
            }
        }
        
        
      
    }
}
