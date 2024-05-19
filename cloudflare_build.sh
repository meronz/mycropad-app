#!/bin/sh
curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh -c 8.0 -InstallDir ./dotnet
./dotnet/dotnet --version
curl -sLO https://github.com/tailwindlabs/tailwindcss/releases/latest/download/tailwindcss-linux-amd64
chmod +x tailwindcss-linux-amd64
mkdir -p /tmp/bin
mv tailwindcss-linux-amd64 /tmp/bin/tailwindcss
export PATH=$PATH:/tmp/bin
./dotnet/dotnet publish -c Release -o output