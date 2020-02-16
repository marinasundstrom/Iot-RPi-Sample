dotnet publish -r linux-arm /p:ShowLinkerSizeComparison=true 
pushd .\bin\Debug\netcoreapp3.1\linux-arm\publish
pscp -pw raspberry -v -r .\* pi@raspberrypi.local:/home/pi/Blink
popd