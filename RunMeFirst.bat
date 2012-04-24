IF EXIST nant.build (.\tools\nant\nant BuildMsmqUtils -D:targetframework=net-4.0)

.\tools\msmqutils\runner.exe %1