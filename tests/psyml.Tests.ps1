#Requires -Modules @{ ModuleName="Pester"; ModuleVersion="5.0" }
#Requires -Module psyml

Describe "ConvertFrom-Yaml - parser tests" {
    # examples from https://yaml.org/spec/1.2/spec.html
    Context "Yaml conversion - collections" {
        It "Handles sequence of scalars" {
            $yaml = @"
- Mark McGwire
- Sammy Sosa
- Ken Griffey
"@
            $yaml | ConvertFrom-Yaml | Should -HaveCount 3
            ConvertFrom-Yaml $yaml | Should -HaveCount 3
            $yaml | ConvertFrom-Yaml | Should -Contain 'Mark McGwire'
            $yaml | ConvertFrom-Yaml | Should -Contain 'Sammy Sosa'
            $yaml | ConvertFrom-Yaml | Should -Contain 'Ken Griffey'
        }

        It "Handles scalars to scalars mapping" {
            $yaml = @"
hr:  65    # Home runs
avg: 0.278 # Batting average
rbi: 147   # Runs Batted In
"@
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -HaveCount 3
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -Contain 'hr'
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -Contain 'avg'
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -Contain 'rbi'
            ($yaml | ConvertFrom-Yaml -AsHashtable).hr | Should -Be 65
            ($yaml | ConvertFrom-Yaml -AsHashtable).avg | Should -Be 0.278
            ($yaml | ConvertFrom-Yaml -AsHashtable).rbi | Should -Be 147
        }

        It "Handles scalars to sequences mapping" {
            $yaml = @"
american:
  - Boston Red Sox
  - Detroit Tigers
  - New York Yankees
national:
  - New York Mets
  - Chicago Cubs
  - Atlanta Braves
"@
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -HaveCount 2
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -Contain 'american'
            ($yaml | ConvertFrom-Yaml -AsHashtable).Keys | Should -Contain 'national'
            ($yaml | ConvertFrom-Yaml -AsHashtable).american | Should -HaveCount 3
            ($yaml | ConvertFrom-Yaml -AsHashtable).national | Should -HaveCount 3
        }

        It "Handles sequences mappings" {
            $yaml = @"
-
    name: Mark McGwire
    hr:   65
    avg:  0.278
-
    name: Sammy Sosa
    hr:   63
    avg:  0.288
"@
            $yaml | ConvertFrom-Yaml | Should -HaveCount 2
            $yaml | ConvertFrom-Yaml | Select-Object -First 1 -ExpandProperty name | Should -Be 'Mark McGwire'
        }

        It "Handles sequence of sequences" {
            $yaml = @"
- [name        , hr, avg  ]
- [Mark McGwire, 65, 0.278]
- [Sammy Sosa  , 63, 0.288]
"@
            $yaml | ConvertFrom-Yaml | Should -HaveCount 3
            $yaml | ConvertFrom-Yaml -NoEnumerate | ForEach-Object {
                $_ | Should -BeOfType System.Array
                $_ | Should -HaveCount 3
            }
        }

        It "Handles mappings of mappings" {
            $yaml = @"
Mark McGwire: {hr: 65, avg: 0.278}
Sammy Sosa: {
    hr: 63,
    avg: 0.288
}
"@
            $object = ConvertFrom-Yaml $yaml -AsHashtable
            $object.Keys | Should -HaveCount 2
            $object.'Mark McGwire'.avg | Should -Be 0.278
            $object.'Sammy Sosa'.hr | Should -Be 63
        }
    }

    Context "Yaml conversion - structures" {
        It "Handles two documents in a stream" {
            $yaml = @"
# Ranking of 1998 home runs
---
- Mark McGwire
- Sammy Sosa
- Ken Griffey

# Team ranking
---
- Chicago Cubs
- St Louis Cardinals
"@
            $objects = ConvertFrom-Yaml $yaml
            $objects | Should -HaveCount 2
            $objects[0] | Should -HaveCount 3
            $objects[1] | Should -HaveCount 2

            $objects = $yaml | ConvertFrom-Yaml
            $objects | Should -HaveCount 2
            $objects[0] | Should -HaveCount 3
            $objects[1] | Should -HaveCount 2
        }

        It "Handles two documents with closing tags in a stream" {
            $yaml = @"
---
time: 20:03:20
player: Sammy Sosa
action: strike (miss)
...
---
time: 20:03:47
player: Sammy Sosa
action: grand slam
...
"@
            $objects = ConvertFrom-Yaml $yaml
            $objects | Should -HaveCount 2

            $objects = $yaml | ConvertFrom-Yaml
            $objects | Should -HaveCount 2
        }

        It "Handles anchors" {
            $yaml = @"
---
hr:
    - Mark McGwire
    # Following node labeled SS
    - &SS Sammy Sosa
rbi:
    - *SS # Subsequent occurrence
    - Ken Griffey
"@
            $object = ConvertFrom-Yaml $yaml
            $object.hr[1] | Should -BeExactly $object.rbi[0]
        }

        It "Handles complex mapping keys" {
            # It's technicaly possible for hashtables and ordered dictionaries but makes no sense
            $yaml = @"
? - Detroit Tigers
  - Chicago cubs
:
  - 2001-07-23

? [ New York Yankees,
    Atlanta Braves ]
: [ 2001-07-02, 2001-08-12,
    2001-08-14 ]
"@
            $psobject = ConvertFrom-Yaml $yaml

            $psobject.psobject.Properties.GetEnumerator() |
            Where-Object { '2001-07-23' -in $_.Value } |
            Select-Object -ExpandProperty Name | Should -Be '[ Detroit Tigers, Chicago cubs ]'

            $hashtable = ConvertFrom-Yaml $yaml -AsHashtable
            $hashtable.GetEnumerator() |
            Where-Object { '2001-07-02' -in $_.Value } |
            Select-Object -Last 1 -ExpandProperty Name | Should -Be @('New York Yankees', 'Atlanta Braves')
        }

        It "Handles compact nested mapping" {
            $yaml = @"
---
# Products purchased
- item    : Super Hoop
  quantity: 1
- item    : Basketball
  quantity: 4
- item    : Big Shoes
  quantity: 1
"@
            $object = ConvertFrom-Yaml $yaml
            $object[0].item | Should -Be 'Super Hoop'
            $object[1].quantity | Should -Be 4
        }
    }

    Context "Yaml conversion - scalars" {
        It "Preserves newlines in literals" {
            $yaml = @"
# ASCII Art
--- |
  \//||\/||
  // ||  ||__
"@
            ConvertFrom-Yaml $yaml | Should -Be "\//||\/||`n// ||  ||__"
        }

        It "Changes newlines to spaces in folded scalars" {
            $yaml = @"
--- >
  Mark McGwire's
  year was crippled
  by a knee injury.
"@
            ConvertFrom-Yaml $yaml | Should -Be "Mark McGwire's year was crippled by a knee injury."
        }

        It "Preserves folded newlines for 'more indented' and blank lines" {
            $yaml = @"
>
  Sammy Sosa completed another
  fine season with great stats.

    63 Home Runs
    0.288 Batting Average

  What a year!
"@
            ConvertFrom-Yaml $yaml | Should -Be "Sammy Sosa completed another fine season with great stats.`n`n  63 Home Runs`n  0.288 Batting Average`n`nWhat a year!"
        }
    }
}

