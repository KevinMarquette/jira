Describe "function Invoke-IssueTransition" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"

        Open-JiraSession -Credential $Credential -Uri $JiraUri
    }

    BeforeEach {
        $issue = Get-Issue -ID $Ticket
        if("In Progress" -eq $issue.Status)
        {
            $issue | Invoke-IssueTransition -TransitionTo "Open"
        }
    }

    It "Transition by ID" {
        
        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        Invoke-IssueTransition -ID $ticket -TransitionTo "In Progress"
        $issue.Refresh()
        $issue.Status | Should -Be 'In Progress'
        Invoke-IssueTransition -ID $ticket -TransitionTo "Open"
        $issue.Refresh()
        $issue.Status | Should -Be 'Open'
    }

    It "Transition by object" {
        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        $issue | Invoke-IssueTransition -TransitionTo "In Progress"
        $issue.Status | Should -Be 'In Progress'
        $issue | Invoke-IssueTransition -TransitionTo "Open"
        $issue.Status | Should -Be 'Open'       
    }

    It "Transition invalid ticket should throw" {
        {
            Invoke-IssueTransition -ID $MissingTicket -TransitionTo "In Progress"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])   
    }

    It "Transition invalid status should throw" {
        {
            Invoke-IssueTransition -ID $Ticket -TransitionTo "INVALID_STATUS"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])   
    }

    It "Transition invalid status should throw" {
        {
            $issue | Invoke-IssueTransition -TransitionTo "INVALID_STATUS"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])   
    }
}