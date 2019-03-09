#https://bitbucket.org/farmas/atlassian.net-sdk/wiki/Home

cd .\src
dotnet clean
dotnet publish
Import-module .\src\bin\Debug\netstandard2.0\publish\jiraModule.dll
$JiraUri = 'https://jira.loandepot.com'

$cred = Get-LDRemoteCredential -RemoteTarget ld.corp.local
$tickets = echo LDCM-14057 LDCM-14058 LDCM-14059 LDCM-14060 LDCM-14061 LDCM-14062 LDCM-14063 LDCM-14064 LDCM-14065
$tickets = echo LDDTFT-10

Get-Issue -ID $tickets -Credential $cred -Uri $JiraUri -Async 
Get-Member -InputObject $b

Measure-Command {
    $a = Get-Issue -ID $tickets -Credential $cred -Uri $JiraUri -Async
}
Measure-Command {
    $r = Get-RawIssue -ID $tickets -Credential $cred -Uri $JiraUri
}
Measure-Command {
    $b = Search-Issue -ID $tickets -Credential $cred -Uri $JiraUri -Async
}
Get-Issue2 -Query 'issuekey in ("LDCM-14057")' -Credential $cred -Verbose -Uri $JiraUri -OutVariable result
Measure-Command {
    $r = Search-Issue -Query 'status = "Ready for Release" ' -Credential $cred -Verbose -Uri $JiraUri -Count 1000 -Async
}

# LDDTFT-10


#https://jira.loandepot.com/rest/api/latest/search?maxResults=50&jql=issuekey+in+("LDCM-14057")&expand=transitions
#https://jira.loandepot.com/rest/api/latest/search?maxResults=50&jql=issuekey+in+("LDCM-14057")&expand=transitions
