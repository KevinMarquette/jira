using System;
using System.Security.Authentication;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Atlassian.Jira;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JiraModule
{
    [Cmdlet(VerbsCommon.Open, "JiraSession")]
    public class JiraSession : JiraCmdlet
    {
        
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

        [Hidden()]
        [Parameter()]
        public SwitchParameter PassThru { get; set; } = false;

        protected override void EndProcessing()
        {
            WriteVerbose($"Connectiong to Jira endpoint [{Uri}] with username [{Credential.UserName}]");
            CreateClient();

            WriteDebug("Issuing basic request to verify connectivity");
            TestClientConnection();

            if (PassThru)
            {
                WriteObject(JiraApi);
            }
        }

        protected void CreateClient()
        {
            string username = Credential.UserName;
            string password = Credential.GetNetworkCredential().Password;
            JiraSession.jiraApi = Jira.CreateRestClient(Uri, username, password);
        }

        private void TestClientConnection()
        {
            try
            {
                WriteDebug("Requeting Jira priorities to test connectivity and authentication");
                var result = JiraApi.Priorities.GetPrioritiesAsync().GetAwaiter().GetResult();
            }
            catch (AuthenticationException ex)
            {
                // set the session to null because it is invalid
                JiraSession.jiraApi = null;
                string message = $"AuthenticationException with Jira Endpoint [{Uri}] for [{Credential.UserName}]: Please verify the credentials are correct and you have authorization to access this Jira endpoint";
                ThrowTerminatingError(
                    new ErrorRecord(
                        new AuthenticationException(message, ex),
                        "AuthenticationException",
                        ErrorCategory.ConnectionError,
                        null
                    )
                );
            }
            catch (Exception ex)
            {
                // set the session to null because it is invalid
                JiraSession.jiraApi = null;
                string message = $"Connectivity issues with Jira Endpoint [{Uri}] for [{Credential.UserName}].";
                
                // Some exception messages include raw HTML from the remote endpoint
                var htmlCheck = new Regex("html|doctype",RegexOptions.IgnoreCase);
                if(htmlCheck.IsMatch(ex.Message))
                {
                    message += " Check the inner exception for the response from server. ";
                }
                else
                {
                    message += $" Message [{ex.Message}]";
                }

                ThrowTerminatingError(
                    new ErrorRecord(
                        new JiraConnectionException(message, ex),
                        ex.GetType().ToString(),
                        ErrorCategory.ConnectionError,
                        null
                    )
                );
            }
        }
    }
}