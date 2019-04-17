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
    [Cmdlet(VerbsCommon.Get, "JVersion", DefaultParameterSetName = "Project")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class GetVersion : PSCmdlet
    {
        Queue<AsyncResult> startedTasks = new Queue<AsyncResult>();

        [Parameter(
            Mandatory = true,
            Position = 0,
            ParameterSetName = "Project"
        )]
        public string Project { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Id",
            ValueFromPipelineByPropertyName = true
        )]
        public string Id { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            AsyncResult queryResult = null;
            string message = "";

            switch (ParameterSetName)
            {
                case "Name":
                    throw new NotImplementedException("Version by ID");
                    message = $"Starting query for Version by ID[{Id}] ";
                    WriteVerbose(message);
                    queryResult = new AsyncResult(
                        message,
                        JSession.Api.Versions.GetVersionAsync(Id)
                    );
                    break;

                case "Project":
                    message = $"Starting query for Version by Project [{Project}]";
                    WriteVerbose(message);
                    queryResult = new AsyncResult(
                        message,
                        JSession.Api.Versions.GetVersionsAsync(Project)
                    );
                    break;
            }

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
