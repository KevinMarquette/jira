Describe "CmdLet Open-JiraSession" -Tag Integration {

    BeforeAll {
        $password = "mypassword" |
            ConvertTo-SecureString -asPlainText -Force

        $username = "joedirt"
        $badCredential = New-Object System.Management.Automation.PSCredential($username, $password)
    }
    It "Invalid URI should throw" {
        {
            Open-JiraSession -Uri "INVALID_URI"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Invalid Hostname should throw" {
        {
            Open-JiraSession -Uri "HTTP://MISSINGHOST"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Unresponsive host should throw" {
        {
            Open-JiraSession -Uri "https://localhost"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Live but invalid endpoint should throw" {
        {
            Open-JiraSession -Uri "https://www.google.com"
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Bad Credential should throw" {
        {
            Open-JiraSession -Credential $badCredential
        } | Should -Throw -ExceptionType ([JiraModule.JiraConnectionException])
    }

    It "Establishes a connection to Jira endpoint" {
        Open-JiraSession
    }

    It "Save parameter persists uri and credential" -Pending {
        Mock -Verifiable -CommandName Set-PSFConfig -MockWith {}
        Mock -Verifiable -CommandName New-StoredCredential -MockWith {}

        {
            $openJiraSessionSplat = @{
                Credential = $badCredential
                Uri        = "https://www.google.com"
            }
            Open-JiraSession @openJiraSessionSplat -ErrorAction Ignore
        } | Should -Throw -ExceptionType ([JiraModule.JiraModuleException])

        Assert-MockCalled -CommandName Set-PSFConfig -Times 0
        Assert-MockCalled -CommandName New-StoredCredential -Times 0

        {
            $openJiraSessionSplat = @{
                Credential = $badCredential
                Save       = $true
                Uri        = "https://www.google.com"
            }
            Open-JiraSession @openJiraSessionSplat
        } | Should -Throw -ExceptionType ([JiraModule.JiraModuleException])

        Assert-MockCalled -CommandName Set-PSFConfig -Times 1
        Assert-MockCalled -CommandName New-StoredCredential -Times 1
    }

    It "Use persisted values" {
        Mock -Verifiable -CommandName Get-PSFConfigValue -MockWith {
            "https://contoso.com"
        }
        Mock -Verifiable -CommandName Get-StoredCredential -MockWith {
            [PSCredential]::Empty()
        }

        {
            Open-JiraSession
        } | Should -Throw -ExceptionType ([JiraModule.JiraModuleException])

        Assert-MockCalled -CommandName Get-PSFConfigValue -Times 1
        Assert-MockCalled -CommandName Get-StoredCredential -Times 1
    }
}
