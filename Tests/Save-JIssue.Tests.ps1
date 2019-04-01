Describe "function Save-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"

    }

    It "Saves custom field value" {

        $value = "kmarquette"
        $name = 'SDM'

        $issue = Get-JIssue -ID $Ticket
        $issue[$name] = ""

        $issue | Save-JIssue

        $issue = Get-JIssue -ID $Ticket
        $issue2 = Get-JIssue -ID $Ticket

        $issue[$name] | Should -Not -Be $value -Because "issue should have some other value to start with"
        $issue2[$name] | Should -Not -Be $value -Because "issue2 should have some other value to start with"

        $issue[$name] = $value
        # throws JiraModule.JiraInvalidActionException, need to fix
        $issue | Save-JIssue

        $issue[$name] | Should -Be $value

        $issue2[$name] | Should -Not -Be $value -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2[$name] | Should -Be $value

        $issue3 = Get-JIssue -ID $Ticket
        $issue3[$name] | Should -Be $value
    }

    It "Sets the description on an issue using the pipe" {
        $description = "TEST_DESCRIPTION_1 $(Get-Date)"
        $issue = Get-JIssue -ID $Ticket
        $issue2 = Get-JIssue -ID $Ticket

        $issue.description | Should -Not -Be $description -Because "issue should have some other value to start with"
        $issue2.description | Should -Not -Be $description -Because "issue2 should have some other value to start with"

        $issue.Description = $description
        $issue | Save-JIssue

        $issue.description | Should -Be $description

        $issue2.description | Should -Not -Be $description -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2.description | Should -Be $description

        $issue3 = Get-JIssue -ID $Ticket
        $issue3.description | Should -Be $description
    }
}
