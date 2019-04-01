Describe "function Close-JSession" -Tag Integration {
    BeforeEach {
        Open-JSession
        $session = Get-JSession
        $session.IsConnected | Should -BeTrue
    }

    It "Closes a JSession" {
        Close-JSession
        $session.IsConnected | Should -BeFalse
        $session2 = Get-JSession
        $session2 | Should -BeNullOrEmpty
    }

    It "Closes a JSession over pipeline" {
        $session | Close-JSession
        $session.IsConnected | Should -BeFalse
        $session2 = Get-JSession
        $session2 | Should -BeNullOrEmpty
    }
}
