using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Gets Jira Issue by Key or Query
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Alias("Get-Issue")]
    [Cmdlet(VerbsCommon.Get, "JIssue", DefaultParameterSetName = "IssueID")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class GetIssue : JiraCmdlet
    {
        Queue<AsyncResult> startedTasks = new Queue<AsyncResult>();

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
            AsyncResult queryResult = null;
            string message = "";

            switch (ParameterSetName)
            {
                case "InputObject":
                    string issueID = InputObject.Key.ToString();
                    message = $"Starting query for [{issueID}] from InputObject";
                    WriteVerbose(message);
                    var task = JSession.Issues.GetIssueAsync(issueID);
                    queryResult = new AsyncResult(message,task);
                    break;

                case "Query":
                    message = $"Starting JQL query [{Query}]";
                    WriteVerbose(message);
                    var queryTask = JSession.Issues.GetIssuesFromJqlAsync(Query, MaxResults, StartAt);
                    queryResult = new AsyncResult(message,queryTask);
                    break;

                default:
                    message = $"Starting query for [{Key}]";
                    WriteVerbose(message);
                    var jiraTask = JSession.Issues.GetIssuesAsync(Key);
                    queryResult = new AsyncResult(
                        message,
                        jiraTask,
                        result => { return result.Values; }
                    );
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
                WriteDebug($"Processing [{startedTasks.Count}] running queries");
                foreach (AsyncResult result in startedTasks)
                {
                    WriteDebug("Waiting for an async result to finish");
                    WriteObject(result.GetResult(), true);
                }
            }
        }
    }
}
