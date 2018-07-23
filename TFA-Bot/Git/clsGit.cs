using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace TFABot.Git
{
    public class clsGit : IDisposable
    {
        
        public clsGitHead Head {get; private set;}
        public Repository Repo {get; private set;}
        List<clsGitBranchInfo> BranchList = new List<clsGitBranchInfo>();
        
        public clsGit(String gitDirectory = null)
        {
#if DEBUG
            if (Directory.Exists("/HOME/TFA-Bot")) gitDirectory = "/HOME/TFA-Bot";
#endif      
            if (String.IsNullOrEmpty(gitDirectory)) gitDirectory = System.Reflection.Assembly.GetEntryAssembly().Location;

            var dir = new DirectoryInfo(gitDirectory);
            while (dir.GetDirectories(".git",SearchOption.TopDirectoryOnly).Length==0)
            {
                dir = dir.Parent;
                if (dir==null) throw new Exception("No .git dir found");
            }
            Console.WriteLine($"Opening Git @ {dir.FullName}");
            Repo = new Repository(dir.FullName);
            Fetch();
            Head = new clsGitHead(this);
        }
       
        public void Fetch()
        {
            FetchOptions options = new FetchOptions() { Prune=true };
            foreach (Remote remote in Repo.Network.Remotes)
            {
                Console.WriteLine($"GIT fetch {remote.Url}");
                Repo.Network.Fetch(remote.Name,remote.FetchRefSpecs.Select(x=>x.Specification).ToArray(),options);
            }
        }
        
        public void GetBranches()
        {
            BranchList.Clear();
            
            foreach (var branch in Repo.Branches.Where(x=>x.IsRemote))
            {
               if (!branch.FriendlyName.EndsWith("/HEAD"))
               {
                  var BranchItem = new clsGitBranchInfo(this, branch, GetTag(branch));
                  BranchList.Add(BranchItem);
               }
            }
        }
              
        public Tag GetTag(Branch branch, Commit commit = null)
        {
            if (commit == null) commit = branch.Commits.FirstOrDefault();
            
            var lastTagCommit = (from branchCommits in branch.Commits
            join tags in Repo.Tags on branchCommits.Sha equals tags.Target.Sha
            where branchCommits.Committer.When <= commit.Committer.When
            select (commit: branchCommits, tag: tags) ).FirstOrDefault();
                
            return lastTagCommit.tag;
        }
        
        public void Switch(String headText, bool pull = false)
        {
            Branch branch;
            Branch existingBranch;
            var splittext = headText.Split('/');
            var name = splittext.Last();
            
            //Find name (case sensitive) or sha, local first
            existingBranch = Repo.Branches.OrderBy(x=>x.IsRemote).FirstOrDefault(x=>x.FriendlyName.EndsWith(name) || x.Tip.Sha.StartsWith(name));
            
            //Try lower case name, local first
            if (existingBranch==null)
            {
                name = name.ToLower();
                existingBranch = Repo.Branches.OrderBy(x=>x.IsRemote).FirstOrDefault(x=>x.FriendlyName.ToLower().EndsWith(name));
            }
                      
            var chechoutOptions = new CheckoutOptions() { };
            
            //If remote only
            if (existingBranch!=null && existingBranch.IsRemote)
            {
                Branch newBranch = Repo.CreateBranch(name, existingBranch.Tip);
                // Make the local branch track the upstream one
                Repo.Branches.Update(newBranch, b => b.TrackedBranch = existingBranch.CanonicalName);
                existingBranch = newBranch;
            }
                       
            if (existingBranch!=null)
            {
                branch = Commands.Checkout(Repo,existingBranch,chechoutOptions);
            }
            else
            {   //Try using gitlib to find branch or commit
                branch = Commands.Checkout(Repo,headText,chechoutOptions);
            }           
            
            if (pull)
            {
                PullOptions pulloptions = new PullOptions()
                {
                    FetchOptions = new FetchOptions() { Prune = true }
                };
                
                Commands.Pull(Repo, new LibGit2Sharp.Signature("In App", "inapp@nowhere.com", new DateTimeOffset(DateTime.Now)), pulloptions);
            }
        }
        
        
        
        public new string ToString()
        {
            GetBranches();
             var cd = new clsColumnDisplay();
             
             cd.AppendCol("Branch");
             cd.AppendCol("Sha");
             cd.AppendCol("Commit");
             cd.AppendCol("Tag");
             
             cd.AppendCharLine('-');
             
             foreach (var branch in BranchList)
             {
                branch.AppendDisplayColumns(ref cd);
                cd.NewLine();
             }
             
             cd.AppendLine(Head.ToString());

             return cd.ToString();
        }

        public void Dispose()
        {
            Repo.Dispose();
        }
        
    }
}
