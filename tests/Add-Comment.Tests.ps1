Describe "function Get-Issue" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
        $Ticket = "LDDTFT-19"

        Open-JiraSession -Credential $Credential -Uri $JiraUri
    }

    It "Add comment by ID" {
        Add-Comment -ID $ticket -Comment 'Test Comment 1'
    }


    It "Add comment by issue" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'
        $issue | Add-Comment -Comment "Test Comment 3"         
    }
}