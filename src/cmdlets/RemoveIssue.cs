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
    /// Remove issue by Key
    /// </summary>
    /// <notes>
    /// </notes>
    [Alias("Remove-Issue")]
    [Cmdlet(VerbsCommon.Remove, "JIssue", DefaultParameterSetName = "IssueID")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class RemoveIssue : JiraCmdlet
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
            if(ParameterSetName == "InputObject")
            {
                string message = $"Removing issue [{InputObject.Key}]";
                WriteVerbose(message);

                string issueID = InputObject.Key.ToString();
                startedTasks.Add(
                    new AsyncResult(
                        message,
                        jiraApi.Issues.DeleteIssueAsync(issueID)
                    )
                );
            }
            else
            {
                var results = from node in Key
                    select new AsyncResult(
                        $"Removing issue [{InputObject.Key}]",
                        jiraApi.Issues.DeleteIssueAsync(node)
                    );

                startedTasks.AddRange(results);
            }
        }

        protected override void EndProcessing()
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