Describe "ConvertFrom-Yaml" {
    Context "Output type" {
        It "Returns '<type>' when '<parameter>' is passed" -TestCases @(
            @{ Type = 'System.Collections.Hashtable'; Parameter = 'AsHashtable' }
            @{ Type = 'System.Collections.Specialized.OrderedDictionary'; Parameter = 'AsOrderedDictionary' }
            @{ Type = 'System.Management.Automation.PSCustomObject'; Parameter = $null }
        ) {
            $params = @{ InputObject = 'tmp: 1' }
            if ($Parameter -ne $null) { $params += @{$Parameter = $true } }
            ConvertFrom-Yaml @params | Should -BeOfType $type
        }
    }

    Context "Data types" {
        It "Converts '<value>' to '<type>'" -TestCases @(
            @{ Value = 'samplestring'; Type = 'System.String' }
            @{ Value = '1'; Type = 'System.Int32' }
            @{ Value = '-1'; Type = 'System.Int32' }
            @{ Value = '1.1'; Type = 'System.Double' }
            @{ Value = '-1.1'; Type = 'System.Double' }
            @{ Value = '1.1.1'; Type = 'System.String' }
            @{ Value = 'true'; Type = 'System.Boolean' }
            @{ Value = '"true"'; Type = 'System.String' }
            @{ Value = 'False'; Type = 'System.Boolean' }
            @{ Value = '2001-12-15T02:59:43.1Z'; Type = 'System.String' }
            @{ Value = '"null"'; Type = 'System.String' }
        ) {
            ConvertFrom-Yaml $value | Should -BeOfType $type
        }

        It "Converts '<value>' to '<type>'" -TestCases @(
            @{ Value = '!!str 1'; Type = 'System.String' }
            @{ Value = '!!bool "true"'; Type = 'System.Boolean' }
        ) {
            ConvertFrom-Yaml $value | Should -BeOfType $type
        }
    }

    It "Handles raw input and string arrays the same way" -TestCases @(
        @{ InputObject = [string[]]@(
            "key:"
            "  value1: 1"
            "  value2:"
            "  - 1"
            "  - 2"
            )
        }
        @{ InputObject = @"
key:
  value1: 1
  value2:
  - 1
  - 2
"@
        }
    ) {
        $object = $InputObject | ConvertFrom-Yaml -AsHashtable

        $object.Contains('key') | Should -Be $true
        $object.key.Contains('value1') | Should -Be $true
        $object.key.value1 | Should -Be 1
        $object.key.Contains('value2') | Should -Be $true
        $object.key.value2 | Should -HaveCount 2
        $object.key.value2 | Should -be @(1, 2)
    }
}

