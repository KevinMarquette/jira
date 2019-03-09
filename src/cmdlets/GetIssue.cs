using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;
using System.Threading.Tasks;

namespace JiraModule
{
    [Cmdlet(VerbsCommon.Get,"Issue")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    public class GetIssue : JiraCmdlet
    {
        
        [Alias("Issue","Key","JiraID")]
        [Parameter(Position = 0)]
        public string[] ID { get; set; }


        [Parameter()]
        public SwitchParameter Async {get;set;} = false;

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
                var jiraTask = JiraApi.Issues.GetIssuesAsync(ID);
                WriteTaskObject( 
                    jiraTask, Async, r => {return r.Values;}
                );
            }
        }


        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}
