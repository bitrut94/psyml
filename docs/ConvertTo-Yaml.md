---
external help file: psyml.dll-Help.xml
Module Name: psyml
online version:
schema: 2.0.0
---

# ConvertTo-Yaml

## SYNOPSIS
Converts an object to a YAML-formatted string.

## SYNTAX

```
ConvertTo-Yaml [-InputObject] <Object> [-JsonCompatible] [-EnableAliases] [-AsArray] [<CommonParameters>]
```

## DESCRIPTION
The ConvertTo-Yaml cmdlet converts (almost) any .NET object to a string in YAML format.
The properties are converted to field names, the field values are converted to property values, and the methods are removed.
You can then use the ConvertFrom-Yaml cmdlet to convert a YAML-formatted string to a YAML object, which is easily managed in PowerShell.

## EXAMPLES

### EXAMPLE 1
```
PS > (Get-UICulture).Calendar | ConvertTo-Yaml
MinSupportedDateTime: 0001-01-01T00:00:00.0000000
MaxSupportedDateTime: 9999-12-31T23:59:59.9999999
AlgorithmType: SolarCalendar
CalendarType: Localized
Eras:
- 1
TwoDigitYearMax: 2029
IsReadOnly: true
```

### EXAMPLE 2
```
PS > 1 | ConvertTo-Yaml
1
PS > 1 | ConvertTo-Yaml -AsArray
- 1
```

This example shows the output from ConvertTo-Yaml cmdlet with and without the AsArray switch parameter.
You can see the second output is preceded by the dash.

### EXAMPLE 3
```
PS > @{key = 'value'} | ConvertTo-Yaml -JsonCompatible
{"key": "value"}
```

This example shows the output from ConvertTo-Yaml cmdlet with the JsonCompatible switch parameter.
You can see that the output is compatible with the JSON format.

## PARAMETERS

### -AsArray
Forces the output to be array type.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -EnableAliases
Enables YAML aliases on output string.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject
Specifies the objects to convert to YAML format.
Enter a variable that contains the objects, or type a command or expression that gets the objects.
You can also pipe an object to ConvertTo-Yaml.
The InputObject parameter is required, but its value can be null ($null) or an empty string.
When the input object is $null, ConvertTo-Yaml returns null in YAML notation.
When the input object is an empty string, ConvertTo-Yaml returns YAML document with empty string (this should be fixed).

```yaml
Type: Object
Parameter Sets: (All)
Aliases: Data

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -JsonCompatible
Converts object to JSON compatible YAML string.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object
Specifies the objects to convert to YAML format.
Enter a variable that contains the objects, or type a command or expression that gets the objects.
You can also pipe an object to ConvertTo-Yaml.
The InputObject parameter is required, but its value can be null ($null) or an empty string.
When the input object is $null, ConvertTo-Yaml returns null in YAML notation.
When the input object is an empty string, ConvertTo-Yaml returns YAML document with empty string (this should be fixed).

## OUTPUTS

## NOTES

## RELATED LINKS
