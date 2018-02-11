#calls dotnet pack for all sub dirs of $dir
function Dotnet-Pack([string]$dir)
{
    $projDirs = Get-ChildItem $dir

    foreach($projDir in $projDirs)
    {
        dotnet pack --configuration Release --no-build --include-symbols --output ./../../output $projDir.FullName;
    }
}

#calls dotnet test for all sub dirs of $dir
function Dotnet-Test([string]$dir)
{
    $projDirs = Get-ChildItem $dir -Recurse -Filter *.csproj

    foreach($projDir in $projDirs)
    {
        dotnet test --configuration Release --no-build $projDir.FullName;
    }
}

# set version suffix if it is tag and the tag name contains a suffix like "beta1"
function Set-VersionSuffixOnTag([string]$dir) 
{
    if($env:APPVEYOR_REPO_TAG -eq "true")
    {
        $suffix = Extract-Suffix($env:APPVEYOR_REPO_TAG_NAME);
    
        if(![string]::IsNullOrWhiteSpace($suffix))
        {
            Set-VersionSuffix $dir $suffix
        }
    }
}


# Gets version suffix from tag name or null. Example: "1.3.3-beta1" => "beta1"
function Extract-Suffix([string] $tagName)
{
    $index = $tagName.IndexOf("-")
    
    if($index -gt -1)
    {
        return $tagName.Substring($index + 1);
    }

    return $null
}

# Add xml element "VersionSuffix" to *.csproj files in $dir.
function Set-VersionSuffix([string]$dir, [string]$suffix)
{
    Write-Host "Setting version suffix to '$suffix'"

    $projFiles = Get-ChildItem $dir -Recurse -Filter *.csproj

    $projFiles | Select "Name"

    foreach($file in $projFiles)
    {
        $content = [xml](Get-Content $file.FullName)
       
        $versionSuffix = $content.CreateElement("VersionSuffix");
        $versionSuffix.set_InnerXML($suffix)
        [void] $content.Project.PropertyGroup.AppendChild($versionSuffix)
        $content.Save($file.FullName);
    }
}