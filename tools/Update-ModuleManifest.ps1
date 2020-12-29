Push-Location $PSScriptRoot/../artifacts
$version = git tag --points-at HEAD
$files = Get-ChildItem -Recurse -File |
Resolve-Path -Relative

$parameters = @{
    Path          = './psyml.psd1'
    ModuleVersion = $version.Split('-')[0]
    FileList      = $files
}

if ($version.Split('-').Count -gt 1) {
    # e.g. 1.0.0-preview0
    $parameters['Prerelease'] = $version.Split('-')[1]
}

Update-ModuleManifest @parameters
Pop-Location
