using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira.Linq;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Saves changes made to a Jira.Issue object
    /// </summary>
    [Cmdlet(VerbsData.Save, "Issue")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class SaveIssue : Cmdlet
    {
        Queue<AsyncAction> startedTasks = new Queue<AsyncAction>();

        [Alias("JiraIssue")]
        [Parameter(
            Position = 0,
            ValueFromPipeline = true
        )]
        public Atlassian.Jira.Issue Issue { get; set; }

        protected override void ProcessRecord()
        {
            var result = new AsyncAction(
                $"Save issue [{Issue.Key}]",
                Issue.SaveChangesAsync()
            );

            startedTasks.Enqueue(result);
        }

        protected override void EndProcessing()
        {
            WriteVerbose($"Processing [{startedTasks.Count}] running queries");
            foreach (AsyncAction query in startedTasks)
            {
                WriteDebug("Waiting for a query to finish");
                query.Wait();
            }
        }
    }
}
