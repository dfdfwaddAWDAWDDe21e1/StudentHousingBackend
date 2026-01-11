$start=(Get-Date).AddMinutes(-30)
Get-WinEvent -FilterHashtable @{LogName='Application'; StartTime=$start} |
  Where-Object { ($_.Message -match 'HousingApp') -or ($_.Message -match 'Faulting') -or ($_.ProviderName -match 'Application Error|.NET Runtime') } |
  Select-Object TimeCreated, ProviderName, Id, LevelDisplayName, @{Name='Message';Expression={$_.Message}} |
  Format-List -Force
