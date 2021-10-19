if ($PSEdition -eq 'Desktop') {
    $loadPath = 'net4.8'
} else {
    $loadPath = 'net5.0'
}

try {
    Add-Type -Path $PSScriptRoot/$loadPath/YamlDotNet.dll `
        -ErrorAction Ignore | Out-Null
} catch {
    Write-Verbose $_
}

try {
    Import-Module $PSScriptRoot/$loadPath/psyml.dll `
        -ErrorAction Ignore | Out-Null
} catch {
    Write-Verbose $_
}
