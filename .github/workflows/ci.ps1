# set version suffix if it is tag and the tag name contains a suffix like "beta1"
function Set-VersionSuffixOnTag([string]$dir, [string]$branch)
{
    Write-Host "Set-VersionSuffixOnTag" $dir "on branch '$branch'"

    # Gets version prefix and suffix from tag name. Example: "refs/tags/1.3.3-beta1" => "1.3.3" + "beta1"
    $match = [regex]::Match($branch, "^(refs\/tags\/)(?<prefix>[^-]+)(-(?<suffix>\S+))?$")

    Write-Host "regex match:" $match

    if ($match.Success)
    {
        Write-Host "Set-VersionSuffixOnTag detected a tag"

        $suffix = $match.Groups["suffix"].Value

        if (![string]::IsNullOrWhiteSpace($suffix))
        {
            Set-VersionSuffix $dir $suffix
        }
    }
}

# Add xml element "VersionSuffix" to *.csproj files in $dir.
function Set-VersionSuffix([string]$dir, [string]$suffix)
{
    Write-Host "Setting version suffix to '$suffix'"

    $projFiles = Get-ChildItem $dir -Recurse -Filter *.csproj

    $projFiles | Select "Name"

    foreach ($file in $projFiles)
    {
        $content = [xml](Get-Content $file.FullName)

        $versionSuffix = $content.CreateElement("VersionSuffix");
        $versionSuffix.set_InnerXML($suffix)
        [void] $content.Project.PropertyGroup.AppendChild($versionSuffix)
        $content.Save($file.FullName);
    }
}
