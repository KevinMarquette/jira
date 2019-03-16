---
external help file: JiraModule.dll-Help.xml
Module Name: jira
online version:
schema: 2.0.0
---

# Remove-Issue

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### IssueID (Default)
```
Remove-Issue [-ID] <String[]> [<CommonParameters>]
```

### InputObject
```
Remove-Issue [-InputObject] <Issue> [<CommonParameters>]
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

### -ID
{{ Fill ID Description }}

```yaml
Type: String[]
Parameter Sets: IssueID
Aliases: Key, JiraID

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
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

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

### Atlassian.Jira.Issue

## OUTPUTS

### Atlassian.Jira.Issue

### JiraModule.AsyncResult

## NOTES

## RELATED LINKS
