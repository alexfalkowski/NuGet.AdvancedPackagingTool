Import-Module WebAdministration

$installationFolder = "file:///C:/Ninemsn/PackageManager/Ninemsn.PackageManager.NuGet.Test.Integration/bin/Debug/App_Data/packages/DummyNews/"
$uri = New-Object System.URI $installationFolder
$localPath = $uri.LocalPath
$siteName = "DummyNews"
$dns = "www.dummynews.com.au"
$appPool = "IIS:\AppPools\$siteName"
$site = "IIS:\Sites\$siteName"

if (Test-Path $appPool ) { 
    Remove-Item $appPool -Force -Recurse 
} 

New-Item $appPool > $null

if (Test-Path $site ) { 
    Remove-Item $site -Force -Recurse 
} 
New-Item $site -bindings @{protocol="http";bindingInformation=":80:$dns"} -physicalPath $installationFolder > $null
Set-ItemProperty $site -name applicationPool -value "$siteName" > $null

Write-Output "Created site $dns"