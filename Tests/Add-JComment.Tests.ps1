Describe "function Get-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Ticket = "LDDTFT-13"
    }

    It "Add comment by ID" {
        Add-JComment -ID $ticket -Comment 'Test Comment 1'
    }

    It "Add comment to missing ticket should throw" {
        {
            Add-JComment -ID 'MISSING-01' -Comment 'Test Comment 1'
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }

    It "Add comment by issue" {
        $issue = Get-JIssue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'
        $issue | Add-JComment -Comment "Test Comment 3"
    }
}
