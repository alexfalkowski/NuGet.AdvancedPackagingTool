﻿param ([string]$installationFolder)

Import-Module WebAdministration

$siteName = "DummyNews"
$dns = "www.dummynews.com.au"
$appPool = "IIS:\AppPools\$siteName"
$site = "IIS:\Sites\$siteName"

if (Test-Path $appPool) { 
    Remove-Item $appPool -Force -Recurse 
} 

if (Test-Path $site) { 
    Remove-Item $site -Force -Recurse 
} 

Write-Output "Uninstalled site $dns"