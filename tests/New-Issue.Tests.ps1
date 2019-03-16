Describe "function New-Issue" -Tag Integration {
    BeforeAll {
        Open-JiraSession
        $Project = "LDDTFT"
    }

    It "Creates a ticket" {
        $newIssueSplat = @{
            Summary = "test ticket 1"
            Project = $Project
            Type    = "New Feature"
        }
        $issue = New-Issue @newIssueSplat
        $issueID = $issue.Key
        $issue | Should -Not -BeNullOrEmpty
        $issue | Remove-Issue
        $issue2 = Get-Issue -ID $issueID
        $issue2 | Should -BeNullOrEmpty
    }
}
