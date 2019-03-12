using System;
using System.Threading.Tasks;

namespace JiraModule
{
    public class AsyncResult
    {
        dynamic task;
        TaskResultTransform resultTransform;

        public int Id 
            => task.Id;
        
        public bool IsCompleted 
            => task.IsCompleted;

        public TaskStatus Status 
            => task.Status;
        
        public AggregateException Exception 
            => task.Exception;
        


        /// <summary>
        /// Gets the internal Task of this object
        /// </summary>
        /// <returns>Task</returns>
        public dynamic GetTask() => this.task;
        

        public override string ToString()
            => $"AsyncQueryResult[{Status}]";
        
        /// <summary>
        /// Create a new instance of a 
        /// </summary>
        /// <param name="task"></param>
        /// <param name="resultTransform"></param>
        public AsyncResult (dynamic task, TaskResultTransform resultTransform)
        {
            this.task = task;
            this.resultTransform = resultTransform;
        }

        /// <summary>
        /// Wiats for the task to finish and then return the result
        /// based on the trandform provided
        /// </summary>
        public dynamic GetResult()
        {
            var taskResult = this.task.GetAwaiter().GetResult();
            var transformed = this.resultTransform(taskResult);
            return transformed;
        }
    }
}