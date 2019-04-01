Describe "CmdLet Open-JSession" -Tag Integration {

    BeforeAll {
        $password = "mypassword" |
            ConvertTo-SecureString -asPlainText -Force

        $username = "joedirt"
        $badCredential = New-Object System.Management.Automation.PSCredential($username, $password)
    }
    It "Invalid URI should throw" {
        {
            Open-JSession -Uri "INVALID_URI"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Invalid Hostname should throw" {
        {
            Open-JSession -Uri "HTTP://MISSINGHOST"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Unresponsive host should throw" {
        {
            Open-JSession -Uri "https://localhost"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Live but invalid endpoint should throw" {
        {
            Open-JSession -Uri "https://www.google.com"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Bad Credential should throw" {
        {
            Open-JSession -Credential $badCredential
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Establishes a connection to Jira endpoint" {
        Open-JSession
    }

    It "PassThru should return a JSession object" {
        $result = Open-JSession -PassThru
        $result | Should -Not -BeNullOrEmpty
    }

    It "Save parameter persists uri and credential" -Pending {
        Mock -Verifiable -CommandName Set-PSFConfig -MockWith {}
        Mock -Verifiable -CommandName New-StoredCredential -MockWith {}

        {
            $openJiraSessionSplat = @{
                Credential = $badCredential
                Uri        = "https://www.google.com"
            }
            Open-JSession @openJiraSessionSplat -ErrorAction Ignore
        } | Should -Throw -ExceptionType ([JiraModule.JiraModuleException])

        Assert-MockCalled -CommandName Set-PSFConfig -Times 0
        Assert-MockCalled -CommandName New-StoredCredential -Times 0

        {
            $openJiraSessionSplat = @{
                Credential = $badCredential
                Save       = $true
                Uri        = "https://www.google.com"
            }
            Open-JSession @openJiraSessionSplat
        } | Should -Throw -ExceptionType ([JiraModule.JiraModuleException])

        Assert-MockCalled -CommandName Set-PSFConfig -Times 1
        Assert-MockCalled -CommandName New-StoredCredential -Times 1
    }

    It "Use persisted values" {

        # Mocking the return of invalid values to get a specific exception
        Mock -Verifiable -CommandName Get-PSFConfigValue -MockWith {
            "https://www.google.com"
        }
        Mock -Verifiable -CommandName Get-StoredCredential -MockWith {
            $password = "mypassword" |
            ConvertTo-SecureString -asPlainText -Force

            $username = "joedirt"
            New-Object System.Management.Automation.PSCredential($username, $password)
        }

        {
            Open-JSession -ErrorAction Stop
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])

        Assert-MockCalled -CommandName Get-PSFConfigValue -Times 1
        Assert-MockCalled -CommandName Get-StoredCredential -Times 1
    }
}
