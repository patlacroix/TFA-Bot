using System;
using System.Text;

namespace TFABot.Git
{
    public class clsGitHead: IDisposable
    {
        clsGit Git;
        bool DisposeofGit = false;
    
        public clsGitHead(clsGit git = null)
        {
           if (git == null)
            {
                Git = new clsGit();
                DisposeofGit = true;
            }
            else
            {
                Git = git;
            }
        }
        
        public String BranchName     {get{return Git.Repo.Head.FriendlyName;}}
        
        public String LocalSha       {get{return Git.Repo.Head?.Tip?.Sha ?? ""; }}
        public String LocalShaShort  {get{return Git.Repo.Head?.Tip?.Sha.Substring(0,7) ?? "";}}
        public DateTime? LocalDate    {get{return Git.Repo.Head?.Tip?.Committer.When.UtcDateTime;}}
        public String LocalTag       {get{return Git.GetTag(Git.Repo.Head)?.FriendlyName ?? "";}}
        public String LocalCommiter  {get{return Git.Repo.Head?.Tip?.Committer?.Name ?? "";}}

        public String RemoteSha       {get{return Git.Repo.Head?.TrackedBranch.Tip?.Sha ?? ""; }}
        public String RemoteShaShort  {get{return Git.Repo.Head?.TrackedBranch.Tip?.Sha.Substring(0,7) ?? "";}}
        public DateTime? RemoteDate      {get{return Git.Repo.Head?.TrackedBranch.Tip?.Committer.When.UtcDateTime;}}
        public String RemoteTag       {get{return Git.GetTag(Git.Repo.Head.TrackedBranch)?.FriendlyName ?? "";}}
        public String RemoteCommiter  {get{return Git.Repo.Head?.TrackedBranch.Tip?.Committer?.Name ?? "";}}

        public int DiffCount
        {
            get
            {
                    if (Git.Repo?.Head?.Tip?.Sha == null) return 0;
                    //if (Git.Repo == null ||  Git.Repo.Head?.Tip == null ?? true) return 0;
                    return Git.Repo.Head.Tip.Tree?.Count ?? 0 - Git.Repo.Head.TrackedBranch?.Tip?.Tree?.Count ?? 0;
            }
        }
        
        static public string GetHeadToString()
        {
            try
            {
                using (var git = new clsGitHead())
                {
                    return git.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"Git unavailabe {ex.Message}";
            }
        }
        
        public new string ToString()
        {
            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(LocalSha))
            {
                sb.AppendLine();
                sb.AppendLine($"HEAD @ {BranchName} {LocalShaShort} {LocalCommiter} {LocalDate:yyyy-MM-dd HH:mm} {LocalTag}");
                
                if (Git.Repo.Head.Tip.Sha != Git.Repo.Head.TrackedBranch.Tip.Sha) 
                {    
                    int commitcount = DiffCount;
                    sb.AppendLine(commitcount < 0 ? $"{-commitcount} commit(s) behind": $"{commitcount} commit(s) ahead of");
                    sb.Append ($" {BranchName} {RemoteShaShort} {RemoteCommiter} {RemoteDate:yyyy-MM-dd HH:mm} {RemoteTag}");
                }
                
                if (Git.Repo.Head.Tip.Sha!= clsVersion.GitCommitHash)
                {
                    sb.AppendLine($"NOTE: Compiled version does not not match HEAD {clsVersion.GitCommitHash.Substring(0,7)}");
                }
                
            }
            else
            {
               sb.AppendLine("HEAD: No information");
            }
            
            return sb.ToString();
        }
        
        public void Dispose()
        {
            if (DisposeofGit) Git.Dispose();
        }
        
    }
}
