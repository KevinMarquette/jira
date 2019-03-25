Describe "function Save-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    It "Saves custom field value" {
        $riskLevel = "4"
        $name = 'Risk Level'
        $issue = Get-JIssue -ID $Ticket
        $issue2 = Get-JIssue -ID $Ticket

        $issue[$name] | Should -Not -Be $riskLevel -Because "issue should have some other value to start with"
        $issue2[$name] | Should -Not -Be $riskLevel -Because "issue2 should have some other value to start with"

        $issue[$name] = $riskLevel
        # throws JiraModule.JiraInvalidActionException, need to fix
        $issue | Save-JIssue

        $issue[$name] | Should -Be $riskLevel

        $issue2[$name] | Should -Not -Be $riskLevel -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2[$name] | Should -Be $riskLevel

        $issue3 = Get-JIssue -ID $Ticket
        $issue3[$name] | Should -Be $riskLevel
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
