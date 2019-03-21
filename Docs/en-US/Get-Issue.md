---
external help file: JiraModule.dll-Help.xml
Module Name: jira
online version:
schema: 2.0.0
---

# Get-Issue

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### IssueID (Default)
```
Get-Issue [-Key] <String[]> [-Async] [<CommonParameters>]
```

### InputObject
```
Get-Issue [-InputObject] <Issue> [-Async] [<CommonParameters>]
```

### Query
```
Get-Issue [-Query] <String> [[-MaxResults] <Int32>] [-StartAt <Int32>] [-Async] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -Async
{{ Fill Async Description }}

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject
{{ Fill InputObject Description }}

```yaml
Type: Issue
Parameter Sets: InputObject
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Key
{{ Fill Key Description }}

```yaml
Type: String[]
Parameter Sets: IssueID
Aliases: ID, JiraID

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -MaxResults
{{ Fill MaxResults Description }}

```yaml
Type: Int32
Parameter Sets: Query
Aliases: Count

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Query
{{ Fill Query Description }}

```yaml
Type: String
Parameter Sets: Query
Aliases: JQL

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -StartAt
{{ Fill StartAt Description }}

```yaml
Type: Int32
Parameter Sets: Query
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

### Atlassian.Jira.Issue

### System.String

### System.Int32

## OUTPUTS

### Atlassian.Jira.Issue

### JiraModule.AsyncResult

## NOTES

## RELATED LINKS
