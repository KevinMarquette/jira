Describe "function Receive-JAsyncResult" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Ticket = "LDDTFT-13"
        $MissingTicket = "MISSING-13"
    }

    It "Gets an async issue" {
        $result = Get-JIssue -ID $Ticket -Async
        $result | Should -Not -BeNullOrEmpty
        $issue = $result | Receive-JAsyncResult
        $issue.Key | Should -Be $Ticket
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
}
