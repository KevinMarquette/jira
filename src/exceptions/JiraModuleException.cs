using System;

namespace JiraModule
{
    public class JiraModuleException : InvalidOperationException
    {
        public JiraModuleException() 
            : base("Jira Module had an issue completing the opperation") {}

        public JiraModuleException(string message)
            : base(message) {}
        
        public JiraModuleException(string message, Exception inner)
            : base(message, inner) {}
        
    }
}