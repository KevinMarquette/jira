using System;

namespace JiraModule
{
    public class JiraInvalidActionException : JiraModuleException
    {
        public JiraInvalidActionException() 
            : base("The specified action is either not valid or not available for the current state.") {}

        public JiraInvalidActionException(string message)
            : base(message) {}

        public JiraInvalidActionException(string message, Exception inner)
            : base(message, inner) {}
    }
}