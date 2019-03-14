using System;
using System.Collections.Generic;
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
    [Cmdlet(VerbsCommon.New, "Issue", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class NewIssue : JiraCmdlet
    {
        Queue<AsyncResult> startedTasks = new Queue<AsyncResult>();

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Project {get;set;}

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Assignee {get;set;}

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Description {get;set;}
        
        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Reporter {get;set;}

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Summary {get;set;}

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Priority {get;set;}

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Type {get;set;}

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string ParentIssueKey {get;set;}

        [Parameter()]
        public SwitchParameter Async { get; set; } = false;


        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            var issue = new Issue(JiraApi,Project,ParentIssueKey);
            if( null != Assignee )
            {
                issue.Assignee = Assignee;
            }
            if( null != Description )
            {
                issue.Description = Description;
            }
            if( null != Reporter )
            {
                issue.Reporter = Reporter;
            }
            if( null != Summary )
            {
                issue.Summary = Summary;
            }
            if( null != Priority )
            {
                issue.Priority = Priority;
            }
            if( null != Type )
            {
                issue.Type = Type;
            }
            var result = new AsyncResult(
                "Create new issue",
                JiraApi.Issues.CreateIssueAsync(issue)
            );

            if (Async)
            {
                WriteObject(result);
            }
            else
            {
                WriteDebug("Queueing running queries");
                startedTasks.Enqueue(result);
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
