using System;
using System.Threading.Tasks;

namespace JiraModule
{
    public class AsyncResult : AsyncAction
    {
        TaskResultTransform resultTransform = 
            result => { return result; };
        

        public override string ToString()
            => $"AsyncResult[{Status}]";
        
        /// <summary>
        /// Create a new instance of a 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="resultTransform"></param>
        public AsyncResult (
            string description,
            dynamic task, 
            TaskResultTransform resultTransform 
        ) : base(description, task as Task)
        {
            this.resultTransform = resultTransform;
        }
        public AsyncResult (
            string description,
            dynamic task
        ) : base(description, task as Task) {}

        /// <summary>
        /// Wiats for the task to finish and then return the result
        /// based on the trandform provided
        /// </summary>
        public dynamic GetResult()
        {
            Func<dynamic> action = () 
                => GetTask().GetAwaiter().GetResult();
                
            var taskResult = JiraModuleException.Try(Description, action);
            
            var transformed = this.resultTransform(taskResult);
            return transformed;
        }
    }
}