push-location .\src
dotnet clean
dotnet publish
Pop-Location

Import-module .\src\bin\Debug\netstandard2.0\publish\jiraModule.dll


$JiraUri = 'https://jira.loandepot.com'
$Credential = Get-LDRemoteCredential -RemoteTarget ld.corp.local
$Ticket = "LDDTFT-19"

Open-JiraSession -Credential $Credential -Uri $JiraUri
break;

Measure-Command {
    $a = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000
    $null = $a.GetResult()
}
Measure-Command {
    $a = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000
    $null = $a.GetResult()
    $b = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000
    $null = $b.GetResult()
    $c = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000
    $null = $c.GetResult()
}

Measure-Command {
    $a = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000
    $b = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000
    $c = Get-Issue -Query 'status = "Ready for Release"' -Async -MaxResults 1000

    $null = $a.GetResult()
    $null = $b.GetResult()
    $null = $c.GetResult()
}

$Ticket = "LDDTFT-1"
$issue = 1..30 | %{"DTM-$_"}


Measure-Command {
    $a = $issue | %{Get-Issue -ID $_}
}


Measure-Command {
    $a = $issue | %{Get-Issue -ID $_ -Async} | Receive-AsyncResult
}
Measure-Command {
    $a = $issue | %{Get-Issue -ID $_  -Async }
    #$null = $a | Receive-AsyncResult
}
Measure-Command {
    $a = Get-Issue -ID $issue
}

Measure-Command {
    $a =  Get-Issue -ID $issue -Async | Receive-AsyncResult
}
