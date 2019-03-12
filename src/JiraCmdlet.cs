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

        /// <summary>
        /// Takes a Async Task and either waits for the result 
        /// or wraps it in a PowerShell friendly AsyncQueryResult
        /// </summary>
        /// <param name="jiraTask">The async task</param>
        /// <param name="async">flag to indicate if the opperation should stay async</param>
        /// <param name="resultTransform">function to modify the task result for the client</param>
        /// <notes>
        /// PowerShell does not play nicely with C# tasks.
        /// We need to box them in a PowerShell friendly object to prevent Results
        /// from getting access by PowerShell
        /// There are also issues unboxing the Task.Result and 
        /// the resultTransform helps with that
        /// </notes>
        protected void WriteTaskObject(
            dynamic jiraTask,
            bool async,
            TaskResultTransform resultTransform
        )
        {
            if (async)
            {
                // use this if looping is causing thread locks
                //var jiraTask = Task.Run( async () => await (JiraApi.Issues.GetIssuesAsync(ID).ConfigureAwait(false)) );
                AsyncResult asyncQueryResult = new AsyncResult(jiraTask, resultTransform);
                WriteObject(asyncQueryResult);
            }
            else
            {
                var taskResult = jiraTask.GetAwaiter().GetResult();
                var transformed = resultTransform(taskResult);
                WriteObject(transformed, true);
            }
        }
    }
}