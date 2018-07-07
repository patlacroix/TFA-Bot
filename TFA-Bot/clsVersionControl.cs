using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TFABot
{
    static public class clsVersionControl
    {
        const string versionFilePath = "version.txt";
        
        static Regex GetVersionRegex = new Regex(@"(?<=version\s{0,}=\s{0,})\S*");
        static Regex GetVersionDateRegex=new Regex(@"(?<=versiondate\s{0,}=\s{0,})\d{4,4}-\d{2,2}-\d{2,2}\s\d{2,2}:\d{2,2}:\d{2,2}\s[+-]?\d{4,4}");
    
        static public string PreviousVersion {get; private set;}
        static public string Version {get; private set;}
        static public string VersionDateTime {get; private set;}
        static public bool UpdatedFlag {get; private set;}

        static clsVersionControl()
        {
            try
            {
                var match = GetVersionRegex.Match(Environment.CommandLine);
                if (match.Success) Version = match.Value;
    
                match = GetVersionDateRegex.Match(Environment.CommandLine);
                if (match.Success) VersionDateTime = match.Value;
                
                Console.WriteLine($"Version {Version} {VersionDateTime}");
                
                if (File.Exists(versionFilePath))
                {
                    PreviousVersion = File.ReadAllText(versionFilePath);
                }
                
                if (!String.IsNullOrEmpty(Version) && PreviousVersion != Version)
                {
                    UpdatedFlag=true;
                    if (!String.IsNullOrEmpty(PreviousVersion)) Console.WriteLine($"New version {PreviousVersion} => {Version}");
                    File.WriteAllText(versionFilePath,Version);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Version error: {ex.Message}");
            }
        }
        
        public static String GetLatestTag()
        {
            try
            {
               return ExecuteBashCommand("git fetch --tags && git describe --tags --long `git rev-list --tags --max-count=1`");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "not found";
            }
        }
        
        static string ExecuteBashCommand(string command, int timeout = 5000)
        {
            // according to: https://stackoverflow.com/a/15262019/637142
            // thans to this we will pass everything as one command
            command = command.Replace("\"","\"\"");
    
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \""+ command + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
    
            proc.Start();
            proc.WaitForExit(timeout);
            if (proc.HasExited) return proc.StandardOutput.ReadToEnd();
            return "Command Timeout";
            
        }
        

    }
}
