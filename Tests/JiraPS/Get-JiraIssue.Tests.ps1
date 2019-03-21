Describe "JiraPS Get-JiraIssue" -Tag JiraPS {

    BeforeAll {
        Open-JiraSession
        $Ticket = "LDDTFT-13"
        $Credential = Get-StoredCredential -Target 'JiraModule'
        $Uri = Get-PSFConfigValue -FullName jira.Uri
    }
    It "Can pipe issue to Get-JiraIssue" {
        throw 'not implemented'
        $issue = Get-Issue -ID $Ticket
            Trace-Command -Name ParameterBinding -Expression {
                $issue | Get-JiraIssue -Credential $Credential
            } -PSHost
    }
}
