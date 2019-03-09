using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Atlassian.Jira;

namespace JiraModule 
{
    public class JiraCmdlet : PSCmdlet 
    {
        private static Jira _jira;

        public Jira JiraApi
        {
            get 
            {
                if(_jira == null)
                {
                    string username = Credential.UserName;
                    string password = Credential.GetNetworkCredential().Password;
                    _jira = Jira.CreateRestClient(Uri, username, password);
                }
                return _jira;
            }
        }
        
        
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

        //protected delegate dynamic TaskAction(dynamic result);
        protected void WriteTaskObject(dynamic jiraTask,bool async,TaskResultTransform resultTransform)
        {
            if(async)
            {
                // use this if looping on the above is causing thread locks
                //var jiraTask = Task.Run( async () => await (JiraApi.Issues.GetIssuesAsync(ID).ConfigureAwait(false)) );
                AsyncQueryResult asyncQueryResult = new AsyncQueryResult(jiraTask,resultTransform);
                WriteObject(asyncQueryResult);
            }
            else
            {
                var taskResult = jiraTask.GetAwaiter().GetResult();
                var transformed = resultTransform(taskResult);
                WriteObject(transformed,true);
            }
        }
    }
}