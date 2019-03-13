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
    /// Gets Jira Issue by ID
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Cmdlet(VerbsCommon.Add, "Comment", DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    [OutputType(typeof(JiraModule.AsyncResult))]
    public class AddComment : JiraCmdlet
    {
        List<AsyncAction> startedTasks = new List<AsyncAction>();

        [Alias("Key", "JiraID")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            ParameterSetName = "IssueID"
        )]
        public string[] ID { get; set; }

        /// <summary>
        /// Provides a mapping for an existing issue
        /// </summary>
        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        [Parameter(
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true
        )]
        public string Comment { get; set; }

        [Parameter()]
        public SwitchParameter Async { get; set; } = false;

        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            if (ParameterSetName == "InputObject")
            {
                var result = new AsyncAction(
                    InputObject.AddCommentAsync(Comment)
                );
                startedTasks.Add(result);                
            }
            else
            {
                Comment comment = new Comment();
                comment.Body = Comment;
                comment.Author = Environment.UserName;
                var result = from node in ID
                             select new AsyncAction(
                                 JiraApi.Issues.AddCommentAsync(node, comment)
                             );

                startedTasks.AddRange(result);
            }
        }

        protected override void EndProcessing()
        {
            if (Async)
            {
                WriteObject(startedTasks,true);
            }
            else
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
}
