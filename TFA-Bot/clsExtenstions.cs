using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TFABot
{
    static public class clsExtenstions
    {
        static Regex UTCMatch = new Regex(@"(?<=UTC)\s{0,}[\+\-]\d*");
               
        static clsExtenstions()
        {
       
        }
        
        
        public static bool TimeBetween(this DateTime datetime, TimeSpan start, TimeSpan end)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = datetime.TimeOfDay;
            // see if start comes before end
            if (start < end) return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }
        
        
        public static DateTime ToAbvTimeZone(this DateTime date,String abvTimeZone)
        {
                
            abvTimeZone = abvTimeZone.ToUpper();
            
            if (abvTimeZone == "UTC") return TimeZoneInfo.ConvertTimeToUtc(date);
                        
            var match = UTCMatch.Match(abvTimeZone);
            if (match.Success)
            {
                int offset = int.Parse(match.Value);
                return DateTime.UtcNow.AddHours(offset);
            }
             
            return  TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, abvTimeZone);
        }
    
        public static string ToDHMDisplay(this TimeSpan ts)
        {
            return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m";
        }
        
    }
}
