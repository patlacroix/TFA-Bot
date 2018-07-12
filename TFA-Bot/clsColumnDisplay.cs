using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TFABot
{
    public class clsColumnDisplay
    {
        List<object> Lines = new List<object>();
        List<string> Columns = new List<string>();
        List<int> ColumnMaxLen = new List<int>();
        int colCount = -1;
        int colCountMax = 0;
        StringBuilder sb = new StringBuilder();
    
        public int Margin {get; private set;}
    
        public clsColumnDisplay(int margin = 1)
        {
            Margin = margin;
        }
        
        
        
        public void AppendCol(String text)
        {
            Columns.Add(text);
            
            if (++colCount > colCountMax) colCountMax=colCount;
            
            if (ColumnMaxLen.Count <= colCount)
              ColumnMaxLen.Add(text.Length);
            else if (ColumnMaxLen[colCount] < text.Length)
              ColumnMaxLen[colCount] = text.Length;
        }
        
          
        public void AppendLine(String text="")
        {
            if (colCount>-1) NewLine();
            Lines.Add(text);
        }
        
        public void AppendCharLine(char linechar)
        {
            Lines.Add(linechar);
        }
        
        public void Append(string text)
        {
            if (colCount==-1)
            {
                Lines[Lines.Count-1] = Lines[Lines.Count-1]+text;
            }
            else
            {
                Columns[colCount] = Columns[colCount]+text;
            }
        }
        
        public void NewLine()
        {
            Lines.Add(Columns.ToArray());
            colCount=-1;
            Columns.Clear();
        }
        
                
        public new string ToString()
        {
            foreach (var line in Lines)
            {
                if (line is String[])
                {
                    for (int f=0; f < ((string[])line).Length;f++)
                    {
                        sb.Append( ((string[])line)[f].PadRight(ColumnMaxLen[f]+Margin));
                    }
                    sb.AppendLine();
                }
                else if (line is String)
                {
                    sb.AppendLine((string)line);
                }
                else if (line is char)
                {
                    sb.AppendLine(new string((char)line,ColumnMaxLen.Sum() + (colCountMax)));
                }
            }
            return sb.ToString();
        }
        
    }
}
