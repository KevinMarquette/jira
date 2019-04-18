using System;

namespace JiraModule
{
    public class JiraConnectionException : JiraModuleException
    {
        public JiraConnectionException()
            : base("No Jira connection available. Use Open-JSession to establish a Jira conneciton to a endpoint with a credential") {}

        public JiraConnectionException(string message)
            : base(message) {}

        public JiraConnectionException(string message, Exception inner)
            : base(message, inner) {}
    }
}
