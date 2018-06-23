using System;
using System.Linq;
namespace TFABot
{
    public class ASheetColumnHeader : Attribute
    {
        String[] HeaderMatch;
        public bool IsIndex {get; private set;}
        
        public ASheetColumnHeader(params String[] headerMatch )
        {
            HeaderMatch = headerMatch;
        }
    
        public ASheetColumnHeader(bool index, params String[] headerMatch )
        {
            HeaderMatch = headerMatch;
            IsIndex = index;
        }
        
        public bool IsMatch(String text)
        {
            text = text.ToLower();
            foreach (var item in HeaderMatch)
            {
                if (text.Contains(item)) return true;
            }
            return false;
        }
    }
}
