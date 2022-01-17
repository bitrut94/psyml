---
external help file: psyml.dll-Help.xml
Module Name: psyml
online version:
schema: 2.0.0
---

# ConvertFrom-Yaml

## SYNOPSIS
Converts a YAML-formatted string to a custom object, hash table or ordered dictionary.

## SYNTAX

```
ConvertFrom-Yaml [-InputObject] <String> [-AsHashtable] [-AsOrderedDictionary] [-NoEnumerate]
 [<CommonParameters>]
```

## DESCRIPTION
The ConvertFrom-Yaml cmdlet converts a YAML formatted string to a custom PSCustomObject object that has a property for each field in the YAML string.
YAML is commonly used for configuration files and in applications where data is being stored or transmitted.

## EXAMPLES

### EXAMPLE 1
```
PS > Get-Content YamlFile.yml | ConvertFrom-Yaml
```

The command uses Get-Content cmdlet to get the strings in a YAML file.
Then it uses the pipeline operator to send the delimited string to the ConvertFrom-Json cmdlet, which converts it to a custom object.

### EXAMPLE 2
```
PS > @"
key: value1
Key: value2
"@ | ConvertFrom-Yaml -AsHashtable
```

The YAML string contains two key value pairs with keys that differ only in casing.
Without the switch, the command would have thrown an error.

### EXAMPLE 3
```
PS > @"
key: value1
Key: value2
"@ | ConvertFrom-Yaml -AsOrderedDictionary
```

The YAML string contains two key value pairs with keys that differ only in casing.
Without the switch, the command would have thrown an error.

### EXAMPLE 4
```
PS > Get-Date | Select-Object -Property * | ConvertTo-Yaml | ConvertFrom-Yaml

DisplayHint : DateTime
DateTime    : poniedziałek, 14 grudnia 2020 22:10:03
Date        : 2020-12-14T00:00:00.0000000+01:00
Day         : 14
DayOfWeek   : Monday
DayOfYear   : 349
Hour        : 22
Kind        : Local
Millisecond : 418
Minute      : 10
Month       : 12
Second      : 3
Ticks       : 637435806034183959
TimeOfDay   : 22:10:03.4183959
Year        : 2020
```

The example uses the Select-Object cmdlet to get all of the properties of the DateTime object.
It uses the ConvertTo-Yaml cmdlet to convert the DateTime object to a string formatted as a YAML object and the ConvertFrom-Yaml cmdlet to convert the YAML-formatted string to a PSCustomObject object.

### EXAMPLE 5
```
PS > '- 1' | ConvertFrom-Yaml | ConvertTo-Yaml
1
PS > '- 1' | ConvertFrom-Yaml -NoEnumerate | ConvertTo-Yaml
- 1
```

The YAML string contains an array with a single element.
Without the switch, converting the YAML to a PSObject and then converting it back with the ConvertTo-Yaml command results in a single integer.

## PARAMETERS

### -AsHashtable
Converts the YAML to a hash table object.

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

### -AsOrderedDictionary
Converts the YAML to a ordered dictionary object.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: Ordered

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject
Specifies the YAML strings to convert to YAML objects.
Enter a variable that contains the string, or type a command or expression that gets the string.
You can also pipe a string to ConvertFrom-Yaml.
The InputObject parameter is required, but its value can be an empty string.
When the input object is an empty string, ConvertFrom-Yaml does not generate any output.
The InputObject value cannot be $null.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -NoEnumerate
Specifies that output is not enumerated.
Setting this parameter causes arrays to be sent as a single object instead of sending every element separately.
This guarantees that YAML can be round-tripped via ConvertTo-Yaml.

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

### System.String
Specifies the YAML strings to convert to YAML objects.
Enter a variable that contains the string, or type a command or expression that gets the string.
You can also pipe a string to ConvertFrom-Yaml.
The InputObject parameter is required, but its value can be an empty string.
When the input object is an empty string, ConvertFrom-Yaml does not generate any output.
The InputObject value cannot be $null.

## OUTPUTS

## NOTES

## RELATED LINKS
