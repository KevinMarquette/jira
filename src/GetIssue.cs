using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;

namespace JiraModule
{
    [Cmdlet(VerbsCommon.Get,"Issue")]
    [OutputType(typeof(FavoriteStuff))]
    public class GetIssue : PSCmdlet
    {
        private Jira _jira;


        [Alias("Issue","Key","JiraID")]
        [Parameter(Position = 0)]
        public string[] ID { get; set; }

        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        public PSCredential Credential { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 2,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Uri { get; set; }

        [Parameter()]
        public SwitchParameter Async {get;set;} = false;

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
                var result = _jira.Issues.GetIssuesAsync(ID);
                if(Async)
                {
                    WriteObject(
                       result
                    );
                }
                else
                {
                    WriteObject(
                       result.Result.Values
                    );
                }
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}
