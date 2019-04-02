Describe "function Get-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession

        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    It "Gets an issue by ID" {
        $issue = Get-JIssue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty
        $issue.Key | Should -Be $Ticket
    }

    It "Gets an issue by ID positionally" {
        $issue = Get-JIssue $Ticket
        $issue | Should -Not -BeNullOrEmpty
        $issue.Key | Should -Be $Ticket
    }

    It "Gets an async issue" {
        $result = Get-JIssue -ID $Ticket -Async
        $result | Should -Not -BeNullOrEmpty
        $issue = $result | Receive-JAsyncResult
        $issue.Key | Should -Be $Ticket
    }

    It "Accepts a Jira issue over the pipe" {
        $issue = Get-JIssue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'

        $result = $issue | Get-JIssue
        $result | Should -Not -BeNullOrEmpty
        $result.Key | Should -Be $Ticket
    }

    It "Accepts a Jira issue over the pipe async" {
        $issue = Get-JIssue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty -Because 'We need a valid issue to test with'

        $result = $issue | Get-JIssue -Async | Receive-JAsyncResult
        $result | Should -Not -BeNullOrEmpty
        $result.Key | Should -Be $Ticket
    }

    It "multiple async issues" {
        $result = @(
            Get-JIssue -ID $Ticket -Async
            Get-JIssue -ID $Ticket -Async
            Get-JIssue -ID $Ticket -Async
            Get-JIssue -ID $Ticket -Async
        )

        $issue = $result | Receive-AsyncResult
        $issue | Should -Not -BeNullOrEmpty
        $issue | Should -HaveCount $result.Count
    }

    It "Gets Jira issue by query" {
        Get-JIssue -Query "Key = $Ticket"
    }

    It "Querying for missing issue does not throw" {
        Get-JIssue -Query "Key = $MissingTicket"
    }

    It "Get missing issue does not throw" {
        Get-JIssue -ID $MissingTicket
    }

    It "Invalid query should throw" {
        {
            Get-JIssue -Query "INVALID_QUERY"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
        {
            Get-JIssue -Query "INVALID_FIELD = INVALID_VALUE"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
