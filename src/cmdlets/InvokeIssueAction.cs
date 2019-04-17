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
    [Alias("Invoke-IssueTransition","Invoke-JIssueTransition")]
    [Cmdlet(
        VerbsLifecycle.Invoke,
        "JIssueAction",
        DefaultParameterSetName = "InputObject"
    )]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class InvokeIssueAction : PSCmdlet
    {
        List<AsyncAction> startedTasks = new List<AsyncAction>();

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
        [Alias("TransitionTo","Transition","ActionName","Name")]
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
            string message = "";
            if (ParameterSetName == "IssueID")
            {
                // make this more async
                var issues = JSession.Issues.GetIssuesAsync(Key).GetAwaiter().GetResult();
                if(null == issues || issues.Count == 0)
                {
                    throw new JiraInvalidActionException($"No issue found matching key [{Key}]");
                }
                foreach (Issue issue in issues.Values)
                {
                    message = $"Transitioning issue [{issue.Key}] to [{Action}]";
                    WriteVerbose(message);

                    startedTasks.Add(
                        new AsyncAction(
                            message,
                            issue.WorkflowTransitionAsync(
                                Action
                            )
                        )
                    );
                }
            }
            else
            {
                message = $"Transitioning issue [{InputObject.Key}] to [{Action}]";
                WriteVerbose(message);

                startedTasks.Add(
                    new AsyncAction(
                        message,
                        InputObject.WorkflowTransitionAsync(
                            Action
                        )
                    )
                );
            }
        }

        protected override void EndProcessing()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (var result in startedTasks)
            {
                WriteDebug("Waiting for an async result to finish");
                result.Wait();
            }
        }
    }
}
