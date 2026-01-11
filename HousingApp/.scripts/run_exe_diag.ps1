$path = '.\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\HousingApp.exe'
if (-not (Test-Path $path)) { Write-Output "EXE_NOT_FOUND:$path"; exit 1 }
$exe = (Resolve-Path $path).ProviderPath
Write-Output "EXE:$exe"
try {
    $p = Start-Process -FilePath $exe -PassThru -ErrorAction Stop
    Start-Sleep -Seconds 2
    if ($p) {
        Write-Output "STARTED:PID=$($p.Id)"
        Get-Process -Id $p.Id | Select-Object Id, ProcessName, StartTime | Format-List
    } else {
        Write-Output "START_FAILED"
    }
} catch {
    Write-Output "START_EXCEPTION:$($_.Exception.Message)"
    exit 2
}
