using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Remove issue by Key
    /// </summary>
    /// <notes>
    /// </notes>
    [Alias("Remove-Issue")]
    [Cmdlet(VerbsCommon.Remove, "JIssue", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class RemoveIssue : AsyncActionCmdlet
    {
        List<AsyncResult> startedTasks = new List<AsyncResult>();

        [Alias("ID", "JiraID")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "IssueID"
        )]
        public string[] Key { get; set; }

        /// <summary>
        /// Provides a mapping for an existing issue
        /// </summary>
        [Alias("Issue")]
        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            WriteDebug($"ParameterSetName [{ParameterSetName}]");
            switch (ParameterSetName)
            {
                case "InputObject":
                    DeleteIssue(InputObject.Key.ToString());
                    break;

                default:
                    foreach(string node in Key)
                    {
                        WriteVerbose("Removing issue by ID");
                        DeleteIssue(node);
                    }
                    break;
            }
        }

        internal void DeleteIssue(string issueID)
        {
            StartAsyncTask(
                $"Removing issue [{issueID}]",
                JSession.Issues.DeleteIssueAsync(issueID)
            );
        }

        protected override void EndProcessing()
        {
            WaitAll();
        }
    }
}
