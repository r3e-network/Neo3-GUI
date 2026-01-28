# Neo3-GUI

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/6.0)
[![Neo](https://img.shields.io/badge/Neo-3.9.1-green.svg)](https://github.com/neo-project/neo)

A cross-platform graphical user interface for Neo N3 blockchain. Neo3-GUI provides an intuitive way to interact with the Neo blockchain, manage wallets, deploy smart contracts, and more.

## Features

- **Blockchain Explorer** - Browse blocks, transactions, and assets
- **Wallet Management** - Create, import, and manage Neo wallets
- **Smart Contracts** - Deploy, invoke, and manage contracts
- **Multi-signature Support** - Create and sign multi-sig transactions
- **Network Selection** - Switch between MainNet, TestNet, and private networks
- **Multi-language** - English and Chinese support

## Requirements

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (v16 or later recommended)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (for development)

## Quick Start

### 1. Clone the repository

```bash
git clone https://github.com/neo-project/Neo3-GUI.git
cd Neo3-GUI
```

### 2. Install frontend dependencies

```bash
cd neo3-gui/neo3-gui/ClientApp
npm install
```

### 3. Run with Visual Studio

Open `neo3-gui/neo3-gui.sln` in Visual Studio and press **F5** to start debugging.

### 4. Or run from command line

```bash
cd neo3-gui/neo3-gui
dotnet run
```

## Building for Release

To build a distributable package:

```bash
cd neo3-gui/neo3-gui
./publish.sh        # Linux/Windows
./publish.macos.sh  # macOS
```

Output will be in `neo3-gui/neo3-gui/ClientApp/build-electron`.

## Documentation

- [Usage Guide](docs/USAGE.md) - How to use Neo3-GUI
- [Configuration Guide](docs/CONFIGURATION.md) - Configuration options
- [Changelog](Changelog.md) - Version history

## Neo 3.9.1 Compatibility

This version is aligned with Neo 3.9.1:
- Neo NuGet dependency pinned to 3.9.1
- LevelDBStore plugin 3.9.1 bundled during build

## Project Structure

```
Neo3-GUI/
├── neo3-gui/
│   ├── neo3-gui/           # Main application
│   │   ├── ClientApp/      # React frontend
│   │   ├── Common/         # Shared utilities
│   │   ├── Models/         # Data models
│   │   ├── Services/       # Backend services
│   │   └── config.*.json   # Network configurations
│   └── neo3-gui.tests/     # Unit tests
├── docs/                   # Documentation
└── Changelog.md            # Release notes
```

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Links

- [Neo Official Website](https://neo.org/)
- [Neo Documentation](https://docs.neo.org/)
- [Neo GitHub](https://github.com/neo-project)
