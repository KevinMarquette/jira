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
    [Alias("Wait-AsyncResult", "Wait-JiraResult", "Receive-JiraResult")]
    [Cmdlet(VerbsCommunications.Receive, "AsyncResult")]
    public class ReceiveResult : PSCmdlet
    {
        [Alias("Result", "AsyncResult")]
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true
        )]
        public AsyncResult InputObject { get; set; }

        protected override void ProcessRecord()
        {
            var transformed = InputObject.GetResult();
            WriteObject(transformed, true);
        }
    }
}