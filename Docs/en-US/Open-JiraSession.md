---
external help file: jira-help.xml
Module Name: jira
online version:
schema: 2.0.0
---

# Open-JiraSession

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Default (Default)
```
Open-JiraSession [-Credential <PSCredential>] [[-Uri] <String>] [-SaveUri] [<CommonParameters>]
```

### ClearStoredCredential
```
Open-JiraSession -Credential <PSCredential> [[-Uri] <String>] [-ClearStoredCredential] [-SaveUri]
 [<CommonParameters>]
```

### SaveCredential
```
Open-JiraSession -Credential <PSCredential> [[-Uri] <String>] [-SaveCredential] [-SaveUri] [<CommonParameters>]
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

### -ClearStoredCredential
{{ Fill ClearStoredCredential Description }}

```yaml
Type: SwitchParameter
Parameter Sets: ClearStoredCredential
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential
{{ Fill Credential Description }}

```yaml
Type: PSCredential
Parameter Sets: Default
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

```yaml
Type: PSCredential
Parameter Sets: ClearStoredCredential, SaveCredential
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SaveCredential
{{ Fill SaveCredential Description }}

```yaml
Type: SwitchParameter
Parameter Sets: SaveCredential
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SaveUri
{{ Fill SaveUri Description }}

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

### -Uri
{{ Fill Uri Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see about_CommonParameters (http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Object
## NOTES

## RELATED LINKS
