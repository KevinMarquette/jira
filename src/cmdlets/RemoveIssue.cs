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
    [Cmdlet(VerbsCommon.Remove, "Issue", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class RemoveIssue : JiraCmdlet
    {
        List<AsyncResult> startedTasks = new List<AsyncResult>();

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
        public SwitchParameter Async { get; set; } = false;


        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if(ParameterSetName == "InputObject")
            {
                string issueID = InputObject.Key.ToString();
                var result = new AsyncResult(
                    jiraApi.Issues.DeleteIssueAsync(issueID)
                );
                startedTasks.Add(result);
            }
            else
            {
                var results = from node in ID
                    select new AsyncResult(
                        jiraApi.Issues.DeleteIssueAsync(node)
                    );
                
                startedTasks.AddRange(results);
            }
        }

        protected override void EndProcessing()
        {
            if (!Async)
            {
                WriteDebug($"Processing [{startedTasks.Count}] running queries");
                foreach (AsyncResult result in startedTasks)
                {
                    WriteDebug("Waiting for an async result to finish");
                    result.Wait();
                }
            }
        }
    }
}
