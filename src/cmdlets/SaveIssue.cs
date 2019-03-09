using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;
using System.Threading.Tasks;

namespace JiraModule
{
    [Cmdlet(VerbsData.Save,"Issue")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    public class SaveIssue : JiraCmdlet
    {

        private List<Task<Issue>> _tasks = new List<Task<Issue>>();

        [Alias("JiraIssue")]
        [Parameter(
            Position = 0,
            ValueFromPipeline=true
        )]
        public Atlassian.Jira.Issue Issue { get; set; }

        [Parameter()]
        public SwitchParameter Async {get;set;} = false;
        [Parameter()]
        public SwitchParameter PassThru {get;set;} = false;

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing()
        {
            WriteDebug("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if(Async)
            {
                _tasks.Add(Issue.SaveChangesAsync());
            }
            else
            {
                Issue.SaveChanges();
                if(PassThru)
                {
                    WriteObject(Issue);
                }
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            Task.WaitAll(_tasks.ToArray());
            if(PassThru)
            {
                _tasks.ForEach(i=> WriteObject(i.Result));
            }
            WriteVerbose("End!");
        }
    }
}
