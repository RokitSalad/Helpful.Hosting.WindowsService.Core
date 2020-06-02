$Service = "$PSScriptRoot\..\DemoServiceBackgroundProcess.exe"
& $Service stop
Start-Sleep -Seconds 5
& $Service uninstall
Start-Sleep -Seconds 5
& $Service install
Start-Sleep -Seconds 5
& $Service Start
