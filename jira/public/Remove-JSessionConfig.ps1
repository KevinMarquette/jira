function Remove-JSessionConfig
{
    [Alias("Remove-JiraSessionConfig")]
    [cmdletbinding(SupportsShouldProcess)]
    param()

    begin
    {
        $cmTarget = 'JiraModule'
    }

    end
    {
        if ($PSCmdlet.ShouldProcess("Jira Module Default Connection Values"))
        {
            Write-Verbose "Clearing Jira Uri"
            Unregister-PSFConfig -Module jira -Name Uri

            Write-Verbose "Removing stored credential for [$cmTarget]"
            Remove-StoredCredential -Target $cmTarget -ErrorAction Ignore
        }
    }
}
