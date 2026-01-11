$exeRelative = '..\bin\Debug\net8.0-windows10.0.19041.0\win10-x64\HousingApp.exe'
$exe = Resolve-Path -Path (Join-Path $PSScriptRoot $exeRelative) -ErrorAction SilentlyContinue
if (-not $exe) { Write-Output "EXE_NOT_FOUND:$exeRelative"; exit 1 }
$exe = $exe.ProviderPath
try {
    $p = Start-Process -FilePath $exe -PassThru -ErrorAction Stop
    $p.WaitForExit()
    Write-Output "EXITCODE=$($p.ExitCode)"
    exit $p.ExitCode
} catch {
    Write-Output "START_EXCEPTION:$($_.Exception.Message)"
    exit 2
}
