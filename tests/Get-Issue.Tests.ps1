Describe "function Get-Issue" {
    BeforeAll {
        $JiraUri = 'https://jira.loandepot.com'
        $Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
        $Ticket = "LDDTFT-10"

        Open-JiraSession -Credential $Credential -Uri $JiraUri
    }

    It "Gets an issue by ID" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty
        $issue.Key | Should -Be $Ticket
    }

    It "Gets an async issue" {
        $result = Get-Issue -ID $Ticket -Async
        $result | Should -Not -BeNullOrEmpty
        $issue = $result | Receive-AsyncResult
        $issue.Key | Should -Be $Ticket
    }

    It "Accepts a Jira issue over the pipe" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'

        $result = $issue | Get-Issue
        $result | Should -Not -BeNullOrEmpty
        $result.Key | Should -Be $Ticket
    }

    It "Accepts a Jira issue over the pipe async" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'

        $result = $issue | Get-Issue -Async | Receive-AsyncResult
        $result | Should -Not -BeNullOrEmpty
        $result.Key | Should -Be $Ticket
    }

    It "multiple async issues" {
        $result = @(
            Get-Issue -ID $Ticket -Async
            Get-Issue -ID $Ticket -Async
            Get-Issue -ID $Ticket -Async
            Get-Issue -ID $Ticket -Async
        )

        $issue = $result | Receive-AsyncResult
        $issue | Should -Not -BeNullOrEmpty
        $issue | Should -HaveCount $result.Count
    }

}