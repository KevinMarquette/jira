---
external help file: JiraModule.dll-Help.xml
Module Name: jira
online version:
schema: 2.0.0
---

# Invoke-IssueTransition

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### IssueID (Default)
```
Invoke-IssueTransition [-Key] <String[]> [-TransitionTo] <String> [<CommonParameters>]
```

### InputObject
```
Invoke-IssueTransition [-InputObject] <Issue> [-TransitionTo] <String> [<CommonParameters>]
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

### -TransitionTo
{{ Fill TransitionTo Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases: Action, Target

Required: True
Position: 1
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

## OUTPUTS

### Atlassian.Jira.Issue

### JiraModule.AsyncResult

## NOTES

## RELATED LINKS
