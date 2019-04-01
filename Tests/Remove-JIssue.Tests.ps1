Describe "function Remove-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Project = "LDDTFT"
    }

    It "Removes a ticket by InputObject" {
        $newIssueSplat = @{
            Summary = "test Remove-Issue ticket 1"
            Project = $Project
            Type    = "New Feature"
            Description = "Test ticket creation"
        }
        $issue = New-JIssue @newIssueSplat
        $issueID = $issue.Key
        $issue | Should -Not -BeNullOrEmpty
        Remove-JIssue -InputObject $issue
        $issue2 = Get-JIssue -Key $issueID
        $issue2 | Should -BeNullOrEmpty
    }

    It "Removes a ticket by ID positionally" {
        $newIssueSplat = @{
            Summary = "test Remove-Issue ticket 2"
            Project = $Project
            Type    = "New Feature"
            Description = "Test ticket creation"
        }
        $issue = New-JIssue @newIssueSplat
        $issueID = $issue.Key
        $issue | Should -Not -BeNullOrEmpty
        $issueID | Should -Match $Project

        Remove-JIssue $issueID

        $issue2 = Get-JIssue -ID $issueID
        $issue2 | Should -BeNullOrEmpty
    }

    It "Removes a ticket over pipeline" {
        $newIssueSplat = @{
            Summary = "test Remove-Issue ticket 3"
            Project = $Project
            Type    = "New Feature"
            Description = "Test ticket creation"
        }
        $issue = New-JIssue @newIssueSplat
        $issueID = $issue.Key
        $issue | Should -Not -BeNullOrEmpty

        $issue | Remove-JIssue

        $issue2 = Get-JIssue -ID $issueID
        $issue2 | Should -BeNullOrEmpty
    }
}
