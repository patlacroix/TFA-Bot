using System;
using System.Text;
using LibGit2Sharp;

namespace TFABot.Git
{
    public class clsGitBranchInfo
    {
        private string FriendlyName;
        clsGit Git;
        
        public Tag TagRemote {get; private set;}
        public Tag TagHead {get; private set;}
        public Branch BranchRemote {get; private set;}
        public Branch BranchHead {get; private set;}
        
        public clsGitBranchInfo(clsGit git, Branch branch, Tag tag)
        {
            Git = git;
            BranchRemote = branch;
            TagRemote = tag;
            
            if (branch == Git.Repo.Head.TrackedBranch)
            {
              BranchHead = Git.Repo.Head;
              TagHead = Git.GetTag(BranchHead);
            }
        }
        
        
        public void AppendDisplayColumns(ref clsColumnDisplay columnDisplay)
        {
            columnDisplay.AppendCol( (BranchRemote.FriendlyName));
            columnDisplay.AppendCol( (BranchRemote.Tip.Sha.Substring(0,7)));
            columnDisplay.AppendCol( ($"{BranchRemote.Tip.Committer.When:yyyy-MM-dd HH:mm}"));
            columnDisplay.AppendCol(TagRemote!=null ? TagRemote.FriendlyName:"");
            if (BranchHead!=null)
            {
                 columnDisplay.AppendCol( ($"<== HEAD "));
                 if (Git.Repo.Head.Tip.Sha == BranchRemote.Tip.Sha) 
                    columnDisplay.Append ("Up to date");
                 else
                    if (TagHead?.FriendlyName!=null) columnDisplay.Append(TagHead.FriendlyName);
                    columnDisplay.Append ($"{(Git.Repo.Head.Tip.Tree.Count - BranchRemote.Tip.Tree.Count):+0;-0}");
            }
        }
        

        
    }
}
