Describe "function Get-JIssue" -Tag Integration {
    BeforeAll {
        Open-JSession

    }

    It "Gets a JSession" {
        $result = Get-JSession
        $result | Should -Not -BeNullOrEmpty
    }
}
