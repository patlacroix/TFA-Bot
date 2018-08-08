using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using RestSharp;

namespace TFABot
{
    public class clsNode : ISpreadsheet<clsNode>
    {
    
        clsRollingAverage LatencyList = new clsRollingAverage(10);
        clsRollingAverage PacketLoss = new clsRollingAverage(100);
        
        public clsNode()
        {
        }
        
        public clsNodeGroup NodeGroup;
        
        public uint LatencyLowest { get; private set;}
        
        [ASheetColumnHeader(true,"name")]
        public string Name {get;set;}
        [ASheetColumnHeader("group")]
        public string Group {get;set;}
        [ASheetColumnHeader("host")]
        public string Host {get;set;}
        [ASheetColumnHeader("monitor")]
        public bool Monitor {get;set;}
        
        public String ErrorMsg {get; private set;}
        public String NodeVersion {get; private set;}
        
        public int Latency
        {
          get{ return LatencyList.CurrentAverage; }
        }
        
        public DateTime? LastLeaderHeight { get; private set;}
        uint _leaderHeight = 0;
        public uint LeaderHeight
         { 
            get
            {
                return _leaderHeight;
            }
            private set
            {
                if (_leaderHeight != value)
                {
                    _leaderHeight = value;
                    
                    if (LastLeaderHeight.HasValue)
                    {
                        if (_leaderHeight < NodeGroup.Network.TopHeight &&  //We are behind the network.
                          (DateTime.UtcNow - LastLeaderHeight.Value).TotalMinutes>1 )  //Been offline for > 1 minute?
                          {
                            SyncMode = true;   
                            GetVersionAsync();  //Get version no if leader now known.
                          }
                    }
                    else
                    {
                        GetVersionAsync();  //Get version no if leader now known.
                    }
                    LastLeaderHeight = DateTime.UtcNow;
                }
            }
        }

        clsAlarm AlarmSyncing = null;
                
        clsAlarm AlarmHeightLow = null;
        uint _heightLowCount;
        public uint HeightLowCount
        {
           get
           {
             return _heightLowCount;
           }
           set
           {
             _heightLowCount = value;
             if (AlarmSyncing!=null && _heightLowCount==0)  SyncMode=false;  //Reset sync mode if active
             if (!SyncMode)
             {
                 if (_heightLowCount==0 && AlarmHeightLow!=null)
                 {
                    Program.AlarmManager.Clear(AlarmHeightLow,$"CLEARED: {Name} height low.");
                    AlarmHeightLow = null;
                 }
                 else if (_heightLowCount > 3 && _requestFailCount ==0 && AlarmHeightLow==null )
                 {
                    AlarmHeightLow = new clsAlarm(clsAlarm.enumAlarmType.Height,$"WARNING: {Name} height low.",this);
                    Program.AlarmManager.New(AlarmHeightLow);
                 }
             }
           }
        }
        
        clsAlarm AlarmLatencyLow = null;
        uint _latencyLowCount;
        public uint LatencyLowCount
        {
           get
           {
             return _latencyLowCount;
           }
           set
           {
             _latencyLowCount = value;
             if (_latencyLowCount==0 && AlarmLatencyLow!=null)
             {
                Program.AlarmManager.Clear(AlarmLatencyLow,$"CLEARED:  {Name} poor latency cleared.");
                AlarmLatencyLow = null;
             }
             else if (_requestFailCount ==0 && AlarmLatencyLow==null && _latencyLowCount > 3)
             {
                AlarmLatencyLow = new clsAlarm(clsAlarm.enumAlarmType.Latency,$"WARNING: {Name} latency poor.",this);
                Program.AlarmManager.New(AlarmLatencyLow);
             }
           }
        }


        clsAlarm AlarmRequestFail = null;
        uint _requestFailCount;
        public uint RequestFailCount
        {
           get
           {
             return _requestFailCount;
           }
           set
           {
             if (_requestFailCount == value) return; //no change
             
             _requestFailCount = value;
             if (_requestFailCount==0)
             {
                if (AlarmRequestFail!=null)
                {
                    Program.AlarmManager.Clear(AlarmRequestFail,$"CLEARED: {Name} now responding.");
                    AlarmRequestFail = null;
                    GetVersionAsync();
                }
                if (!String.IsNullOrEmpty(ErrorMsg)) ErrorMsg="";
                if (NodeGroup.Network.TopHeight > LeaderHeight) SyncMode=true;
             }
             else if (AlarmRequestFail ==null && _requestFailCount == 2)
             {
                AlarmRequestFail = new clsAlarm(clsAlarm.enumAlarmType.NoResponse,$"WARNING: {Name} not responding.",this);
                Program.AlarmManager.New(AlarmRequestFail);
                RunMTRAsync();
             }
           }
        }
                
