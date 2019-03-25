Describe "function Invoke-JIssueTransition" -Tag Integration {
    BeforeAll {
        Open-JSession

        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    BeforeEach {
        $issue = Get-JIssue -ID $Ticket
        if("In Progress" -eq $issue.Status)
        {
            $issue | Invoke-JIssueTransition -TransitionTo "Open"
        }
    }

    It "Transition by ID" {

        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        Invoke-JIssueTransition -ID $ticket -TransitionTo "In Progress"
        $issue.Refresh()
        $issue.Status | Should -Be 'In Progress'
        Invoke-JIssueTransition -ID $ticket -TransitionTo "Open"
        $issue.Refresh()
        $issue.Status | Should -Be 'Open'
    }

    It "Transition by object" {
        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        $issue | Invoke-JIssueTransition -TransitionTo "In Progress"
        $issue.Status | Should -Be 'In Progress'
        $issue | Invoke-JIssueTransition -TransitionTo "Open"
        $issue.Status | Should -Be 'Open'
    }

    It "Transition invalid ticket should throw" {
        {
            Invoke-JIssueTransition -ID $MissingTicket -TransitionTo "In Progress"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }

    It "Transition invalid status should throw" {
        {
            Invoke-JIssueTransition -ID $Ticket -TransitionTo "INVALID_STATUS"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }

    It "Transition invalid status should throw" {
        {
            $issue | Invoke-JIssueTransition -TransitionTo "INVALID_STATUS"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
