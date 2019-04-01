using System;
using System.Management.Automation;

namespace JiraModule
{
    [Cmdlet(VerbsCommon.Close, "JSession")]
    public class CloseSession : JiraCmdlet
    {
        /// <summary>
        /// Session to close
        /// </summary>
        /// <value>A current session</value>
        /// <notes>For illusion of pipeline support, not actually used</notes>
        [Parameter(
            Position = 0,
            ValueFromPipeline = true
        )]
        public JSession Session { get; set; }

        protected override void EndProcessing()
        {
            JSession.Close();
        }
    }
}