        public async Task GetHeightAsync(int timeout = 2000)
        {
             await Task.Run(() => {GetHeight(timeout);});
        }
                
        public void GetHeight(int timeout = 2000)
        {
            try {
                
                var client = new RestClient($"http://{Host}:8088");
                        
                client.Timeout = timeout;                    
                        
                var request = new RestRequest("v2", Method.POST);
                request.AddHeader("Content-type", "application/json");
                request.AddHeader("header", "value");
                request.AddJsonBody(
                    new { jsonrpc = "2.0", id = 0, method = "heights" }
                );
                  
                // execute the request
                var sw = Stopwatch.StartNew();
                IRestResponse response = client.Execute(request);
                sw.Stop();
                
                if(response.ResponseStatus == ResponseStatus.Completed)
                {                    
                   var content = response.Content; // raw content as string
                   
                   var pos1 = 0;
                   var pos2= 0;
                   
                    if (!string.IsNullOrEmpty(content))
                    {
                        pos1 = content.IndexOf("leaderheight\":");
                        pos1+=14;
                        pos2 = content.IndexOf(",",pos1);
                    
                        uint msgheight=0;
                        if (UInt32.TryParse(content.Substring(pos1,pos2-pos1),out msgheight))
                        {
                           LeaderHeight = msgheight;
                        }
                        else
                        {
                            ErrorMsg="Invalid data";
                        }
                    }
                    else
                    {
                        ErrorMsg="Empty data";
                    }
                    LatencyList.Add((int)sw.ElapsedMilliseconds);
                    PacketLoss.Add(0);
                    RequestFailCount = 0;
                    
               } else if(response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
               {
                PacketLoss.Add(100);
                ErrorMsg=response.ErrorMessage;
                
                RequestFailCount++;                
               }
               else if(response.ResponseStatus == ResponseStatus.None)
               {
                 ErrorMsg="Empty data";
               }
               
              // Console.WriteLine(ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                RequestFailCount++;
            }
        }
        
        public bool SyncMode
        {
            get
            {
                return AlarmSyncing!=null;
            }
            set
            {
                if (value)
                {
                    if (AlarmSyncing==null)
                    {
                        AlarmSyncing = new clsAlarm(clsAlarm.enumAlarmType.Syncing,"{Name} in SYNC mode.",this);
                        Program.AlarmManager.New(AlarmRequestFail);
                    }
                }
                else if (AlarmSyncing != null)
                {
                    Program.AlarmManager.Clear(AlarmSyncing,$"{Name} SYNC cleared.");
                }
            }
        }
        

        public async Task GetVersionAsync(int timeout = 2000)
        {
             await Task.Run(() => {GetVersion(timeout);});
        }

        public void GetVersion(int timeout = 2000)
        {
            try {
                var client = new RestClient($"http://{Host}:8088");
                        
                client.Timeout = timeout;                    
                        
                var request = new RestRequest("v2", Method.POST);
                request.AddHeader("Content-type", "application/json");
                request.AddHeader("header", "value");
                request.AddJsonBody(
                    new { jsonrpc = "2.0", id = 0, method = "properties" }
                );
                  
                // execute the request
                IRestResponse response = client.Execute(request);
                
                if(response.ResponseStatus == ResponseStatus.Completed)
                {                    
                   var content = response.Content; // raw content as string
                   
                   var pos1 = 0;
                   var pos2= 0;
                   
                    if (!string.IsNullOrEmpty(content))
                    {
                        pos1 = content.IndexOf("factomdversion\":");
                        pos1+=17;
                        pos2 = content.IndexOf("\"",pos1);
                        if (pos2>pos1) NodeVersion = content.Substring(pos1,pos2-pos1);
                    }
                    else
                    {
                        ErrorMsg="Empty version data";
                    }
                    
               } else if(response.ResponseStatus == ResponseStatus.Error || response.ResponseStatus == ResponseStatus.TimedOut)
               {
                    ErrorMsg=response.ErrorMessage;
               }
               else if(response.ResponseStatus == ResponseStatus.None)
               {
                    ErrorMsg="Empty data";
               }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
      
        async public void PingHostAsync()
        {
            try
            {
                var pingTask = Task.Run(() =>
                {
            
                    Uri myUri = new Uri(Host);
                
                    clsRollingAverage pingLatency = new clsRollingAverage(10);
                    clsRollingAverage pingPacketLoss = new clsRollingAverage(10);
                    
                    try
                    {
                        using (var pinger = new Ping())
                        {
                            for (var f=0;f<10;f++)
                            {
                                PingReply reply = pinger.Send(myUri.Host,2000);
                                
                                if (reply.Status == IPStatus.Success)
                                {
                                    pingPacketLoss.Add((int)reply.RoundtripTime);
                                    pingPacketLoss.Add(0);
                                    Program.SendAlert($"Ping {myUri.Host} {reply.RoundtripTime:0} ms  {pingPacketLoss.CurrentAverage:0.0}%");
                                }
                                else
                                {
                                    pingPacketLoss.Add(100);
                                    Program.SendAlert($"Ping {myUri.Host} {reply.Status} ms  {pingPacketLoss.CurrentAverage:0.0}%");
                                }
                            }
                        }
                    }
                    catch (PingException ex)
                    {
                        Program.SendAlert($"Ping error {myUri.Host} {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                Program.SendAlert($"Ping error {Name} {ex.Message}");
            }
        }
        
        public void RunMTRAsync()
        {
            try{
                    Console.WriteLine($"Starting MTR on {Name} {Host}");
                   
                    var process = new Process
                    {
                        StartInfo = { FileName = "/usr/bin/mtr",
                                      Arguments = $"-rw {Host}",
                                      UseShellExecute = false,
                                      RedirectStandardOutput = true,
                                      // RedirectStandardError = true
                                    },
                            EnableRaisingEvents = true
        
                    };
        
                    process.Exited += (sender, eargs) =>
                    {
                        var mtrOutput = process.StandardOutput.ReadToEnd();
                        Console.WriteLine(mtrOutput);
                        if (AlarmRequestFail!=null) AlarmRequestFail.AddNote($"{Name}```{mtrOutput}```");
                        process.Dispose();
                    };
                    process.Start();
            }catch (Exception ex)
            {
                Console.WriteLine($"RunMTRAsync {ex.Message}");
            }
        }
       
        public void Update(clsNode node)
        {
            if (Name != node.Name) throw new Exception("index name does not match");
        
            Group = node.Group;
            Host = node.Host;
            
            if (Monitor != node.Monitor)
            {
                Monitor = node.Monitor;
                HeightLowCount=0;
                LatencyLowCount=0;
                RequestFailCount=0;
            }
            PostPopulate();
        }
        
        public string PostPopulate()
        {
            ErrorMsg = "";
            return (!Program.NodeGroupList.TryGetValue(Group,out NodeGroup)) ? "Error: Node Group Not Found!" : null;
        }
        
        public new String ToString()
        {
            return $"{Name}\t{Host}\t{LeaderHeight}\t{LatencyList.CurrentAverage.ToString().PadLeft(3)} ms ({(100-PacketLoss.CurrentAverage):0.#}%) {ErrorMsg??""}";
        }
        
        public void AppendDisplayColumns(ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol(Name ?? "?");
            columnDisplay.AppendCol(Host ?? "?");
            columnDisplay.AppendCol(NodeVersion??"");
            columnDisplay.AppendCol($"{LeaderHeight}");
            columnDisplay.AppendCol($"{LatencyList.CurrentAverage.ToString().PadLeft(3)} ms ({(100-PacketLoss.CurrentAverage):0.#}%) ");
            
            if (!Monitor)
            {
                 columnDisplay.Append ("MONITOR OFF");
            }
            else if (AlarmRequestFail !=null)
            {
                 columnDisplay.Append ("NOT RESPONDING");
            }
            else
            {
                if (AlarmHeightLow!=null) columnDisplay.Append ("HEIGHT LOW ");
                if (AlarmSyncing!=null) columnDisplay.Append ("SYNCING ");
                if (AlarmLatencyLow !=null) columnDisplay.Append ("LATENCY LOW ");
            }
            
            if (String.IsNullOrEmpty(ErrorMsg)) columnDisplay.Append(ErrorMsg);
            
            
        }
        
    }
}
