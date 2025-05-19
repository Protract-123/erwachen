# erwachen

erwachen is a .NET based wake on lan cli tool which has alias support, allowing you to easily wake devices with a single 
line command without creating your own list of hardware addresses. Inspired from [jpoliv's](https://github.com/jpoliv) 
[wakeonlan](https://github.com/jpoliv/wakeonlan) project.

## Getting Started

Follow the instructions below in order to get the latest version of erwachen on your machine. If you don't want to build yourself,
you can use the premade binaries found under releases.

### Prerequisites

Requirements to build the software
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Git](https://git-scm.com/downloads)

### Installing

A step by step series of examples that tell you how to build erwachen

Clone the git repository

    git clone https://github.com/Protract-123/erwachen.git

Access the cloned directory

    cd ./erwachen

Then access the directory with the .csproj file (yes it's the same directory name)

    cd ./erwachen

Given that .NET is installed, you can run the following to build for your platform. Some common platforms are win-x64, osx-x64 and linux-x64. 
A list of all platforms can be found [here](https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.NETCore.Platforms/src/PortableRuntimeIdentifierGraph.json)
    
    dotnet publish -c Release -r [platform] --self-contained true

## The CLI

    erwachen [OPTIONS] <COMMAND>

### Examples:
    erwachen wake [alias] -h 00:00:00:00:00:00 -i 192.168.1.1 -p 10
    erwachen alias add <alias> <hardwareAddress>
    erwachen alias remove <alias>
    erwachen alias list

### Commands:
    alias - Allows you to add, remove or list your current aliases
    wake - Wake a hardware address with alias or arbitrary address

## License

This project is licensed under the [MIT License](LICENSE.md)
see the [LICENSE.md](LICENSE.md) file for
details

## Acknowledgments

- [jpoliv](https://github.com/jpoliv) - Made [wakeonlan](https://github.com/jpoliv/wakeonlan), which inspired this project
