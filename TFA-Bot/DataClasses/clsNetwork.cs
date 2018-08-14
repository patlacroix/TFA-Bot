using System;

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
        
        clsRollingAverage AverageBlocktime = new clsRollingAverage(10);
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
            NextHeight = LastHeight.Value.AddSeconds( AverageBlocktime.Count > 1 ? AverageBlocktime.CurrentAverage : (int)BlockTimeSeconds );
            
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
                    if (++LateHeightCount==1)
                    {
                        NetworkAlarm = new clsAlarm(clsAlarm.enumAlarmType.Network,$"WARNING: Network Height {seconds:0} sec late.  {Name} stall or an election?",this);
                        Program.AlarmManager.New(NetworkAlarm);
                    }
                }
            }
        }
        
        public void AppendDisplayColumns(ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol(Name ?? "?");
            columnDisplay.AppendCol($"{TopHeight}");
            
            if (FullBlockMesured)
            {
                columnDisplay.AppendCol(LastHeight.HasValue ? $"-{(DateTime.UtcNow - LastHeight.Value).ToMSDisplay()}":"n/a");
                columnDisplay.AppendCol(NextHeight.HasValue ? $"+{(NextHeight.Value - DateTime.UtcNow).ToMSDisplay()}":"n/a");
                
                if (AverageBlocktime.Count>1)
                {
                    columnDisplay.AppendCol($"{new TimeSpan(0,0,AverageBlocktime.CurrentAverage).ToMSDisplay()}");
                }
                else
                {
                    columnDisplay.AppendCol("n/a");
                }
            }
            else
            {
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a");
                columnDisplay.AppendCol("n/a - please wait 1-2 blocks");
            }
        }
    }
}
