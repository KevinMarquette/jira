using System;
using System.Threading.Tasks;

namespace JiraModule
{
    public class AsyncAction
    {
        dynamic task;
        TaskResultTransform resultTransform = 
            result => { return result; };
        
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
        public dynamic GetTask() 
            => this.task;
        

        public override string ToString()
            => $"AsyncResult[{Status}]";
        
        public AsyncAction (dynamic task)
        {
            this.task = task;
        }

        /// <summary>
        /// Wait for the task to complete
        /// </summary>
        public void Wait()
        {
            this.task.GetAwaiter().GetResult();
        }
    }
}