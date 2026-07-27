param(
    [string]$Version = "2.1.8",
    [string]$OutputPath = "AutoUpdateInfo.xml"
)

$xml = [xml](Get-Content $OutputPath)
$xml.item.version = $Version
$xml.item.url = "https://github.com/Brian0255/Track-O-Matic/releases/download/$Version/TrackOMatic_$Version.zip"
$xml.Save($OutputPath)

# Ensure file ends with a blank line
Add-Content -Path $OutputPath -Value "" -NoNewline
Add-Content -Path $OutputPath -Value ""

Write-Host "Updated AutoUpdateInfo.xml to version $Version"
