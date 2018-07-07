﻿using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using DSharpPlus.EventArgs;
using System.Linq;

namespace TFABot.DiscordBot.Commands
{
    public class clsMTR : IBotCommand
    {
    
        public String[] MatchCommand {get; private set;}
        public String[] MatchSubstring {get; private set;}
        public Regex[] MatchRegex {get; private set;}
        
        public clsMTR()
        {
            MatchCommand = new []{"mtr"};
        }
        
        public void Run(MessageCreateEventArgs e)
        {
            try
            {
                var args = e.Message.Content.ToLower().Split(new String []{" ",","},StringSplitOptions.RemoveEmptyEntries);
               
                foreach (var arg in args.Skip(1))
                {
                    String host = null;
                    
                    var node=Program.NodesList.Values.FirstOrDefault(x => x.Host.ToLower() == arg || x.Name.ToLower() == arg);
                    if (node!=null)
                    {
                        host = node.Host;
                    }
                    else
                    {
                        System.Net.IPAddress iPAddress;
                        if (System.Net.IPAddress.TryParse(arg,out iPAddress))
                        {
                            host = iPAddress.ToString();
                        }
                    }
                    
                    if (host==null) continue;
                    
                
                    var process = new Process
                    {
                        StartInfo = { FileName = "/usr/bin/mtr",
                                      Arguments = $"-rw {host}",
                                      UseShellExecute = false,
                                      RedirectStandardOutput = true,
                                      // RedirectStandardError = true
                                    },
                            EnableRaisingEvents = true
        
                    };
        
                    process.Exited += (sender, eargs) =>
                    {
                        e.Channel.SendMessageAsync($"```{process.StandardOutput.ReadToEnd()}```");
                        process.Dispose();
                    };
        
                    process.Start();
                    e.Channel.SendMessageAsync($"Running mtr on {host}");
                }
            }
            catch (Exception ex)
            {
                e.Channel.SendMessageAsync("Error " + ex.Message);
            }
        }
        
        public String HelpString
        {
            get
            {
                return @"mtr <ip/host>\tRun mtr test.";
            }
        }        
    }
}