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
    [Cmdlet(VerbsCommon.Set, "Issue", DefaultParameterSetName = "IssueID")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class SetIssue : JiraCmdlet
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

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Project { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Assignee { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Description { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Reporter { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Summary { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Priority { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Type { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string ParentIssueKey { get; set; }

        public bool PassThru {get;set;} = false;
        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            string message = "";
            switch (ParameterSetName)
            {
                case "InputObject":
                    SetIssueProperties(InputObject);

                    message = $"Saving [{InputObject.Key}]";
                    WriteVerbose(message);

                    startedTasks.Add(
                        new AsyncAction(
                            message,
                            InputObject.SaveChangesAsync()
                        )
                    );
                    break;

                default:
                    var issues = new AsyncResult(
                        "Querying for tickets",
                        JiraApi.Issues.GetIssuesAsync(ID)
                    ).GetResult();

                    foreach (Issue issue in issues)
                    {
                        SetIssueProperties(issue);
                        
                        message = $"Saving [{InputObject.Key}]";
                        WriteVerbose(message);

                        startedTasks.Add(
                           new AsyncAction(
                               message,
                               InputObject.SaveChangesAsync()
                           )
                       );
                    }
                    break;
            }
        }

        Issue SetIssueProperties(Issue issue)
        {
            if (null != Assignee)
            {
                issue.Assignee = Assignee;
            }
            if (null != Description)
            {
                issue.Description = Description;
            }
            if (null != Reporter)
            {
                issue.Reporter = Reporter;
            }
            if (null != Summary)
            {
                issue.Summary = Summary;
            }
            if (null != Priority)
            {
                issue.Priority = Priority;
            }
            if (null != Type)
            {
                issue.Type = Type;
            }
            return issue;
        }
        
        protected override void EndProcessing()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (AsyncResult result in startedTasks)
            {
                WriteDebug("Waiting for an async result to finish");
                if(PassThru)
                {
                    WriteObject(result.GetResult(), true);
                }
                else
                {
                    result.Wait();
                }
            }
        }
    }
}