Describe "ConvertTo-Yaml - parser tests" {
    # testing examples from 'Parser tests' in the opposite direction
    Context "Yaml conversion - collections" {
        It "Handles sequence of scalars" {
            $yaml = @"
- Mark McGwire
- Sammy Sosa
- Ken Griffey
"@
            @('Mark McGwire', 'Sammy Sosa', 'Ken Griffey') | ConvertTo-Yaml | Should -Match $yaml
        }

        It "Handles scalars to scalars mapping" {
            $yaml = @"
hr: 65
avg: 0.278
rbi: 147
"@
            [PSCustomObject]@{
                hr  = 65
                avg = 0.278
                rbi = 147
            } | ConvertTo-Yaml | Should -Match $yaml
            [ordered]@{
                hr  = 65
                avg = 0.278
                rbi = 147
            } | ConvertTo-Yaml | Should -Match $yaml
            # @{
            #     hr = 65
            #     avg = 0.278
            #     rbi = 147
            # } | ConvertTo-Yaml | Should -Match $yaml
        }

        It "Handles scalars to sequences mapping" {
            $yaml = @"
american:
- Boston Red Sox
- Detroit Tigers
- New York Yankees
national:
- New York Mets
- Chicago Cubs
- Atlanta Braves
"@

            [PSCustomObject]@{
                american = @(
                    'Boston Red Sox',
                    'Detroit Tigers',
                    'New York Yankees'
                )
                national = @(
                    'New York Mets',
                    'Chicago Cubs',
                    'Atlanta Braves'
                )
            } | ConvertTo-Yaml | Should -Match $yaml
            [ordered]@{
                american = @(
                    'Boston Red Sox',
                    'Detroit Tigers',
                    'New York Yankees'
                )
                national = @(
                    'New York Mets',
                    'Chicago Cubs',
                    'Atlanta Braves'
                )
            } | ConvertTo-Yaml | Should -Match $yaml
        }

        It "Handles sequences mappings" {
            $yaml = @"
- name: Mark McGwire
  hr: 65
  avg: 0.278
- name: Sammy Sosa
  hr: 63
  avg: 0.288
"@
            @(
                [ordered]@{
                    name = 'Mark McGwire'
                    hr   = 65
                    avg  = 0.278
                }
                [PSCUstomObject]@{
                    name = 'Sammy Sosa'
                    hr   = 63
                    avg  = 0.288
                }
            ) | ConvertTo-Yaml | Should -Match $yaml
        }
    }
}

Describe "ConvertTo-Yaml" {
    Context "Output type" {
        if ($PSEdition -eq 'Core') {
            It "Returns json compatible output when '-JsonCompatible' is passed" {
                @{
                    string  = 'teststring'
                    int     = 1
                    bool    = $true
                    nullkey = $null
                    array   = @(
                        1,
                        'tmp'
                        @{
                            nestedItem = $true
                        }
                    )
                } | ConvertTo-Yaml -JsonCompatible | Test-Json | Should -Be $true
            }
        } else {
            It "Returns json compatible output when '-JsonCompatible' is passed" {
                {
                    @{
                        string  = 'teststring'
                        int     = 1
                        bool    = $true
                        nullkey = $null
                        array   = @(
                            1,
                            'tmp'
                            @{
                                nestedItem = $true
                            }
                        )
                    } | ConvertTo-Yaml -JsonCompatible | ConvertFrom-Json
                } | Should -Not -Throw
            }
        }

        It "Converts '<value>' to '<string>'" -TestCases @(
            @{ Value = 'samplestring'; String = 'samplestring' }
            @{ Value = 1; String = '1' }
            @{ Value = -1; String = '-1' }
            @{ Value = 1.1; String = '1.1' }
            @{ Value = -1.1; String = '-1.1' }
            @{ Value = $true; String = 'true' }
            @{ Value = 'true'; String = '"true"' }
            @{ Value = $false; String = 'false' }
            @{ Value = @{tmp = $null }; String = 'tmp: null' }
            @{ Value = @{tmp = '' }; String = 'tmp: ""' }
            @{ Value = @{tmp = ' ' }; String = 'tmp: " "' }
            @{ Value = '2001-12-15T02:59:43.1Z'; String = '"2001-12-15T02:59:43.1Z"' }
        ) {
            ConvertTo-Yaml $value | Should -Match $String
        }
    }
}
