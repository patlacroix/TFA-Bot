using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TFABot.Git;

namespace TFABot
{
    static public class clsVersion
    {
        static public String GitCommitHash {get;private set;}
        const string versionFilePath = "version.txt";
        static public bool VersionChangeFlag {get; private set;}
    
        static clsVersion()
        {
            var GitCommit = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyGitCommit),false).Cast<AssemblyGitCommit>();
            if (GitCommit!=null && GitCommit.Any<AssemblyGitCommit>()) GitCommitHash = GitCommit.First().Hash;
            if (!String.IsNullOrEmpty(GitCommitHash))
            {
                VersionChangeFlag = CheckFileVersion();
            }
            else
            {
                GitCommitHash = "< Not Set >";
            }
        }
        
        static string GetFileVersion()
        {
            try
            {
                Console.Write("Get file version...");
                if (File.Exists(versionFilePath))
                {
                    var ver = File.ReadAllText(versionFilePath);
                    Console.Write(ver);
                    return ver;
                }
                Console.WriteLine("not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetFileVersion Error {ex.Message}");
            }
            return null;
        }
        
        
        static public string CheckForUpdate()
        {
           try
           { 
               using (var git = new clsGit())
               {
                    return git.Head.RemoteSha != GitCommitHash ? git.Head.ToString() : null;
               }
           } catch (Exception ex)
           {
                Console.WriteLine($"CheckForUpdate Error:{ex.Message}");
                return null;
           }
        }
        
        static public bool CheckFileVersion()
        {
            var fileVer = GetFileVersion();
            if (fileVer == null || fileVer != GitCommitHash)
            {
                VersionChangeFlag=true;
                SetFileVersion(GitCommitHash);
                return true;
            }
            return false;
        }
        
        static void SetFileVersion(string version)
        {
           try
           { 
                File.WriteAllText(versionFilePath,version);
           }
           catch (Exception ex)
           {
                Console.WriteLine($"SetFileVersion Error {ex.Message}");
           }
        }
        
    }
}
