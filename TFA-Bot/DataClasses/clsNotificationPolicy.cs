using System;
namespace TFABot
{
    public class clsNotificationPolicy : ISpreadsheet<clsNotificationPolicy>
    {
        //Name  Discord Call
        
        public clsNotificationPolicy()
        {
            Call = -1;  //Default
            Discord = -1; //Default
        }
        
        [ASheetColumnHeader(true,"name")]
        public String Name {get;set;}
        
        [ASheetColumnHeader("discord")]
        public int Discord {get;set;}

        [ASheetColumnHeader("call")]
        public int Call {get;set;}

        public void Update(clsNotificationPolicy node)
        {
            if (Name != node.Name) throw new Exception("index name does not match");
            Discord = node.Discord;
            Call = node.Call;
        }

        public string PostPopulate()
        {
            return null;
        }
    }
}
