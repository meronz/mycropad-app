#!/bin/sh
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh -c 8.0 -InstallDir ./dotnet
./dotnet/dotnet --version
echo "dotnet installed!"
./dotnet/dotnet workload install wasm-tools

echo "downloading tailwindcss"
curl -sLO https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-x64
chmod +x tailwindcss-linux-x64
mkdir -p /tmp/bin
mv tailwindcss-linux-x64 /tmp/bin/tailwindcss
export PATH=$PATH:/tmp/bin

echo "Build"
./dotnet/dotnet build src/Mycropad.App.Shared/ -c Release
./dotnet/dotnet publish src/Mycropad.App.Wasm/ -c Release -o output