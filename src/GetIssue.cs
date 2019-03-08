using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;

namespace jira
{
    [Cmdlet(VerbsCommon.Get,"Issue")]
    [OutputType(typeof(FavoriteStuff))]
    public class GetIssue : PSCmdlet
    {

        private List<string> _issueList;
        private Dictionary<string,string> _headers;

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Uri { get; set; }

        [Parameter(
            Position = 3,
            ValueFromPipelineByPropertyName = true)]
        public PSCredential Credential { get; set; }

        [Alias("Issue","Key","JiraID")]
        [Parameter(Position = 0)]
        public string ID { get; set; }

        
        [Alias("JQL")]
        [Parameter(Position = 1)]
        public string Query { get; set; }

        
        [Parameter(Position = 2)]
        public uint Count { get; set; } = 50;


        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            string token = "";
            string password = Credential.GetNetworkCredential().Password;
            string username = Credential.UserName;

            token = System.Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(
                    $"{username}:{password}"
                )
            );

            _headers = new Dictionary<string,string>()
            {
                ["Authorization"] = $"Basic {token}",
                ["Content-Type"] = "application/json"
            };

            _issueList = new List<string>();

            WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            foreach(var issueID in ID)
            {
                _issueList.Add($"\"{ID}\"");
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            if(_issueList.Count > 0)
            {
                string csvList = string.Join(",",_issueList);
                Query = "$issuekey in ({csvList})";
            }
            WriteVerbose($"Query [{Query}]");
            
            string fullUri = string.Format(
                "{0}/rest/api/latest/search?maxResults={1}&jql={2}&expand=transitions",
                Uri, Count, Query
            );
            WriteVerbose($"request [{Uri}]");
/*
            response = Invoke-WebRequest -Headers headers -Uri uri
            data = response.content | ConvertFrom-Json

            foreach(issue in data.issues)
            {
                issue.PSObject.TypeNames.Insert(0,"LD.JiraIssue")
                issue
            } */
            WriteVerbose("End!");
        }
    }

    public class FavoriteStuff
    {
        public int FavoriteNumber { get; set; }
        public string FavoritePet { get; set; }
    }
}
