using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira.Linq;
using Atlassian.Jira;
using System.Linq;

namespace JiraModule
{
    /// <summary>
    /// Saves changes made to a Jira.Issue object
    /// </summary>
    [Alias("Save-Issue")]
    [Cmdlet(VerbsData.Save, "JIssue")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class SaveIssue : AsyncActionCmdlet
    {
        [Alias("JiraIssue")]
        [Parameter(
            Position = 0,
            ValueFromPipeline = true
        )]
        public Atlassian.Jira.Issue Issue { get; set; }

        protected override void ProcessRecord()
        {
            StartAsyncTask(
                $"Save issue [{Issue.Key}]",
                Issue.SaveChangesAsync()
            );
        }

        protected override void EndProcessing()
        {
            WaitAll();
        }
    }
}
