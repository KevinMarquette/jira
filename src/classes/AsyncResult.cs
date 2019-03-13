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
        public AsyncResult (dynamic task, TaskResultTransform resultTransform) : base(task as Task)
        {
            this.resultTransform = resultTransform;
        }
        public AsyncResult (dynamic task) : base(task as Task) {}

        /// <summary>
        /// Wiats for the task to finish and then return the result
        /// based on the trandform provided
        /// </summary>
        public dynamic GetResult()
        {
            var task = GetTask();
            var taskResult = task.GetAwaiter().GetResult();
            var transformed = this.resultTransform(taskResult);
            return transformed;
        }
    }
}