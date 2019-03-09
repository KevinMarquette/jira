using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;
using System.Threading.Tasks;

namespace JiraModule
{
    [Alias("Search-Issue", "Get-JqlIssue")]
    [Cmdlet(VerbsCommon.Find, "Issue")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    public class FindIssue : JiraCmdlet
    {
        private List<string> _issueList = new List<string>();

        [Alias("Issue", "Key", "JiraID")]
        [Parameter(
            Position = 0,
            ParameterSetName = "ID"
        )]
        public string[] ID { get; set; }


        [Alias("JQL")]
        [Parameter(
            Position = 0,
            ParameterSetName = "JQL"
        )]
        public string Query { get; set; }

        [Alias("Count")]
        [Parameter(Position = 2)]
        public int MaxResults { get; set; } = 50;

        [Parameter()]
        public int StartAt { get; set; } = 0;

        [Parameter()]
        public SwitchParameter Async { get; set; } = false;

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteDebug("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if (null != ID)
            {
                foreach (var issueID in ID)
                {
                    _issueList.Add($"\"{issueID}\"");
                }
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            if (_issueList.Count > 0)
            {
                string csvList = string.Join(",", _issueList);
                Query = $"issuekey in ({csvList})";
            }

            WriteVerbose($"Query [{Query}]");
            var jiraTask = JiraApi.Issues.GetIssuesFromJqlAsync(Query, MaxResults, StartAt);

            WriteTaskObject( 
                jiraTask, Async, r => {return r;}
            );
        }
    }
}
