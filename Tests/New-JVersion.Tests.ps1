Describe "function New-JVersion" -Tag Integration {
    BeforeAll {
        Open-JSession
        $Project = "LDDTFT"
    }

    It "Creates a version" {
        $name = "test {0:MM.dd.hh.mm}" -f (Get-Date)
        $version = New-JVersion -Project $Project -Name $Name
        $version | Should -Not -BeNullOrEmpty
        $version.Name | Should -Be $name
    }
}
