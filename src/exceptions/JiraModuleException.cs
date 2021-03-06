using System;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Json.Net;
using JiraModule.Models;
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

        static public void Try(string message, Action action )
        {
            Try(message, () => { action(); return 0; });
        }

        static public T Try<T>(string message, Func<T> action)
        {
            try
            {
                return action();
            }
            catch (AuthenticationException ex)
            {
                string exMessage = $"Error with {message}: Please verify the credentials are correct and you have authorization to access this Jira endpoint";
                throw new JiraAuthenticationException(exMessage, ex);
            }
            catch (InvalidOperationException ex)
            {
                string exMessage = CreateExceptionMessage(message,ex);
                throw new JiraInvalidActionException(exMessage, ex);
            }
            catch (Exception ex)
            {
                string exMessage = CreateExceptionMessage(message,ex);
                throw new JiraModuleException(exMessage, ex);
            }
        }

        public static string CreateExceptionMessage(string message, Exception exception)
        {
            string exMessage = $"{exception.GetType()} with {message}:";

            // Some exception messages include raw HTML from the remote endpoint
            var htmlCheck = new Regex("html|doctype",RegexOptions.IgnoreCase);
            if(htmlCheck.IsMatch(exception.Message))
            {
                exMessage += " Check the inner exception for the response from server. ";
            }
            else
            {
                var jsonResponseCode = new Regex(@"Response Status Code: (?<code>\d+)",RegexOptions.IgnoreCase);
                var jsonResponseContent = new Regex(@"Response Content: (?<json>\{.*\})",RegexOptions.IgnoreCase);

                // if we have a valid json structured error, clean it up and use it
                Match jsonMatch = jsonResponseContent.Match(exception.Message);
                if(jsonMatch.Success)
                {
                    string json = jsonMatch.Groups["json"].Value;
                    JiraStandardError errorInfo = null;
                    try
                    {
                        errorInfo = JsonNet.Deserialize<JiraStandardError>(json);
                    }
                    catch (Exception ex)
                    {
                        throw new JiraModuleException($"Issue parsing Json in message [{exception.Message}",ex);
                    }

                    if (errorInfo.errorMessages?.Length > 0)
                    {
                        exMessage += $" Message [{errorInfo.errorMessages[0]}]" ;
                    }

                    foreach(string key in errorInfo.errors.Keys)
                    {
                        exMessage += $" [{errorInfo.errors[key]}]";
                    }
                }
                else
                {
                    exMessage += $" Message [{exception.Message}]";
                }
            }
            return exMessage;
        }
    }
}
