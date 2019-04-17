using System;
using System.Management.Automation;
using Atlassian.Jira;

namespace JiraModule
{
    [Cmdlet(VerbsCommon.Get, "JSession")]
    public class GetSession : PSCmdlet
    {
        protected override void EndProcessing()
        {
            var session = new JSession();
            if (session.IsConnected)
            {
                WriteObject(session);
            }
            else
            {
                WriteVerbose("No Jira session is available.");
            }
        }
    }
}
