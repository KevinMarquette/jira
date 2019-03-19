Describe "function Save-Issue" -Tag Integration {
    BeforeAll {
        Open-JiraSession
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    It "Saves custom field value" {
        $riskLevel = "4"
        $name = 'Risk Level'
        $issue = Get-Issue -ID $Ticket
        $issue2 = Get-Issue -ID $Ticket

        $issue[$name] | Should -Not -Be $riskLevel -Because "issue should have some other value to start with"
        $issue2[$name] | Should -Not -Be $riskLevel -Because "issue2 should have some other value to start with"

        $issue[$name] = $riskLevel
        # throws JiraModule.JiraInvalidActionException, need to fix
        $issue | Save-Issue

        $issue[$name] | Should -Be $riskLevel

        $issue2[$name] | Should -Not -Be $riskLevel -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2[$name] | Should -Be $riskLevel

        $issue3 = Get-Issue -ID $Ticket
        $issue3[$name] | Should -Be $riskLevel
    }

    It "Sets the description on an issue using the pipe" {
        $description = "TEST_DESCRIPTION_1 $(Get-Date)"
        $issue = Get-Issue -ID $Ticket
        $issue2 = Get-Issue -ID $Ticket

        $issue.description | Should -Not -Be $description -Because "issue should have some other value to start with"
        $issue2.description | Should -Not -Be $description -Because "issue2 should have some other value to start with"

        $issue.Description = $description
        $issue | Save-Issue

        $issue.description | Should -Be $description

        $issue2.description | Should -Not -Be $description -Because "This is a second local copy that should not auto-update"
        $issue2.Refresh()
        $issue2.description | Should -Be $description

        $issue3 = Get-Issue -ID $Ticket
        $issue3.description | Should -Be $description
    }
}
