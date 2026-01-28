# Neo3-GUI Configuration Guide

Neo3-GUI uses JSON configuration files for different networks.

## Configuration Files

| File | Network |
|------|---------|
| `config.mainnet.json` | Neo MainNet |
| `config.testnet.json` | Neo TestNet |
| `config.private.json` | Private Network |

## Configuration Structure

### ApplicationConfiguration

```json
{
  "ApplicationConfiguration": {
    "Logger": { ... },
    "Storage": { ... },
    "P2P": { ... },
    "UnlockWallet": { ... },
    "Contracts": { ... },
    "Plugins": { ... }
  }
}
```

### Logger Settings

```json
"Logger": {
  "Path": "Logs",
  "ConsoleOutput": false,
  "Active": false
}
```

| Option | Description |
|--------|-------------|
| `Path` | Log file directory |
| `ConsoleOutput` | Print logs to console |
| `Active` | Enable/disable logging |

### Storage Settings

```json
"Storage": {
  "Engine": "LevelDBStore",
  "Path": "Data_LevelDB_Mainnet"
}
```

| Option | Description |
|--------|-------------|
| `Engine` | Storage engine (LevelDBStore) |
| `Path` | Blockchain data directory |

### P2P Network Settings

```json
"P2P": {
  "Port": 10333,
  "EnableCompression": true,
  "MinDesiredConnections": 10,
  "MaxConnections": 40,
  "MaxConnectionsPerAddress": 3
}
```

| Option | Description | MainNet | TestNet |
|--------|-------------|---------|---------|
| `Port` | P2P listening port | 10333 | 20333 |
| `EnableCompression` | Compress network data | true | true |
| `MinDesiredConnections` | Minimum peer connections | 10 | 10 |
| `MaxConnections` | Maximum peer connections | 40 | 40 |

### Auto-Unlock Wallet

```json
"UnlockWallet": {
  "Path": "",
  "Password": "",
  "IsActive": false
}
```

| Option | Description |
|--------|-------------|
| `Path` | Wallet file path |
| `Password` | Wallet password |
| `IsActive` | Auto-unlock on startup |

> ⚠️ **Security Warning**: Storing passwords in config is not recommended for production.

## ProtocolConfiguration

### Network Settings

```json
"ProtocolConfiguration": {
  "Network": 860833102,
  "MillisecondsPerBlock": 15000,
  "MaxTransactionsPerBlock": 512
}
```

| Network | ID |
|---------|-----|
| MainNet | 860833102 |
| TestNet | 894710606 |

### Seed Nodes

MainNet seeds:
```
seed1.neo.org:10333
seed2.neo.org:10333
seed3.neo.org:10333
seed4.neo.org:10333
seed5.neo.org:10333
```

## Common Tasks

### Change Data Directory

Edit `Storage.Path` in config file.

### Change P2P Port

Edit `P2P.Port` (ensure firewall allows it).

### Enable Logging

Set `Logger.Active` to `true`.
