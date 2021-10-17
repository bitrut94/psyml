try {
    Add-Type -Path $PSScriptRoot/YamlDotNet.dll `
        -ErrorAction Ignore | Out-Null
} catch {
    Write-Verbose $_
}

try {
    Import-Module $PSScriptRoot/psyml.dll `
        -ErrorAction Ignore | Out-Null
} catch {
    Write-Verbose $_
}
