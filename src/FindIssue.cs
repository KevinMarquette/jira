using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;

namespace JiraModule
{
    [Alias("Search-Issue", "Get-JqlIssue")]
    [Cmdlet(VerbsCommon.Find, "Issue")]
    [OutputType(typeof(FavoriteStuff))]
    public class FindIssue : PSCmdlet
    {
        private Jira _jira;
        private List<string> _issueList = new List<string>();

        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Uri { get; set; }

        [Parameter(
            Position = 2,
            ValueFromPipelineByPropertyName = true)]
        public PSCredential Credential { get; set; }

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
        public int StartAt { get; set; } = 0;

        [Parameter()]
        public SwitchParameter Async { get; set; } = false;

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteDebug("Begin!");

            string username = Credential.UserName;
            string password = Credential.GetNetworkCredential().Password;
            _jira = Jira.CreateRestClient(Uri, username, password);
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
            var results = _jira.Issues.GetIssuesFromJqlAsync(Query, MaxResults, StartAt);
            if (Async)
            {
                WriteObject(
                    results
                );
            }
            else
            {
                WriteObject(
                    results.Result
                );
            }
            WriteVerbose("End!");
        }
    }
}
