using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Set issue properties
    /// </summary>
    /// <notes>
    /// </notes>
    [Alias("Set-Issue")]
    [Cmdlet(VerbsCommon.Set, "JIssue", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class SetIssue : JiraCmdlet
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
        [Alias("Issue")]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        [Parameter(
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
            ValueFromPipelineByPropertyName = true
        )]
        public string Summary { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Priority { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string Type { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public string ParentIssueKey { get; set; }

        [Parameter(
            ValueFromPipelineByPropertyName = true
        )]
        public IDictionary CustomField { get; set; }
        public bool PassThru { get; set; } = false;
        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteVerbose($"ParameterSetName [{ParameterSetName}]");

            switch (ParameterSetName)
            {
                case "InputObject":
                    SaveChanges(InputObject);
                    break;

                default:
                    ProcessIssueKey();
                    break;
            }
        }

        protected void SaveChanges(Issue issue)
        {
            SetIssueProperties(issue);

            string message = $"Saving [{issue.Key}]";
            WriteVerbose(message);

            startedTasks.Add(
                new AsyncResult(
                    message,
                    issue.SaveChangesAsync()
                )
            );
        }


        protected void ProcessIssueKey()
        {
            WriteVerbose("Updating issue");
            var issues = new AsyncResult(
                "Querying for issues",
                JSession.Issues.GetIssuesAsync(Key),
                result => { return result.Values; }
            ).GetResult();

            if(null == issues || issues.Count == 0)
            {
                throw new JiraInvalidActionException(
                    "No issues exist matching that issue key"
                );
            }

            foreach (Issue issue in issues)
            {
                SaveChanges(issue);
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
            if (null != CustomField && CustomField.Count > 0)
            {
                foreach (string key in CustomField.Keys)
                {
                    issue[key] = CustomField[key]?.ToString();
                }
            }
            return issue;
        }

        protected override void EndProcessing()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (AsyncResult result in startedTasks)
            {
                WriteDebug("Waiting for an async result to finish");
                if (PassThru)
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
