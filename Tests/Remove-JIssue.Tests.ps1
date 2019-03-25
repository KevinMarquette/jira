Describe "function Remove-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Project = "LDDTFT"
    }

    It "Creates a ticket" {
        $newIssueSplat = @{
            Summary = "test ticket 2"
            Project = $Project
            Type    = "New Feature"
        }
        $issue = New-JIssue @newIssueSplat
        $issueID = $issue.Key
        $issue | Should -Not -BeNullOrEmpty
        $issue | Remove-JIssue
        $issue2 = Get-JIssue -ID $issueID
        $issue2 | Should -BeNullOrEmpty
    }
}
