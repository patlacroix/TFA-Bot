using System;
namespace TFABot
{
    public class clsSetting : ISpreadsheet<clsSetting>
    {
        public clsSetting()
        {
        
        }
        
        [ASheetColumnHeader(true,"setting")]
        public string Key {get;set;}
        
        [ASheetColumnHeader("value")]
        public string Value {get;set;}
        
        
        public void Update(clsSetting setting)
        {
            if (Key != setting.Key) throw new Exception("index name does not match");
            
            Key = setting.Key;
            Value = setting.Value;
        }
        
        public void PostPopulate()
        {
        
        }
    }
}