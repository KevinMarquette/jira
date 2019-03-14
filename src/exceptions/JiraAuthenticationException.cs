using System;

namespace JiraModule
{
    public class JiraAuthenticationException : JiraConnectionException
    {
        public JiraAuthenticationException() 
            : base("unable to authenticate. Please verify the credentials are correct and you have authorization to access this Jira endpoint") {}

        public JiraAuthenticationException(string message)
            : base(message) {}

        public JiraAuthenticationException(string message, Exception inner)
            : base(message, inner) {}
    }
}