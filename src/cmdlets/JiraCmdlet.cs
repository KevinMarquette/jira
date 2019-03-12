using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Atlassian.Jira;

namespace JiraModule
{
    /// <summary>
    /// Base class for all CmdLets that make Jira Calls
    /// </summary>
    public class JiraCmdlet : PSCmdlet
    {
        /// <summary>
        /// This is set by calls to Open-JiraSession
        /// </summary>
        protected static Jira jiraApi = null;
        protected Jira JiraApi
        {
            get
            {
                if (null == jiraApi)
                {
                    ThrowTerminatingError(
                        new ErrorRecord(
                            new JiraConnectionException(),
                            "TestConnectionException",
                            ErrorCategory.ConnectionError,
                            null
                        )
                    );
                }
                return jiraApi;
            }
        }
    }
}