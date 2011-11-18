param ([string]$installationFolder)

Import-Module WebAdministration

$uri = New-Object System.URI $installationFolder
$localPath = $uri.LocalPath
$siteName = "DummyNews"
$dns = "www.dummynews.com.au"
$appPool = "IIS:\AppPools\$siteName"
$site = "IIS:\Sites\$siteName"

if (!(Test-Path $appPool)) { 
    New-Item $appPool | Out-Null
} 

if (!(Test-Path $site)) { 
    New-Item $site -bindings @{protocol="http";bindingInformation=":80:$dns"} -physicalPath $installationFolder | Out-Null
	Set-ItemProperty $site -name applicationPool -value "$siteName" | Out-Null
} 

Write-Output "Installed site $dns"