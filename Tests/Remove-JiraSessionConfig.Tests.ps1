InModuleScope jira {
    Describe "function Remove-JiraSessionConfig" -Tag Build {
        Mock -Verifiable -CommandName Unregister-PSFConfig -MockWith {$null}
        Mock -Verifiable -CommandName Remove-StoredCredential -MockWith {$null}

        It "Remove should clear previously set values" {
            Remove-JiraSessionConfig
            Assert-MockCalled -CommandName Unregister-PSFConfig -Times 1
            Assert-MockCalled -CommandName Remove-StoredCredential -Times 1
        }
    }
}
