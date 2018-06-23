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
        
        public clsAlarm NetworkAlarm = null;
        
        public void Update(clsNetwork network)
        {
            if (Name != network.Name) throw new Exception("index name does not match");
            
            Name = network.Name;
            BlockTimeSeconds = network.BlockTimeSeconds;
            BlockTimeSecondsAllowance = network.BlockTimeSecondsAllowance;
            StallNotification = network.StallNotification;
        }
        
        public void PostPopulate()
        {
        
        }
        
        public void SetTopHeight(uint height)
        {
            TopHeight = height;
            LastHeight = DateTime.UtcNow;
            NextHeight = LastHeight.Value.AddSeconds(BlockTimeSeconds);
        }
        
        public void CheckStall()
        {
            bool ok = true;
            if (NextHeight.HasValue && DateTime.UtcNow > NextHeight.Value)
            {
                var seconds = (DateTime.UtcNow - NextHeight).Value.TotalSeconds;
                if (seconds>BlockTimeSecondsAllowance)
                {
                    ok=false;
                    if (++LateHeightCount==1)
                    {
                        NetworkAlarm = new clsAlarm(clsAlarm.enumAlarmType.Network,$"Warning Height {seconds:0} sec late.  {Name} Stall?",this);
                        Program.AlarmManager.New(NetworkAlarm);
                    }
                }
            }
            if (LateHeightCount>0 && ok)
            {
                LateHeightCount=0;
                Program.AlarmManager.Clear(NetworkAlarm, $"Stall Alarm Cleared {Name}");
                NetworkAlarm = null;
            }
        }
        
    }
}
