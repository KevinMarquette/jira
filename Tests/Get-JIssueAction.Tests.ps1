Describe "function Get-JIssueAction" -Tag Integration {
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

    It "Get Action by Key" {

        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        $results = Get-JIssueAction -Key $ticket
        $results | Should -Not -BeNullOrEmpty
        $results | Should -HaveCount 2

        $results = $results | Sort-Object -Property Id
        $results[0].To | Should -Be "In Progress"
        $results[0].Action | Should -Be "In Progress"
        $results[1].To | Should -Be "Canceled"
        $results[1].Action | Should -Be "Canceled"
    }

    It "Get Action by object over pipleine" {
        $issue.Status | Should -Be 'Open' -Because "We need a known starting point"
        $results = $issue | Get-JIssueAction
        $results | Should -Not -BeNullOrEmpty
        $results | Should -HaveCount 2

        $results = $results | Sort-Object -Property Id
        $results[0].To | Should -Be "In Progress"
        $results[0].Action | Should -Be "In Progress"
        $results[1].To | Should -Be "Canceled"
        $results[1].Action | Should -Be "Canceled"
    }

    It "Transition invalid ticket should throw" {
        {
            Get-JIssueAction -Key $MissingTicket
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
