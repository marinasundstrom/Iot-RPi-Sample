dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true 
pushd ./bin/Debug/netcoreapp3.1/linux-arm/publish
scp -p raspberry * pi@raspberrypi.local:/home/pi/Blink
popd