Describe "function Get-Issue" -Tag Integration {
    BeforeAll {
        Open-JiraSession
        $Ticket = "LDDTFT-13"
    }

    It "Add comment by ID" {
        Add-Comment -ID $ticket -Comment 'Test Comment 1'
    }

    It "Add comment to missing ticket should throw" {
        {
            Add-Comment -ID 'MISSING-01' -Comment 'Test Comment 1'
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }

    It "Add comment by issue" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'
        $issue | Add-Comment -Comment "Test Comment 3"
    }
}
