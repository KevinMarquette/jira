using System.Collections.Generic;
using Atlassian.Jira;

namespace JiraModule.Models
{
    public class JiraIssueAction
    {
        protected IssueTransition issueAction = null;

        /// <summary>
        /// The Jira Action that will perfrom this transition
        /// </summary>
        public string Action {get => issueAction.Name;}

        /// <summary>
        /// Name of the resulting status
        /// </summary>
        public string To {get => issueAction.To.Name;}

        /// <summary>
        /// Transition ID
        /// </summary>
        public string Id {get => issueAction.Id;}

        public JiraIssueAction(IssueTransition issueAction)
        {
            this.issueAction = issueAction;
        }

        public override string ToString() => Action;
    }
}
