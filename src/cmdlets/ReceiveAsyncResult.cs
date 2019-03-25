using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JiraModule
{
    /// <summary>
    /// <para type="synopsis">Waits for and unwrapps an Async result</para>
    /// <summary>
    [Alias("Wait-AsyncResult", "Wait-JiraResult", "Receive-JiraResult","Receive-AsyncResult")]
    [Cmdlet(
        VerbsCommunications.Receive,
        "JAsyncResult",
        DefaultParameterSetName = "AsyncResult"
    )]
    public class ReceiveResult : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "AsyncAction"
        )]
        public AsyncAction AsyncAction { get; set; }

        [Alias("Result", "AsyncResult")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ParameterSetName = "AsyncResult"
        )]
        public AsyncResult InputObject { get; set; }

        protected override void ProcessRecord()
        {
            switch (ParameterSetName)
            {
                case "AsyncResult":
                    WriteObject(InputObject.GetResult(), true);
                    break;

                default:
                    AsyncAction.Wait();
                    break;
            }
        }
    }
}
