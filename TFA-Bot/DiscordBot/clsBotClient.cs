using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System.Threading;
using DSharpPlus.Net.WebSocket;
using System.Linq;
using System.Diagnostics;
using TFABot;
using static TFABot.Program;
using TFABot.DiscordBot;
using TFABot.Git;

namespace DiscordBot
{
    public class clsBotClient : IDisposable
    {
        private DiscordClient _client;
        private CancellationTokenSource _cts;
        private clsCommands Commands = new clsCommands();

        public DateTime TimeConnected {get; private set;}
        
        public DSharpPlus.Entities.DiscordChannel Our_BotAlert = null;
        public DSharpPlus.Entities.DiscordChannel Factom_BotAlert = null;
        
        StringBuilder TextBuffer = new StringBuilder();
        
        public static clsBotClient Instance = null;
        

        public clsBotClient(String Token)
        {
            Instance = this;
                    
            _client = new DiscordClient(new DiscordConfiguration()
            {
                AutoReconnect = true,
                EnableCompression = true,
                LogLevel = LogLevel.Debug,
                Token = Token,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true
            });


            _client.SetWebSocketClient<WebSocketSharpClient>();
            _client.Ready += OnReadyAsync;
            _client.GuildAvailable += this.Client_GuildAvailable;
            _client.ClientErrored += this.Client_ClientError;
            _client.MessageCreated += MessageCreateEvent;
          //  _client.mess
            
           
           Commands.LoadCommandClasses();
           
        }

        //Incoming Discord Message
        private async Task MessageCreateEvent(MessageCreateEventArgs e)
        {
        
            if (e.Author.IsBot) return;  //Reject our own messages
            
            if ( e.Channel == Factom_BotAlert)
            {
               Our_BotAlert.SendMessageAsync(e.Message.Content);
               return;
            }
            
            if (e.Channel.GuildId == 419201548372017163) return;  //Ignore Factom's Discord server
            
            Commands.DiscordMessage(e); //Forward message to commands lookup.
            Console.Write(e.Message);
        }


        public async Task RunAsync()
        {
            await _client.ConnectAsync();
            await WaitForCancellationAsync();
        }

        private async Task WaitForCancellationAsync()
        {
            while(!_cts.IsCancellationRequested)
                await Task.Delay(500);
        }

        private async Task OnReadyAsync(ReadyEventArgs e)
        {
            await Task.Yield();
            TimeConnected = DateTime.Now;
        }


        //Discord Server connected
        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {

            if (e.Guild.Id == 419201548372017163)  //Factom's Discord server
            {
                Factom_BotAlert = e.Guild.Channels.FirstOrDefault(x=>x.Id == 443025488655417364);
                Console.WriteLine($"Factom Alert channel: {Factom_BotAlert.Name}");
            }
            else
            {
                String alertChannelString;
                if (Program.SettingsList.TryGetValue("Discord-AlertsChannel",out alertChannelString))
                {
                    alertChannelString = alertChannelString.ToLower().Replace("#","");
                    var alertChannel = e.Guild.Channels.FirstOrDefault(x=>x.Name == alertChannelString);
                    if (alertChannel!=null)
                    {
                       Our_BotAlert = alertChannel;
                       Console.WriteLine($"Our Alert channel: {Our_BotAlert.Name}");
                       if (clsVersion.VersionChangeFlag)
                         Bot.Our_BotAlert.SendMessageAsync($":drum: Welcome to version {clsGitHead.GetHeadToString()} :trumpet:");
                       else
                         Bot.Our_BotAlert.SendMessageAsync("Hello :innocent:");
                         
                       if (TextBuffer.Length>0) Our_BotAlert.SendMessageAsync(TextBuffer.ToString()).ContinueWith((x)=>{TextBuffer.Clear();});
                    }
                }
                else
                {
                    Console.WriteLine("Warning: Discord-AlertsChannel not set");
                }
            }

            
            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        public void SendAlert(String text)
        {
            if (Our_BotAlert != null)
                Our_BotAlert.SendFileAsync(text);
            else
                TextBuffer.AppendLine(text);
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the details of the error that just 
            // occured in our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "ExampleBot", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);
          //  Console.WriteLine();
            
            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }   

        public void Dispose()
        {
            this._client.Dispose();
       
        }

        
    }
}
