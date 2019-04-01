using Atlassian.Jira;

namespace JiraModule {
    /// <summary>
    /// Manages access to the Jira API and related connection information
    /// </summary>
    /// <notes>
    /// This is just a friendly wrapper around the JiraRestClient
    /// No actual connection to the server is established
    /// These sessions give the illusion of connections
    /// </notes>
    public class JSession
    {
        protected static Jira api = null;

        /// <summary>
        /// Atlassian Jira endpoint
        /// </summary>
        /// <value></value>
        internal static Jira Api
        {
            get
            {
                if (null == api)
                {
                    throw new JiraConnectionException();
                }
                return api;
            }
        }

        /// <summary>
        /// The Jira server address
        /// </summary>
        /// <value></value>
        public string Uri {
            get => api?.Url;
        }

        /// <summary>
        /// Credential used for authentication
        /// </summary>
        /// <value></value>
        public string UserName {
            get => api?.Credentials.UserName;
        }

        /// <summary>
        /// Raw JiraClient for issuing rest calls
        /// </summary>
        /// <value></value>
        public Jira JiraClient {
            get => api;
        }


        /// <summary>
        /// Opens a Jira sessions
        /// </summary>
        /// <param name="uri">Jira server</param>
        /// <param name="username">Jira username</param>
        /// <param name="password">Jira password</param>
        /// <notes>Internally creates a Jira RestClient using specified values but does not actually establish a connection.</notes>
        public static void Open(string uri, string username, string password)
        {
            api = Jira.CreateRestClient(uri, username, password);
        }

        /// <summary>
        /// Closes the Jira session
        /// </summary>
        public static void Close()
        {
            api = null;
        }

        /// <summary>
        /// Checks to see if a session exists
        /// </summary>
        /// <value>true if connected</value>
        /// <notes>if api is null, then there is no conneciton.</notes>
        public bool IsConnected {
            get
            {
                return (api != null);
            }
        }

        /// <summary>
        /// Jira Issue Client Actions
        /// </summary>
        /// <value></value>
        internal static IIssueService Issues {get => api.Issues;}
    }
}
