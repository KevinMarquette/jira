Describe "function Invoke-JIssueAction" -Tag Integration {
    BeforeAll {
        Open-JSession

        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    BeforeEach {
        $issue = Get-JIssue -Key $Ticket
        if("In Progress" -eq $issue.Status)
        {
            $issue | Invoke-JIssueAction -Name "Open"
        }
    }

    It "Transition by ID (by position)" {

        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        Invoke-JIssueAction -Key $ticket -TransitionTo "In Progress"
        $issue.Refresh()
        $issue.Status | Should -Be 'In Progress'
        Invoke-JIssueAction -Key $ticket -TransitionTo "Open"
        $issue.Refresh()
        $issue.Status | Should -Be 'Open'
    }

    It "Transition by object" {
        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        $issue | Invoke-JIssueAction -Name "In Progress"
        $issue.Status | Should -Be 'In Progress'
        $issue | Invoke-JIssueAction -Name "Open"
        $issue.Status | Should -Be 'Open'
    }

    It "Transition invalid ticket should throw" {
        {
            Invoke-JIssueAction -Key $MissingTicket -Name "In Progress"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }

    It "Transition invalid status should throw" {
        {
            Invoke-JIssueAction -Key $Ticket -Name "INVALID_STATUS"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }

    It "Transition invalid status by pipeline should throw" {
        {
            $issue | Invoke-JIssueAction -Name "INVALID_STATUS"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
