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
        //int colCount = -1;
        int colCountMax = -1;
        StringBuilder sb = new StringBuilder();
    
        public int Margin {get; private set;}
    
        public clsColumnDisplay(int margin = 1)
        {
            Margin = margin;
        }
        
        
        
        public void AppendCol(String text)
        {
            Columns.Add(text);
            
            if (Columns.Count > colCountMax) colCountMax = Columns.Count;
            
            if (ColumnMaxLen.Count < Columns.Count)
              ColumnMaxLen.Add(text.Length);
            else if (ColumnMaxLen[Columns.Count-1] < text.Length)
              ColumnMaxLen[Columns.Count-1] = text.Length;
        }
        
          
        public void AppendLine(String text="")
        {
            NewLine();
            Lines.Add(text);
        }
        
        public void AppendCharLine(char linechar)
        {
            NewLine();
            Lines.Add(linechar);
        }
        
        public void Append(string text)
        {
            if (Columns.Count==0)
            {
                Lines[Lines.Count-1] = Lines[Lines.Count-1]+text;
            }
            else
            {
                Columns[Columns.Count-1] = Columns[Columns.Count-1]+text;
            }
        }
        
        public void NewLine()
        {
            if (Columns.Count>0)
            {
                Lines.Add(Columns.ToArray());
                Columns.Clear();
            }
        }
        
                
        public new string ToString()
        {
            foreach (var line in Lines)
            {
                if (line is String[])
                {
                    for (int f=0; f < colCountMax;f++)
                    {
                        if (f < ((string[])line).Length)
                        {
                            sb.Append( ((string[])line)[f].PadRight(ColumnMaxLen[f]+Margin));
                            if (f < colCountMax-1) sb.Append("| ");
                        }
                        else
                        {
                            sb.AppendLine(new string(' ',ColumnMaxLen[f]+Margin));
                        }
                    }
                    sb.AppendLine();
                }
                else if (line is String)
                {
                    sb.AppendLine((string)line);
                }
                else if (line is char)
                {
                    for (int f=0; f < colCountMax;f++)
                    {
                        sb.AppendLine(new string((char)line,ColumnMaxLen[f]+Margin));
                        if (f < colCountMax-1) sb.Append("| ");
                    }
                }
            }
            return sb.ToString();
        }
        
    }
}
