using System;
namespace TFABot.Git
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class GitCommit : Attribute
    {
        
    public string Hash;
    public GitCommit() : this(string.Empty) {}
    public GitCommit(string hash) { Hash = hash; }
        
    }
}
