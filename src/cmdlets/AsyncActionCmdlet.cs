using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Atlassian.Jira;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JiraModule
{
    /// <summary>
    /// Base class for all CmdLets that make Jira Calls
    /// </summary>

    public class AsyncActionCmdlet : PSCmdlet
    {
        Queue<AsyncAction> startedTasks = new Queue<AsyncAction>();

        internal void StartAsyncTask(string message, Task task)
        {
            WriteVerbose(message);
            var result = new AsyncAction(
                message,
                Task<Issue>.Run(async () => await task.ConfigureAwait(false))
            );

            startedTasks.Enqueue(result);
        }

        internal void WaitAll()
        {
            WriteDebug($"Processing [{startedTasks.Count}] running queries");
            foreach (AsyncAction query in startedTasks)
            {
                try
                {
                    query.Wait();
                }
                catch (Exception ex)
                {
                    WriteError(
                        new ErrorRecord(
                            ex,query.Description,ErrorCategory.WriteError,null
                        )
                    );
                }
            }
        }
    }


}
