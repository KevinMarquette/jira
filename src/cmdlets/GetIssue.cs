using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using Atlassian.Jira.Linq;
using Atlassian.Jira;
using System.Threading.Tasks;

namespace JiraModule
{
    /// <summary>
    /// Gets Jira Issue by ID
    /// </summary>
    /// <notes>
    /// The inputObject is the DefaultParameterSetName for a better pipeline experience
    /// </notes>
    [Cmdlet(VerbsCommon.Get,"Issue",DefaultParameterSetName = "InputObject")]
    [OutputType(typeof(Atlassian.Jira.Issue))]
    public class GetIssue : JiraCmdlet
    {

        [Alias("Issue","Key","JiraID")]
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
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "InputObject"
        )]
        public Issue InputObject { get; set; }

        [Parameter()]
        public SwitchParameter Async {get;set;} = false;


        // This method will be called for each input received from the 
        //pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            switch(ParameterSetName)
            {
                case "InputObject":
                    var task = JiraApi.Issues.GetIssueAsync(InputObject.Key.ToString());
                    WriteTaskObject( 
                        task, Async, r => {return r;}
                    );
                    break;

                default:
                    var jiraTask = JiraApi.Issues.GetIssuesAsync(ID);
                    WriteTaskObject( 
                        jiraTask, Async, r => {return r.Values;}
                    );
                    break;
            }
        }
    }
}
