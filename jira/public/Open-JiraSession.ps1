function Open-JiraSession
{
    [cmdletbinding(DefaultParameterSetName = 'Default')]
    param(
        [Parameter(
            Mandatory,
            ParameterSetName = 'Save'
        )]
        [Parameter(
            ParameterSetName = 'Default'
        )]
        [PSCredential]
        $Credential,

        [Parameter(
            Mandatory,
            ParameterSetName = 'Save',
            Position = 0
        )]
        [Parameter(
            ParameterSetName = 'Default',
            Position = 0
        )]
        [string]
        $Uri,

        [Parameter(
            ParameterSetName = 'Save'
        )]
        [switch]
        $Save
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
            $Uri = Get-PSFConfigValue -FullName jira.Uri

            if ([string]::IsNullOrEmpty($Uri))
            {
                throw [JiraModule.JiraConnectionException]::new(
                    'No Jira URI or server endpoint was provided, call Open-JiraSession -Uri https://myjiraserver.org -SaveUri'
                )
            }
        }

        Write-Verbose "Uri [$Uri]"

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

        Write-Verbose "Credential [$($credential.UserName)]"
        JiraModule\Open-JiraSession -Credential $Credential -Uri $uri

        if ($Save)
        {
            Write-Verbose "Saving URI [$Uri] for future use"
            $null = Set-PSFConfig -Module jira -Name Uri -Value $Uri -PassThru |
                Register-PSFConfig

            Write-Verbose "Storing [$Credential.UserName] credential for [$cmTarget]"

            $storedCredential = @{
                Target         = $cmTarget
                UserName       = $Credential.UserName
                SecurePassword = $Credential.Password
            }
            $null = New-StoredCredential @storedCredential -Comment "for use with the Jira module"
        }
    }
}
