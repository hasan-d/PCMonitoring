@echo off
echo Building PCmonitoring for Windows and Linux...
echo.

echo === Windows x64 ===
dotnet publish -c Release -r win-x64 --self-contained -o publish/win-x64
echo.

echo === Linux x64 ===
dotnet publish -c Release -r linux-x64 --self-contained -o publish/linux-x64
echo.

echo Done! Files are in publish\win-x64 and publish\linux-x64
pause
