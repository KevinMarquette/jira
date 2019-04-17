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
    /// Gets Jira Issue by Key
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Alias("Add-Comment")]
    [Cmdlet(VerbsCommon.Add, "JComment", DefaultParameterSetName = "JiraID")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class AddComment : PSCmdlet
    {
        List<AsyncAction> startedTasks = new List<AsyncAction>();

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
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipelineByPropertyName = true
        )]
        public string Comment { get; set; }

        // This method will be called for each input received from the
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if (ParameterSetName == "InputObject")
            {
                var result = new AsyncAction(
                    $"Add comment to issue [{InputObject.Key}]",
                    InputObject.AddCommentAsync(Comment)
                );
                startedTasks.Add(result);
            }
            else
            {
                Comment comment = new Comment();
                comment.Body = Comment;
                comment.Author = Environment.UserName;
                var result = from node in Key
                             select new AsyncAction(
                                 $"Add comment to issue [{node}]",
                                 JSession.Issues.AddCommentAsync(node, comment)
                             );

                startedTasks.AddRange(result);
            }
        }

        protected override void EndProcessing()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (var result in startedTasks)
            {
                WriteDebug("Waiting for an async result to finish");
                result.Wait();
            }
        }
    }
}
