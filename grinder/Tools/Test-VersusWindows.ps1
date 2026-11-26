[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $Executable,
    [ValidateSet('Pair', 'Host', 'Client')]
    [string] $Role = 'Pair',
    [ValidatePattern('^[A-Za-z0-9-]{4,24}$')]
    [string] $Room = ('QA-' + [guid]::NewGuid().ToString('N').Substring(0, 10)),
    [switch] $Impaired,
    [switch] $Relay,
    [string] $LogDirectory
)
$ErrorActionPreference = 'Stop'
$player = (Resolve-Path -LiteralPath $Executable).Path
if (!$LogDirectory) { $LogDirectory = Join-Path (Split-Path $player) "qa-$Room" }
$logs = (New-Item -Path $LogDirectory -ItemType Directory -Force).FullName
$children = @{}

function Start-Probe([string] $name) {
    $log = Join-Path $logs "$name.log"
    if (Test-Path -LiteralPath $log) { throw "Existing log: $log. Use a new room or log directory." }
    $arguments = @('-batchmode', '-nographics', '--meatgrinder-versus-probe', $name, $Room, '-logFile', ('"' + $log + '"'))
    if ($Impaired) { $arguments += '--meatgrinder-impaired' }
    if ($Relay) { $arguments += '--meatgrinder-relay' }
    $children[$name] = Start-Process -FilePath $player -WorkingDirectory (Split-Path $player) -ArgumentList $arguments -WindowStyle Hidden -PassThru
    return $log
}

try {
    Write-Output "Room: $Room. Logs: $logs"
    if ($Role -ne 'Client') { $hostLog = Start-Probe 'host' }
    if ($Role -eq 'Pair') {
        $deadline = [datetime]::UtcNow.AddSeconds(45)
        while (!(Test-Path -LiteralPath $hostLog) -or
            !(Select-String -LiteralPath $hostLog -SimpleMatch '[VersusProbe] Connected host' -Quiet)) {
            if ($children['host'].HasExited -or [datetime]::UtcNow -ge $deadline) {
                throw "Host did not open a lobby. See $hostLog"
            }
            Start-Sleep -Milliseconds 250
        }
    }
    if ($Role -ne 'Host') { $null = Start-Probe 'client' }
    $deadline = [datetime]::UtcNow.AddSeconds(180)
    $failed = $false
    foreach ($name in @($children.Keys)) {
        $process = $children[$name]
        while (!$process.HasExited) {
            if ([datetime]::UtcNow -ge $deadline) { throw "Timeout. See $logs" }
            Start-Sleep -Milliseconds 250
        }
        $log = Join-Path $logs "$name.log"
        Select-String -LiteralPath $log -SimpleMatch '[VersusProbe]' | ForEach-Object Line
        if ($process.ExitCode -ne 0 -or
            !(Select-String -LiteralPath $log -SimpleMatch "[VersusProbe] PASS $name" -Quiet)) { $failed = $true }
    }
    if ($failed) { throw "Versus probe failed. See $logs" }
}
finally {
    foreach ($process in $children.Values) {
        if (!$process.HasExited) { $process.Kill() }
        $process.Dispose()
    }
}
