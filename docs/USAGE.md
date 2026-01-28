# Neo3-GUI Usage Guide

This guide covers the main features of Neo3-GUI.

## Getting Started

### First Launch

1. Start Neo3-GUI
2. Select your network (MainNet, TestNet, or Private)
3. Wait for blockchain synchronization

### Network Selection

- **MainNet** - Production network with real assets
- **TestNet** - Testing network for development
- **Private** - Local private network

## Wallet Management

### Creating a New Wallet

1. Go to **Wallet** menu
2. Click **Create Wallet**
3. Enter a secure password
4. Save the wallet file (.json) securely
5. **Important**: Backup your private key!

### Opening an Existing Wallet

1. Go to **Wallet** menu
2. Click **Open Wallet**
3. Select your wallet file
4. Enter your password

### Importing a Wallet

You can import using:
- **Private Key** - 64-character hex string
- **WIF** - Wallet Import Format (starts with K or L)
- **NEP-2 Key** - Encrypted key (requires password)

### Wallet Operations

- **View Balance** - See all assets in your wallet
- **Transfer** - Send NEO, GAS, or other tokens
- **Batch Transfer** - Send to multiple addresses
- **View Transactions** - Transaction history
- **Change Password** - Update wallet password

## Blockchain Explorer

### Viewing Blocks

1. Go to **Blockchain** section
2. Browse the block list
3. Click any block to see details:
   - Block height and hash
   - Timestamp
   - Transaction count
   - Transactions list

### Searching

- **By Block Height** - Enter block number
- **By Transaction Hash** - Enter tx hash
- **By Address** - Enter Neo address

### Viewing Transactions

Transaction details include:
- Transaction hash
- Block confirmation
- Sender and receiver
- Amount transferred
- Application logs (for contract calls)

## Smart Contracts

### Deploying a Contract

1. Go to **Contract** → **Deploy**
2. Select your `.nef` file
3. Select your `manifest.json` file
4. Review deployment cost
5. Confirm and sign transaction

### Invoking a Contract

1. Go to **Contract** → **Invoke**
2. Enter contract hash or search
3. Select method from manifest
4. Fill in parameters
5. Choose parameter types if needed
6. Execute (test invoke or send tx)

### Contract Management

- View contract details by hash
- See all transactions related to a contract
- Migrate or destroy contracts (if owner)

## Multi-Signature

### Creating Multi-Sig Address

1. Go to **Advanced** → **Multi-Signature**
2. Add public keys of participants
3. Set minimum signatures required
4. Generate multi-sig address

### Signing Transactions

1. Create transaction from multi-sig address
2. Export unsigned transaction
3. Each participant signs
4. Collect signatures
5. Broadcast when threshold met

## Advanced Features

### Voting

1. Open your wallet
2. Go to **Advanced** → **Vote**
3. Select a candidate
4. Confirm vote transaction

### Data Converter

Convert between formats:
- String ↔ Hex String
- Address ↔ Script Hash
- Big Integer ↔ Hex

## Tips

- Always backup your wallet and private keys
- Test on TestNet before MainNet
- Keep your software updated
- Use strong passwords
