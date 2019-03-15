Describe "function New-Issue" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
        $Project = "LDDTFT"
    
        Open-JiraSession -Credential $Credential -Uri $JiraUri
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