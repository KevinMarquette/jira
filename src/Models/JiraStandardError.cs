using System.Collections.Generic;

namespace JiraModule.Models
{
    public class JiraStandardError
    {
        public string[] errorMessages {get;set;}
        public Dictionary<string,string> errors {get;set;}
    }
}
