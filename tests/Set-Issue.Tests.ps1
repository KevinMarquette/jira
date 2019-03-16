Describe "function Set-Issue" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"

        Open-JiraSession -Credential $Credential -Uri $JiraUri
    }

    It "Sets the description on an issue using the pipe" {
        $description = "TEST_DESCRIPTION_1 $(Get-Date)"
        $issue = Get-Issue -ID $Ticket
        $issue2 = Get-Issue -ID $Ticket

        $issue.description | Should -Not -Be $description -Because "issue should have some other value to start with"
        $issue2.description | Should -Not -Be $description -Because "issue2 should have some other value to start with"

        Set-Issue -Description $description -Verbose -InputObject $issue
        $issue.description | Should -Be $description

        $issue2.description | Should -Not -Be $description -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2.description | Should -Be $description

        $issue3 = Get-Issue -ID $Ticket
        $issue3.description | Should -Be $description
    }

    It "Sets the description on an issue using the pipe" {
        $description = "TEST_DESCRIPTION_2 $(Get-Date)"
        $issue = Get-Issue -ID $Ticket
        $issue2 = Get-Issue -ID $Ticket

        $issue.description | Should -Not -Be $description -Because "issue should have some other value to start with"
        $issue2.description | Should -Not -Be $description -Because "issue2 should have some other value to start with"

        Set-Issue -Description $description -Verbose -ID $Ticket
        $issue.description | Should -Be $description

        $issue2.description | Should -Not -Be $description -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2.description | Should -Be $description

        $issue3 = Get-Issue -ID $Ticket
        $issue3.description | Should -Be $description
    }


    It "Invalid ticket should throw" {
        $description = "TEST_DESCRIPTION_2 %{Get-Date}"
        {
            Set-Issue -Description $description -Verbose -ID $MissingTicket
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
