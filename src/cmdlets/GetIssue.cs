using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;
using System.Threading.Tasks;

namespace JiraModule
{
    /// <summary>
    /// Gets Jira Issue by ID
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Cmdlet(VerbsCommon.Get, "Issue", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    public class GetIssue : JiraCmdlet
    {
        Queue<AsyncQueryResult> startedTasks = new Queue<AsyncQueryResult>();

        [Alias("ID", "Key", "JiraID")]
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

        [Alias("JQL")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ParameterSetName = "Query",
            ValueFromPipelineByPropertyName = true
        )]
        public string Query { get; set; }

        [Alias("Count")]
        [Parameter(
            Position = 1,
            ParameterSetName = "Query",
            ValueFromPipelineByPropertyName = true
        )]
        public int MaxResults { get; set; } = 50;

        [Parameter(
            ParameterSetName = "Query",
            ValueFromPipelineByPropertyName = true
        )]
        public int StartAt { get; set; } = 0;

        [Parameter()]
        public SwitchParameter Async { get; set; } = false;


        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            AsyncQueryResult queryResult = null;
            switch (ParameterSetName)
            {
                case "InputObject":
                    string issueID = InputObject.Key.ToString();
                    WriteVerbose("Starting query for [{issueID}] from InputObject");
                    var task = JiraApi.Issues.GetIssueAsync(issueID);
                    queryResult = new AsyncQueryResult(task, r => { return r; });
                    break;

                case "Query":
                    WriteVerbose($"Starting JQL query [{Query}]");
                    var queryTask = JiraApi.Issues.GetIssuesFromJqlAsync(Query, MaxResults, StartAt);
                    queryResult = new AsyncQueryResult(queryTask, r => { return r; });
                    break;

                default:
                    WriteVerbose($"Starting query for [{ID}]");
                    var jiraTask = JiraApi.Issues.GetIssuesAsync(ID);
                    queryResult = new AsyncQueryResult(jiraTask, r => { return r.Values; });
                    break;
            }

            if (Async)
            {
                WriteObject(queryResult);
            }
            else
            {
                WriteDebug("Queueing running queries");
                startedTasks.Enqueue(queryResult);
            }
        }

        protected override void EndProcessing()
        {
            if (!Async)
            {
                WriteVerbose($"Processing [{startedTasks.Count}] running queries");
                foreach (AsyncQueryResult query in startedTasks)
                {
                    WriteDebug("Waiting for a query to finish");
                    WriteObject(query.GetResult(), true);
                }
            }
        }
    }
}
