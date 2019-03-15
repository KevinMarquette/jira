Describe "CmdLet Open-JiraSession" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
    }

    It "Invalid URI should throw" {
        { 
            Open-JiraSession -Credential $Credential -Uri "INVALID_URI" 
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Invalid Hostname should throw" {
        { 
            Open-JiraSession -Credential $Credential -Uri "HTTP://MISSINGHOST" 
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Unresponsive host should throw" {
        { 
            Open-JiraSession -Credential $Credential -Uri "https://localhost" 
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Live but invalid endpoint should throw" {
        { 
            Open-JiraSession -Credential $Credential -Uri "https://devtools.ld.corp.local" 
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }
    
    It "Bad Credential should throw" {
        $password = "mypassword" | ConvertTo-SecureString -asPlainText -Force
        $username = "joedirt" 
        $badCredential = New-Object System.Management.Automation.PSCredential($username,$password)
        { 
            Open-JiraSession -Credential $badCredential -Uri $JiraUri 
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Establishes a connection to Jira endpoint" {
        Open-JiraSession -Credential $Credential -Uri $JiraUri
    }
}