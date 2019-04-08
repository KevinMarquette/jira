Describe "function Get-JVersion" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Project = "LDDTFT"
    }

    It "Gets a version by Pproject" {
        $version = Get-JVersion -Project $Project
        $version | Should -Not -BeNullOrEmpty
    }
}
