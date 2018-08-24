using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

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
                
            abvTimeZone = abvTimeZone.Trim().ToUpper();
            
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
        
        public static string ToHMSDisplay(this TimeSpan ts)
        {
            return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
        }
        
        public static string ToMSDisplay(this TimeSpan ts)
        {
            return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
          return enumerable.GroupBy(keySelector).Select(grp => grp.First());
        }
        
        public static string[] SplitAfter(this string text, int len)
        {
            List<string> textOut = new List<string>();
            int pt1 = 0;
            int pt2 = len;
            
            while (pt1 < text.Length)
            {
                pt2 = text.IndexOf('\n',pt2);
                if (pt2 == -1) pt2 = text.Length - 1;
                
                textOut.Add(text.Substring(pt1,pt2-pt1));
                pt1 = pt2 + 1;
                pt2 = pt1 + len;
                if (pt2 >= text.Length) pt2 = text.Length;
            }
            
            return textOut.ToArray();
        }
    }
}
