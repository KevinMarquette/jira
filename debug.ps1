#https://bitbucket.org/farmas/atlassian.net-sdk/wiki/Home

cd .\src
dotnet clean
dotnet publish
Import-module .\src\bin\Debug\netstandard2.0\publish\jira.dll
$JiraUri = 'https://jira.loandepot.com'

$cred = Get-Credential -UserName kmarquette -Message corp
$tickets = echo LDCM-14057 LDCM-14058 LDCM-14059 LDCM-14060 LDCM-14061 LDCM-14062 LDCM-14063 LDCM-14064 LDCM-14065
Measure-Command {
    $a = Get-Issue -ID $tickets -Credential $cred -Verbose -Uri $JiraUri -Async
}
Measure-Command {
    $r = Get-RawIssue -ID $tickets -Credential $cred -Verbose -Uri $JiraUri
}
Measure-Command {
    $b = Search-Issue -ID $tickets -Credential $cred -Verbose -Uri $JiraUri -Async
}
Get-Issue2 -Query 'issuekey in ("LDCM-14057")' -Credential $cred -Verbose -Uri $JiraUri -OutVariable result
Measure-Command {
    $r = Search-Issue -Query 'status = "Ready for Release" ' -Credential $cred -Verbose -Uri $JiraUri -Count 1000
}
#https://jira.loandepot.com/rest/api/latest/search?maxResults=50&jql=issuekey+in+("LDCM-14057")&expand=transitions
#https://jira.loandepot.com/rest/api/latest/search?maxResults=50&jql=issuekey+in+("LDCM-14057")&expand=transitions