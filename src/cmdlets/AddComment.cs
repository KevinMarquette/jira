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
    public class AddComment : AsyncActionCmdlet
    {
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
            switch (ParameterSetName)
            {
                case "InputObject":
                    AddIssueComment(InputObject,Comment);
                    break;
                default:
                    AddIssueComment(Key,Comment);
                    break;
            }
        }

        internal void AddIssueComment(Issue issue, string comment)
        {
            StartAsyncTask(
                $"Add comment to issue [{issue.Key}]",
                issue.AddCommentAsync(comment)
            );
        }
        internal void AddIssueComment(string[] keys, string comment)
        {
            Comment jiraComment = new Comment();
            jiraComment.Body = Comment;
            jiraComment.Author = Environment.UserName;

            foreach(var key in keys)
            {
                StartAsyncTask(
                    $"Add comment to issue [{key}]",
                    JSession.Issues.AddCommentAsync(key, jiraComment)
                );
            }
        }

        protected override void EndProcessing()
        {
            WaitAll();
        }
    }
}
