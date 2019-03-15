using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    [Cmdlet(VerbsCommon.Open, "JiraSession")]
    public class JiraSession : JiraCmdlet
    {
        
        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true
        )]
        public PSCredential Credential { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Uri { get; set; }

        [Hidden()]
        [Parameter()]
        public SwitchParameter PassThru { get; set; } = false;

        protected override void EndProcessing()
        {
            WriteVerbose($"Connectiong to Jira endpoint [{Uri}] with username [{Credential.UserName}]");
            CreateClient();

            if (PassThru)
            {
                WriteObject(JiraApi);
            }
        }

        protected void CreateClient()
        {
            string username = Credential.UserName;
            string password = Credential.GetNetworkCredential().Password;
            
            try
            {
                string message = $"Connecting to Jira Endpoint [{Uri}] with Username [{Credential.UserName}]";
                WriteVerbose(message);

                JiraSession.jiraApi = Jira.CreateRestClient(Uri, username, password);
                
                WriteDebug("Issuing basic request to verify connectivity [Get Priorities]");
                new AsyncResult(
                    message,
                    JiraApi.Priorities.GetPrioritiesAsync()
                ).Wait();
            }
            catch (Exception ex)
            {
                // clear invalid session
                JiraSession.jiraApi = null;
                throw new JiraConnectionException(ex.Message,ex);
            }
        }
    }
}