$ws = New-Object -ComObject WScript.Shell
$desktop = [Environment]::GetFolderPath('Desktop')
$lnk = Join-Path $desktop 'HousingApp.lnk'
$sc = $ws.CreateShortcut($lnk)
$exe = (Resolve-Path '.\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\HousingApp.exe').ProviderPath
if (-not (Test-Path $exe)) { Write-Error "EXE_NOT_FOUND: $exe"; exit 1 }
$sc.TargetPath = $exe
$sc.WorkingDirectory = Split-Path $exe
$sc.IconLocation = $exe + ',0'
$sc.Save()
Write-Output "SHORTCUT_CREATED:$lnk"
