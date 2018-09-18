using System;
using System.Linq;
using System.Text;

namespace TFABot
{
    public class clsNetwork : ISpreadsheet<clsNetwork>
    {
        public clsNetwork()
        {

        }
        
        [ASheetColumnHeader(true,"name")]
        public string Name {get;set;}
        [ASheetColumnHeader("time")]
        public uint BlockTimeSeconds {get;set;}
        [ASheetColumnHeader("after")]
        public uint BlockTimeSecondsAllowance {get;set;}
        [ASheetColumnHeader("stall")]
        public string StallNotification {get;set;}
        
       
        public uint TopHeight {get; private set;}
        public DateTime? LastHeight  {get; private set;}
        public DateTime? NextHeight  {get; private set;}
        int LateHeightCount;
        
        clsRollingAverage AverageBlocktime = new clsRollingAverage(6);
        TimeSpan? LastBlockTimeDuration;
        bool FullBlockMesured = false;
        
        public clsAlarm NetworkAlarm = null;
        
        public void Update(clsNetwork network)
        {
            if (Name != network.Name) throw new Exception("index name does not match");
            
            Name = network.Name;
            BlockTimeSeconds = network.BlockTimeSeconds;
            BlockTimeSecondsAllowance = network.BlockTimeSecondsAllowance;
            StallNotification = network.StallNotification;
        }
        
        public string PostPopulate()
        {
           return (!Program.NotificationPolicyList.ContainsKey(StallNotification))? $"Error: {StallNotification} not found" : null;
        }
        
        public void SetTopHeight(uint height)
        {
            TopHeight = height;
            
            if (LastHeight.HasValue)
            {
                if (FullBlockMesured)
                {
                    LastBlockTimeDuration = (DateTime.UtcNow - LastHeight.Value);
                    AverageBlocktime.Add((int)LastBlockTimeDuration.Value.TotalSeconds);
                }
                else
                {
                    FullBlockMesured=true;
                }
            }
            
            LastHeight = DateTime.UtcNow;           
            NextHeight = LastHeight.Value.AddSeconds( (int)BlockTimeSeconds );
            
            if (LateHeightCount>0)
            {
                LateHeightCount=0;
                Program.AlarmManager.Clear(NetworkAlarm, $"CLEARED: Network Stall Alarm {Name}");
                NetworkAlarm = null;
            }
        }
        
        public void CheckStall()
        {
            if (NextHeight.HasValue && DateTime.UtcNow > NextHeight.Value)
            {
                var seconds = (DateTime.UtcNow - NextHeight).Value.TotalSeconds;
                if (seconds>BlockTimeSecondsAllowance)
                {
                
                    if (MonitoringSources==0)  //No data sources, so we need to cancel 
                    {
                        NextHeight=null;
                        if (NetworkAlarm!=null)
                        {
                           Program.AlarmManager.Remove(NetworkAlarm);
                           NetworkAlarm = null;
                        }
                    }
                    else if (++LateHeightCount==1)
                    {
                        NetworkAlarm = new clsAlarm(clsAlarm.enumAlarmType.Network,$"WARNING: Network Height {seconds:0} sec late.  {Name} stall or an election?",this);
                        Program.AlarmManager.New(NetworkAlarm);
                    }
                }
            }
        }
        
        public uint MonitoringSources
        {
            get
            {
                return (uint)Program.NodesList.Values.Count(x=>x.NodeGroup.Network == this && x.Monitor);
            }
        }
        
        
        
        public void AppendDisplayColumns(ref clsColumnDisplay columnDisplay)
        {
        
            var sources = MonitoringSources;
        
            columnDisplay.AppendCol(Name ?? "?");
            columnDisplay.AppendCol($"{TopHeight:#;;'n/a'}");
            columnDisplay.AppendCol($"{sources}");
            
            if (MonitoringSources==0)
            {
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a - no data sources available");
            }
            else if (FullBlockMesured)
            {
                columnDisplay.AppendCol(LastHeight.HasValue ? $"{LastHeight.Value:HH:mm:ss}":"n/a");
                columnDisplay.AppendCol(NextHeight.HasValue ? $"{(NextHeight.Value - DateTime.UtcNow).ToMSDisplay()}":"n/a");
                
                var sb = new StringBuilder();
                foreach (var bt in AverageBlocktime.GetValues().Take(3).Reverse())
                {
                    if (sb.Length>0) sb.Append("<");
                    sb.Append($"[{new TimeSpan(0,0,bt).ToMSDisplay()}]");
                }
                
                if (MonitoringSources==1) sb.Append(" (only one data source)");
                
                columnDisplay.AppendCol(sb.ToString());
            }
            else
            {
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a - please wait for next block");
            }
        }
    }
}
