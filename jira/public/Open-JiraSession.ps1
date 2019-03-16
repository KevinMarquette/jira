function Open-JiraSession
{
    [cmdletbinding(DefaultParameterSetName = 'Default')]
    param(
        [Parameter(
            Mandatory,
            ParameterSetName = 'SaveCredential'
        )]
        [Parameter(
            Mandatory,
            ParameterSetName = 'ClearStoredCredential'
        )]
        [Parameter(
            ParameterSetName = 'Default'
        )]
        [PSCredential]
        $Credential,

        [Parameter(
            Position = 0
        )]
        [string]
        $Uri,

        [Parameter(ParameterSetName = 'ClearStoredCredential')]
        [switch]
        $ClearStoredCredential,

        [Parameter(ParameterSetName = 'SaveCredential')]
        [switch]
        $SaveCredential,

        [Parameter()]
        [switch]
        $SaveUri
    )

    begin
    {
        $cmTarget = 'JiraModule'
    }
    end
    {
        if ([string]::IsNullOrEmpty($Uri))
        {
            Write-Verbose "Using Saved Uri"
            $Uri = Get-PSFConfig -Module jira -Name Uri

            if ([string]::IsNullOrEmpty($Uri))
            {
                throw [JiraModule.JiraConnectionException]::new(
                    'No Jira URI or server endpoint was provided, call Open-JiraSession -Uri https://myjiraserver.org -SaveUri'
                )
            }
        }

        if ($SaveUri)
        {
            Write-Verbose "Saving URI [$Uri] for future use"
            Set-PSFConfig -Module jira -Name Uri -Value $Uri
        }

        if ($ClearStoredCredential)
        {
            Write-Verbose "Removing stored Credential for [$cmTarget]"
            Remove-StoredCredential -Target $cmTarget -ErrorAction Ignore
        }

        if ($SaveCredential)
        {
            Write-Verbose "Storing [$Credential.UserName] credential for [$cmTarget]"

            $storedCredential = @{
                Target         = $cmTarget
                UserName       = $Credential.UserName
                SecurePassword = $Credential.Password
            }
            New-StoredCredential @storedCredential -Comment "for use with the Jira module"
        }

        if ($null -eq $Credential -or
            $Credential -eq [PScredential]::Empty)
        {
            Write-Verbose "Credential was empty, using Stored Credential for [$cmTarget]"
            $Credential = Get-StoredCredential -Target $cmTarget
            if ($null -eq $Credential)
            {
                throw [JiraModule.JiraAuthenticationException]::new(
                    'No credential was provided and no previoud credential was saved. Run [Open-JiraSessions -Credential $Credential -SaveCredential]'
                )
            }
        }

        JiraModule\Open-JiraSession -Credential $Credential -Uri $uri
    }
}
