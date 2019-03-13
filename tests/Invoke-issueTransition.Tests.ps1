Describe "function Invoke-IssueTransition" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
        $Ticket = "LDDTFT-14"

        Open-JiraSession -Credential $Credential -Uri $JiraUri
    }

    It "Transition by ID" {
        Invoke-IssueTransition -ID $ticket -Action "In Progress4"
    }
    It "Transition by ID async" {
        Invoke-IssueTransition -ID $ticket -Action "Open" -Async |
            Receive-AsyncResult
    }

    It "Add comment by ID async" {
        Add-Comment -ID $ticket -Comment 'Test Comment 2' -Async
    }

    It "Add comment by issue" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'
        $issue | Add-Comment -Comment "Test Comment 3"         
    }

    It "Add comment by issue async" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'

        $issue | Add-Comment -Comment "Test Comment 5" -Async   
    }

}