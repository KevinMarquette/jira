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
    [Cmdlet(
        VerbsLifecycle.Invoke,
        "IssueTransition",
        DefaultParameterSetName = "IssueID"
    )]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class StepIssueTransition : JiraCmdlet
    {
        List<AsyncAction> startedTasks = new List<AsyncAction>();

        [Alias("Key", "JiraID")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "IssueID"
        )]
        public string[] ID { get; set; }

        /// <summary>
        /// Provides a mapping for an existing issue
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        /// <summary>
        /// The workflow value for the ticket transition
        /// </summary>
        /// <value></value>
        [Alias("Action","Target")]
        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true
        )]
        public string TransitionTo { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            string message = "";
            if (ParameterSetName == "IssueID")
            {
                // make this more async
                var issues = JiraApi.Issues.GetIssuesAsync(ID).GetAwaiter().GetResult();
                foreach (Issue issue in issues.Values)
                {
                    message = $"Transitioning issue [{issue.Key}] to [{TransitionTo}]";
                    WriteVerbose(message);

                    startedTasks.Add(
                        new AsyncAction(
                            message,
                            issue.WorkflowTransitionAsync(
                                TransitionTo
                            )
                        )
                    );
                }
            }
            else
            {
                message = $"Transitioning issue [{InputObject.Key}] to [{TransitionTo}]";
                WriteVerbose(message);

                startedTasks.Add(
                    new AsyncAction(
                        message,
                        InputObject.WorkflowTransitionAsync(
                            TransitionTo
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
