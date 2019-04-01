Describe "function Set-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    It "Sets the description on an issue using issue ID" {
        $description = "TEST_DESCRIPTION_1 $(Get-Date)"
        $issue = Get-JIssue -ID $Ticket

        $issue.description | Should -Not -Be $description -Because "issue should have some other value to start with"

        Set-JIssue  $Ticket -Description $description

        $issue.description | Should -Not -Be $description -Because "This is a second local copy that should not auto-update"
        $issue.Refresh()
        $issue.description | Should -Be $description

        $issue2 = Get-JIssue -ID $Ticket
        $issue2.description | Should -Be $description
    }

    It "Sets the description on an issue using the pipe" {
        $description = "TEST_DESCRIPTION_2 $(Get-Date)"
        $issue = Get-JIssue -ID $Ticket
        $issue2 = Get-JIssue -ID $Ticket

        $issue.description | Should -Not -Be $description -Because "issue should have some other value to start with"
        $issue2.description | Should -Not -Be $description -Because "issue2 should have some other value to start with"

        $issue | Set-JIssue -Description $description
        $issue.description | Should -Be $description

        $issue2.description | Should -Not -Be $description -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2.description | Should -Be $description

        $issue3 = Get-JIssue -ID $Ticket
        $issue3.description | Should -Be $description
    }


    It "Invalid ticket should throw" {
        $description = "TEST_DESCRIPTION_2 $(Get-Date)"
        {
            Set-JIssue -Description $description -ID $MissingTicket
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
