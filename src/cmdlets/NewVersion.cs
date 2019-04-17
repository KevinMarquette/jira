using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Gets Jira Issue by Key or Query
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Cmdlet(VerbsCommon.New, "JVersion")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class NewVersion : Cmdlet
    {
        Queue<AsyncResult> startedTasks = new Queue<AsyncResult>();

        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true

        )]
        public string Project { get; set; }

        [Alias("Version","FixVersion")]
        [Parameter(
            Mandatory = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Name { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            AsyncResult queryResult = null;

            var creationInfo = new ProjectVersionCreationInfo(Name);
            creationInfo.ProjectKey = Project;

            string message = $"Starting query for Version by Project [{Project}]";
            WriteVerbose(message);
            queryResult = new AsyncResult(
                message,
                JSession.Api.Versions.CreateVersionAsync(creationInfo)
            );

            startedTasks.Enqueue(queryResult);
        }

        protected override void EndProcessing()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (AsyncResult result in startedTasks)
            {
                WriteDebug("Waiting for an async result to finish");
                WriteObject(result.GetResult(), true);
            }
        }
    }
}
