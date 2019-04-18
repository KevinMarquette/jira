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
    [Alias("Invoke-IssueTransition", "Invoke-JIssueTransition")]
    [Cmdlet(
        VerbsLifecycle.Invoke,
        "JIssueAction",
        DefaultParameterSetName = "InputObject"
    )]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class InvokeIssueAction : AsyncActionCmdlet
    {
        [Alias("ID", "JiraID")]
        [Parameter(
            Mandatory = true,
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
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        /// <summary>
        /// The workflow value for the ticket transition
        /// </summary>
        /// <value></value>
        [Alias("TransitionTo", "Transition", "ActionName", "Name")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true
        )]
        public string Action { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "InputObject":
                    TransitionIssue(InputObject,Action);
                    break;
                default:
                    TransitionIssue(Key,Action);
                    break;
            }
        }

        internal void TransitionIssue(Issue issue, string action)
        {
            StartAsyncTask(
                $"Transitioning issue [{issue.Key}] to [{action}]",
                issue.WorkflowTransitionAsync(
                    action
                )
            );
        }

        internal void TransitionIssue(string[] keys, string action)
        {
            WriteVerbose("Removing issue by Key");

            var issues = JSession.Issues.GetIssuesAsync(keys).GetAwaiter().GetResult();
            if (null == issues || issues.Count == 0)
            {
                string errorMessage = $"No issue found matching key [{string.Join(",", keys)}]";
                WriteError(
                    new ErrorRecord(
                        new JiraInvalidActionException(errorMessage),
                        errorMessage,
                        ErrorCategory.WriteError,
                        null
                    )
                );
            }
            foreach (Issue issue in issues.Values)
            {
                TransitionIssue(issue,action);
            }
        }
        protected override void EndProcessing()
        {
            WaitAll();
        }
    }
}
