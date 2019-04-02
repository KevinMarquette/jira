using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Transitions a jira issue to a new status
    /// </summary>
    [Alias("Get-JIssueTransition")]
    [Cmdlet(
        VerbsCommon.Get,
        "JIssueAction",
        DefaultParameterSetName = "InputObject"
    )]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class GetIssueAction : JiraCmdlet
    {
        List<AsyncResult> startedTasks = new List<AsyncResult>();

        [Alias("ID", "JiraID")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "IssueID"
        )]
        public string[] Key { get; set; }

        /// <summary>
        /// Provides a mapping for an existing issue
        /// </summary>
        [Alias("Issue")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose($"ParameterSetName [{ParameterSetName}]");

            switch (ParameterSetName)
            {
                case "InputObject":
                    QueueAvailableActions(InputObject);
                    break;

                default:
                    ProcessIssueKey();
                    break;
            }
        }

        internal void QueueAvailableActions(Issue issue)
        {
            string message = $"Getting actions for [{issue.Key}]";
            WriteVerbose(message);

            startedTasks.Add(
                new AsyncResult(
                    message,
                    issue.GetAvailableActionsAsync()
                )
            );
        }

        internal void ProcessIssueKey()
        {
            WriteVerbose("Getting actions");
            var issues = new AsyncResult(
                "Querying for issues",
                JSession.Issues.GetIssuesAsync(Key),
                result => { return result.Values; }
            ).GetResult();

            if(null == issues || issues.Count == 0)
            {
                throw new JiraInvalidActionException(
                    "No issues exist matching that issue key"
                );
            }

            foreach (Issue issue in issues)
            {
                QueueAvailableActions(issue);
            }
        }

        protected override void EndProcessing()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (var task in startedTasks)
            {
                WriteDebug("Waiting for an async result to finish");
                foreach(IssueTransition result in task.GetResult())
                {
                    WriteObject(new Models.JiraIssueAction(result), true);
                }
            }
        }
    }
}
