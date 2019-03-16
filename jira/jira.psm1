[CmdletBinding()]
param()
$Script:PSModuleRoot = $PSScriptRoot
Write-Verbose -Message "This file is replaced in the build output, and is only used for debugging."
Write-Verbose -Message $PSScriptRoot

# Class importer
$root = Join-Path -Path $PSScriptRoot -ChildPath 'classes'
Write-Verbose "Load classes from [$root]"
$classFiles = Get-ChildItem -Path $root -Filter '*.ps1' -Recurse |
            Where-Object Name -notlike '*.Tests.ps1'

$classes = @{}

foreach($file in $classFiles)
{
    $name = $file.BaseName
    $classes[$name] = @{
        Name = $name
        Path = $file.FullName
    }
    $data = Get-Content $file.fullname
    foreach($line in $data)
    {
        if($line -match "\s+($Name)\s*(:|requires)\s*(?<baseclass>\w*)|\[(?<baseclass>\w+)\]")
        {
            $classes[$name].Base += @($Matches.baseclass)
        }
    }
}

$importOrder = $classes.GetEnumerator() | 
    Resolve-DependencyOrder -Key {$_.Name} -DependsOn {$_.Value.Base}

foreach( $class in $importOrder )
{
    Write-Verbose $class.Value.Path
    . $class.Value.Path
}

$folders = 'includes', 'internal', 'private', 'public', 'resources'
foreach ($folder in $folders)
{
    $root = Join-Path -Path $PSScriptRoot -ChildPath $folder
    if (Test-Path -Path $root)
    {
        Write-Verbose -Message "Importing files from [$folder]..."
        $files = Get-ChildItem -Path $root -Filter '*.ps1' -Recurse |
            Where-Object Name -notlike '*.Tests.ps1'

        foreach ($file in $files)
        {
            Write-Verbose -Message "Dot sourcing [$($file.BaseName)]..."
            . $file.FullName
        }
    }
}

Write-Verbose -Message 'Exporting public functions...'
$functions = Get-ChildItem -Path "$PSScriptRoot\public" -Filter '*.ps1' -Recurse

Export-ModuleMember -Function $functions.BaseName
