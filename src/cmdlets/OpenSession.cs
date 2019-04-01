using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    [Alias("Open-JiraSession")]
    [Cmdlet(VerbsCommon.Open, "JSession")]
    public class OpenSession : JiraCmdlet
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
            WriteVerbose($"Connection to Jira endpoint [{Uri}] with username [{Credential.UserName}]");
            CreateClient();

            if (PassThru)
            {
                WriteObject(new JSession());
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

                JSession.Open(Uri, username, password);

                WriteDebug("Issuing basic request to verify connectivity [Get Priorities]");
                new AsyncResult(
                    message,
                    JSession.Api.Priorities.GetPrioritiesAsync()
                ).Wait();
            }
            catch (Exception ex)
            {
                // clear invalid session
                JSession.Close();
                throw new JiraConnectionException(ex.Message,ex);
            }
        }
    }
}
