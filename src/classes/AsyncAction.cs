using System;
using System.Threading.Tasks;

namespace JiraModule
{
    public class AsyncAction
    {
        dynamic task;

        public string Description {get;set;}
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
            => $"AsyncAction[{Status}]";
        
        public AsyncAction (string description, dynamic task)
        {
            this.task = task;
            this.Description = description;
        }

        /// <summary>
        /// Wait for the task to complete
        /// </summary>
        public void Wait()
        {
            Action action = () 
                => this.task.GetAwaiter().GetResult();

            JiraModuleException.Try(Description, action);            
        }
    }
}