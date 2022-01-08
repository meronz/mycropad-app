# Mycropad Application

[![App Build](https://github.com/meronz/mycropad-app/actions/workflows/app-build.yaml/badge.svg)](https://github.com/meronz/mycropad-app/actions/workflows/app-build.yaml)

## How to build
### Requirements
- dotnet v5.0
- npm (for postcss)
- Electron.NET CLI tool (more info [here](https://github.com/ElectronNET/Electron.NET)).

```sh
cd src/Mycropad.App
electronize build /target <target>
# where target is one of win, osx, linux
```

---
## Other Repos
- [Mycropad Firmware](https://github.com/meronz/mycropad-fw)
- [Mycropad Hardware](https://github.com/meronz/mycropad-hw)
