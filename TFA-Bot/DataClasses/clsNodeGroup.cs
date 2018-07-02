using System;
using System.Linq;
namespace TFABot
{
    public class clsNodeGroup : ISpreadsheet<clsNodeGroup>
    {
        public clsNodeGroup()
        {
        }
           
        public clsNetwork Network {get; private set;}

        [ASheetColumnHeader(true,"group")]
        public String Name {get;set;}

        [ASheetColumnHeader("network")]
        public String NetworkString
        {
            get
            {
                return Network?.Name??"Not Set";
            }
            set
            {
                clsNetwork network;
                if (Program.NetworkList.TryGetValue(value,out network))
                {
                    Network = network;
                }
                else
                {
                  new Exception("Network Name not found.");
                }
            }
        }

        [ASheetColumnHeader("ping")]
        public String Ping {get;set;}

        [ASheetColumnHeader("height")]
        public String Height {get;set;}

        [ASheetColumnHeader("latency")]
        public String Latency {get;set;}

        [ASheetColumnHeader("stall")]
        public String Stall {get;set;}

        public void Monitor()
        {
            foreach (var node in Program.NodesList.Values.Where(x=>x.Group == this.Name && x.Monitor))
            {
                //Check the height, against heighest known height
                if (node.LeaderHeight > Network.TopHeight) //New highest LearderHeight.
                {
                
                    if (Network.TopHeight > 0 && (node.LeaderHeight - Network.TopHeight > 10))
                    {
                        //Suspect wrong network setting
                        if (Network.NetworkAlarm==null)
                        {
                            Network.NetworkAlarm = new clsAlarm(clsAlarm.enumAlarmType.Error,$"WARNING: {node.Name} height too high!  Wrong Bot setting?",Network);
                            Program.AlarmManager.New(Network.NetworkAlarm);
                        }
                    }
                    else
                    {
                        Network.SetTopHeight(node.LeaderHeight);
                        node.HeightLowCount=0;
                    }
                }
                else if (node.LeaderHeight < Network.TopHeight && node.RequestFailCount==0)  //Node height too low.
                {
                    if (Network.TopHeight - node.LeaderHeight > 10) //If by a large amount, maybe Wrong setting or syncing?
                    {
                        if (Network.NetworkAlarm==null)
                        {
                            Network.NetworkAlarm = new clsAlarm(clsAlarm.enumAlarmType.Error,$"WARNING: {node.Name} height low for network.  Wrong Bot setting or syncing?",Network);
                            Program.AlarmManager.New(Network.NetworkAlarm);
                        }
                    }
                    else
                    {
                        node.HeightLowCount++;
                    }
                }
                else  //All good
                {   
                    node.HeightLowCount=0;
                    if (Network.NetworkAlarm!=null)
                    {
                        Program.AlarmManager.Clear(Network.NetworkAlarm,$"{Network.Name} clear.");
                        Network.NetworkAlarm = null;
                    }
                }
                                    
                //Check latency
                if (node.Latency > node.LatencyLowest * 3 && node.LatencyLowest>50) node.LatencyLowCount ++;
            }
        }
       
         public void Update(clsNodeGroup group)
        {
            if (Name != group.Name) throw new Exception("index name does not match");
            
            Ping = group.Ping;
            Height = group.Height;
            Latency = group.Latency;
            Stall = group.Stall;
            Network = group.Network;
        }

        public void PostPopulate()
        {
        
        }
    }
}
