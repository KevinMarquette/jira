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
    /// Gets Jira Issue by ID
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Cmdlet(VerbsLifecycle.Invoke, "IssueTransition", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class StepIssueTransition : JiraCmdlet
    {
        List<AsyncAction> startedTasks = new List<AsyncAction>();

        [Alias("Key", "JiraID")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
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

        [Parameter()]
        public string Action {get;set;}

        [Parameter()]
        public SwitchParameter Async { get; set; } = false;

        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if(ParameterSetName == "InputObject")
            {
                var update = new WorkflowTransitionUpdates();
                // add comment to update
                
                string issueID = InputObject.Key.ToString();
                var result = new AsyncAction(
                    jiraApi.Issues.ExecuteWorkflowActionAsync(
                        InputObject,
                        Action,
                        update
                    )
                );
                
                startedTasks.Add(result);
            }
            else
            {
                var results = from node in ID
                    select new AsyncAction(
                        jiraApi.Issues.DeleteIssueAsync(node)
                    );
                
                startedTasks.AddRange(results);
            }
        }

        protected override void EndProcessing()
        {
            if (Async)
            {
                WriteObject(startedTasks,true);
            }
            else
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
}
