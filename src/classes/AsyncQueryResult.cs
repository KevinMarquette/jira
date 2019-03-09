using System;
using System.Threading.Tasks;

namespace JiraModule
{
    public class AsyncQueryResult
    {
        dynamic _task;
        TaskResultTransform _resultTransform;

        public int Id 
            => _task.Id;
        
        public bool IsCompleted 
            => _task.IsCompleted;

        public TaskStatus Status 
            => _task.Status;
        
        public AggregateException Exception 
            => _task.Exception;
        

        public override string ToString()
            => $"AsyncQueryResult[{Status}]";
        
        public AsyncQueryResult (dynamic task, TaskResultTransform resultTransform)
        {
            _task = task;
            _resultTransform = resultTransform;
        }

        public dynamic GetResult()
        {
            var taskResult = _task.GetAwaiter().GetResult();
            var transformed = _resultTransform(taskResult);
            return transformed;
        }

        /// Using getter so PowerShell does not try to resolve the task properties
        public dynamic GetTask() => _task;
        
    }
}