# psyml

YAML PowerShell module

This PowerShell module is a wrapper on top of [YamlDotNet][YamlDotNet] that serializes and deserializes PowerShell objects to and from YAML.  
It is heavily inspired by standard `ConvertFrom-Json` and `ConvertTo-Json` implementations.

The main idea of psyml was to create a binary PowerShell YAML module that will be idempotent when converting back and forth objects and YAML strings and would not lose original types.  
If some value is wrapped in single or double quotes in YAML string it is still a string after serialization and stays to be the string after deserialization.

As the examples are better than elaborates, this is the desired behavior:

```powershell
$yaml = 'key: "True"'
$object = ConvertFrom-Yaml $yaml
$object.key.GetType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     String                                   System.Object

$object | ConvertTo-Yaml
key: "True"
```

and this is not:

```powershell
$yaml = 'key: "True"'
$object = ConvertFrom-Yaml $yaml
$object.key.GetType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Boolean                                  System.ValueType

$object | ConvertTo-Yaml
key: true
```

In case something is not handled in this manner please use [Issues page][GitHubIssues]

## Installation

### PowerShell Gallery

Run the following command in a PowerShell session to install the rollup module for psyml cmdlets:

```powershell
Install-Module -Name psyml -Scope CurrentUser
```

This module runs on Windows PowerShell with [.NET Framework 4.8][DotNetFramework] or greater, or [PowerShell Core][PowerShellCore].

If you have an earlier version of the psyml modules installed from the PowerShell Gallery and would like to update to the latest version, run the following commands in a PowerShell session:

```powershell
Update-Module -Name psyml -Scope CurrentUser
```

`Update-Module` installs the new version side-by-side with previous versions. It does not uninstall the previous versions.

## Usage

### ConvertTo-Yaml

#### Convert object to YAML

To convert object to YAML use `ConvertTo-Yaml` cmdlet:

```powershell
PS /> $object = @{
    property = 'value1'
    array = @(
        1,
        2,
        3
    )
    nested = @{
        bool = $true
        string = 'True'
    }
}
PS /> ConvertTo-Yaml $object

property: value1
nested:
  bool: true
  string: "True"
array:
- 1
- 2
- 3
```

#### Convert object to JSON compatible YAML (valid JSON)

To convert object to YAML use `ConvertTo-Yaml` cmdlet with `-JsonCompatible` parameter:

```powershell
PS /> $object = @{
    property = 'value1'
    array = @(
        1,
        2,
        3
    )
    nested = @{
        bool = $true
        string = 'True'
    }
}
PS /> ConvertTo-Yaml $object -JsonCompatible

{"property": "value1", "nested": {"bool": true, "string": "True"}, "array": [1, 2, 3]}
```

### ConvertFrom-Yaml

#### Convert YAML to object

To convert YAML to object use `ConvertFrom-Yaml` cmdlet:

```powershell
PS /> $yaml = @"
property: value1
nested:
  bool: true
  string: "True"
array:
- 1
- 2
- 3
"@
PS /> $object = ConvertFrom-Yaml $yaml
PS /> $object

property nested                    array
-------- ------                    -----
value1   @{bool=True; string=True} {1, 2, 3}

PS /> $object.GetType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     False    PSCustomObject                           System.Object
```

To change output type to Hash Table use `-AsHashtable` parameter:

```powershell
PS /> $object = ConvertFrom-Yaml $yaml -AsHashtable
PS /> $object.gettype()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Hashtable                                System.Object
```

To change output type to Ordered Dictionary use `-AsOrderedDictionary` parameter:

```powershell
PS /> $object = ConvertFrom-Yaml $yaml -AsOrderedDictionary
PS /> $object.gettype()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     OrderedDictionary                        System.Object
```

## Reporting Issues and Feedback

To report issue, bug or feature request please use the [Issues page][GitHubIssues]

## TODO

- [] release pipeline and first published version in PowerShell Gallery
- [] yaml tag handling
- [] `System.DateTime` and `System.Version` types handling
- [] `Test-Yaml` cmdlet
- [] `-NoEnumerate` should be fixed



<!-- References -->

<!-- Local -->
[GitHubIssues]: https://github.com/bitrut94/psyml/issues

<!-- External -->
[YamlDotNet]: https://github.com/aaubry/YamlDotNet
