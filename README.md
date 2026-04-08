# erwachen

erwachen is a Wake-on-LAN CLI tool built with .NET 10. It supports registering devices so you can wake machines by name instead of remembering MAC addresses. It includes both a standard CLI and an interactive terminal interface. Inspired by [jpoliv's](https://github.com/jpoliv) [wakeonlan](https://github.com/jpoliv/wakeonlan).

## Installation

Download a prebuilt binary from [Releases](https://github.com/Protract-123/erwachen/releases) for your platform:
`win-x64`, `win-arm64`, `linux-x64`, `linux-arm64`, `osx-x64`, `osx-arm64`

### Building from source

Requirements: [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download), [Git](https://git-scm.com/downloads)

```shell
git clone https://github.com/Protract-123/erwachen.git
cd erwachen/erwachen
dotnet publish -c Release -r <platform> --self-contained true
```

The binary will be in `bin/Release/net10.0/<platform>/publish/`.

## Usage

```shell
erwachen <command> [options]
```

### Commands

| Command                    | Description                                                         |
| -------------------------- | ------------------------------------------------------------------- |
| `wake <identifier>`        | Send a magic packet to a registered device or arbitrary MAC address |
| `add <name> <macAddress>`  | Register a new device                                               |
| `remove <name>`            | Remove a registered device                                          |
| `list`                     | Show all registered devices                                         |
| `interactive`              | Launch the interactive terminal interface                           |

### Examples

```shell
erwachen wake mypc
erwachen wake AA:BB:CC:DD:EE:FF
erwachen wake mypc -i 192.168.1.255 -p 9
erwachen add mypc AA:BB:CC:DD:EE:FF
erwachen remove mypc
erwachen list
erwachen interactive
```

### Wake options

| Option         | Default           | Description                              |
| -------------- | ----------------- | ---------------------------------------- |
| `-i`, `--ip`   | `255.255.255.255` | IP address to send the magic packet to   |
| `-p`, `--port` | `9`               | UDP port to use                          |

### Configuration

Device registrations are stored in TOML format at `~/.config/erwachen/aliases.toml`, or under `XDG_CONFIG_HOME` if environment variable is set.

## License

This project is licensed under the [MIT License](LICENSE.txt).

## Acknowledgments

- [jpoliv](https://github.com/jpoliv) - Made [wakeonlan](https://github.com/jpoliv/wakeonlan), which inspired this project
