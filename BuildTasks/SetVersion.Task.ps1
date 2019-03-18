function GetModulePublicInterfaceMap
{
    param($Path)
    Write-Verbose "Module path [$Path]"
    $psd1 = Resolve-Path @("$Path\*\*.psd1")
    if (-not $psd1)
    {
        $psd1 = $path
    }
    $metadata = Invoke-Expression (Get-Content -Path $psd1 -Raw)
    $metadata.NestedModules
    $metadata.RequiredModules
    $metadata.FunctionsToExport
    $metadata.CmdletsToExport
    $metadata.AliasesToExport
    $metadata.RootModule

    if ($metadata.NestedModules -match '^bin.*dll$')
    {
        Write-Verbose 'Binary module detected, using simple interface' -Verbose
        'Binary'
        return
    }

    $psm1 = Resolve-Path @("$Path\*\*.psm1")
    if (-not $psm1)
    {
        $psm1 = $path -replace 'psd1$','psm1'
    }
    $module = ImportModule -Path $psm1 -PassThru
    $exportedCommands = @(
        $module.ExportedFunctions.values
        $module.ExportedCmdlets.values
        $module.ExportedAliases.values
    )

    foreach($command in $exportedCommands)
    {
        foreach ($parameter in $command.Parameters.Keys)
        {
            if($false -eq $command.Parameters[$parameter].IsDynamic)
            {
                '{0}:{1}' -f $command.Name, $command.Parameters[$parameter].Name
                foreach ($alias in $command.Parameters[$parameter].Aliases)
                {
                    '{0}:{1}' -f $command.Name, $alias
                }
            }
        }
    }
}

task SetVersion {
    $version = [version]"0.1.0"
    $publishedModule = $null
    $bumpVersionType = 'Patch'
    $versionStamp = (git rev-parse origin/master) + (git rev-parse head)

    "Load current version"
    [version] $sourceVersion = (Get-Metadata -Path $manifestPath -PropertyName 'ModuleVersion')
    "  Source version [$sourceVersion]"

    $downloadFolder = Join-Path -Path $output downloads
    $null = New-Item -ItemType Directory -Path $downloadFolder -Force -ErrorAction Ignore

    $versionFile = Join-Path $downloadFolder versionfile
    if(Test-Path $versionFile)
    {
        $versionFileData = Get-Content $versionFile -raw
        if($versionFileData -eq $versionStamp)
        {
            continue
        }
    }

    "Checking for published version"
    $publishedModule = Find-Module -Name $ModuleName -ErrorAction 'Ignore' -AllowPrerelease |
        Sort-Object -Property {[version]($_.Version -split '-')[0] } -Descending |
        Select -First 1

    if($null -ne $publishedModule)
    {
        [version] $publishedVersion = ($publishedModule.Version -split '-')[0]
        "  Published version [$publishedVersion]"

        $version = $publishedVersion

        "Downloading published module to check for breaking changes"
        $publishedModule | Save-Module -Path $downloadFolder

        [System.Collections.Generic.HashSet[string]] $publishedInterface = @(GetModulePublicInterfaceMap -Path (Join-Path $downloadFolder $ModuleName))
        [System.Collections.Generic.HashSet[string]] $buildInterface = @(GetModulePublicInterfaceMap -Path $ManifestPath)

        if (-not $publishedInterface.IsSubsetOf($buildInterface))
        {
            $bumpVersionType = 'Major'
        }
        elseif ($publishedInterface.count -ne $buildInterface.count)
        {
            $bumpVersionType = 'Minor'
        }
    }

    if ($version -lt ([version] '1.0.0'))
    {
        "Module is still in beta; don't bump major version."
        if ($bumpVersionType -eq 'Major')
        {
            $bumpVersionType = 'Minor'
        }
        else
        {
            $bumpVersionType = 'Patch'
        }
    }

    "  Steping version [$bumpVersionType]"
    $version = [version] (Step-Version -Version $version -Type $bumpVersionType)

    "  Comparing to source version [$sourceVersion]"
    if($sourceVersion -gt $version)
    {
        "    Using existing version"
        $version = $sourceVersion
    }
    <#
    if ( -not [string]::IsNullOrEmpty( $env:Build_BuildID ) )
    {
        $build = $env:Build_BuildID
        $version = [version]::new($version.Major, $version.Minor, $version.Build, $build)
    }
    elseif ( -not [string]::IsNullOrEmpty( $env:APPVEYOR_BUILD_ID ) )
    {
        $build = $env:APPVEYOR_BUILD_ID
        $version = [version]::new($version.Major, $version.Minor, $version.Build, $build)
    }
    #>
    "  Setting version [$version]"
    Update-Metadata -Path $ManifestPath -PropertyName 'ModuleVersion' -Value $version

    (Get-Content -Path $ManifestPath -Raw -Encoding UTF8) |
        ForEach-Object {$_.TrimEnd()} |
        Set-Content -Path $ManifestPath -Encoding UTF8

    Set-Content -Path $versionFile -Value $versionStamp -NoNewline -Encoding UTF8

    if(Test-Path $BuildRoot\fingerprint)
    {
        Remove-Item $BuildRoot\fingerprint
    }
}
