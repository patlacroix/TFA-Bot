using System;
using System.Collections.ObjectModel;

namespace TFABot
{
    public class clsUser : ISpreadsheet<clsUser>
    {
        public clsUser()
        {
        }

        [ASheetColumnHeader(true,"discord")]
        public String DiscordName {get;set;}
        [ASheetColumnHeader("name")]
        public String Name {get;set;}
        [ASheetColumnHeader("tel")]
        public String Tel {get;set;}
        [ASheetColumnHeader("mail")]
        public String email {get;set;}
        [ASheetColumnHeader("zone")]
        public String TimeZone {get;set;}
        [ASheetColumnHeader("tel")]
        public int Weight {get;set;}
        [ASheetColumnHeader("time from","timefrom")]
        public TimeSpan TimeFrom {get;set;}
        [ASheetColumnHeader("time to","timeto")]
        public TimeSpan TimeTo {get;set;}
    
        
        public void Update(clsUser user)
        {
            if (DiscordName != user.DiscordName) throw new Exception("index name does not match");
            
            Tel = user.Tel;
            Name = user.Name;
            email = user.email;
            TimeZone = user.TimeZone;
            Weight = user.Weight;
            TimeFrom = user.TimeFrom;
            TimeTo = user.TimeTo;
        }
        
        public void PostPopulate()
        {
            try
            {
                GetUserTime();  //Test to see if we can get the time.
            }
            catch (Exception ex)
            {
            
                Console.WriteLine($"{Name} has invalid Timezone {ex.Message}");
            }
         
        }
        
        public DateTime GetUserTime()
        {
            return DateTime.UtcNow.ToAbvTimeZone(TimeZone);
        }
        
        public bool OnDuty
        {
            get
            {
                return GetUserTime().TimeBetween(TimeFrom,TimeTo);
            }
        }
    }
}
