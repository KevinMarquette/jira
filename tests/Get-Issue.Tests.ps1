Describe "function Get-Issue" -Tag Integration {
    BeforeAll {
        Open-JiraSession

        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    It "Gets an issue by ID" {
        $issue = Get-Issue -ID $Ticket
        $issue | Should -Not -BeNullOrEmpty
        $issue.Key | Should -Be $Ticket
    }

    It "Gets an issue by ID positionaly" {
        $issue = Get-Issue $Ticket
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

    It "Gets Jira issue by query" {
        Get-Issue -Query "Key = $Ticket"
    }

    It "Querying for missing issue does not throw" {
        Get-Issue -Query "Key = $MissingTicket"
    }

    It "Get missing issue does not throw" {
        Get-Issue -ID $MissingTicket
    }

    It "Invalid query should throw" {
        {
            Get-Issue -Query "INVALID_QUERY"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
        {
            Get-Issue -Query "INVALID_FIELD = INVALID_VALUE"
        } | Should -Throw -ExceptionType ([JiraModule.JiraInvalidActionException])
    }
}
