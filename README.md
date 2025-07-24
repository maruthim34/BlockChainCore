# Simple Blockchain Core (.NET 8, C# 12)

This project is a simple educational blockchain implementation in C# (.NET 8). It demonstrates how blockchains work internally, including mining (proof-of-work), auto-adjusting difficulty, and a simulation of proof-of-stake (PoS).

## Features
- **Block Mining (Proof of Work):** Each block is mined by finding a hash with a required number of leading zeros (proof-of-work).
- **Proof of Stake (PoS) Simulation:** Blocks are created by randomly selecting a validator, weighted by their stake, with no mining required.
- **Auto-Adjusting Difficulty (PoW only):** The mining difficulty increases or decreases based on how quickly blocks are mined.
- **Block Structure:** Each block contains an index, timestamp, data, previous hash, hash, and either a nonce (PoW) or validator (PoS).
- **Chain Validation:** The blockchain can be validated to ensure integrity.
- **Switchable Modes:** You can switch between PoW and PoS modes in the console menu.

## Proof of Work vs Proof of Stake
- **Proof of Work (PoW):** Miners compete to solve cryptographic puzzles. The first to solve adds the next block. This requires significant computational power and energy.
- **Proof of Stake (PoS):** Validators are chosen to create new blocks based on the amount of cryptocurrency they “stake” (lock up as collateral). PoS is more energy-efficient and secure against some attacks. Ethereum now uses PoS instead of PoW.
- **This Project:** Implements both PoW and a simple PoS simulation. In PoS mode, a validator is selected randomly, weighted by their stake, to create the next block. No mining is performed in PoS mode.

## How It Works
- When you add a block, the app uses the current mode (PoW or PoS) to add it to the chain.
- In PoW mode, blocks are mined and difficulty is auto-adjusted every 5 blocks.
- In PoS mode, a validator is selected by stake to create the block instantly.
- You can switch between PoW and PoS modes at any time using the menu.

## How to Run
1. Build and run the project in Visual Studio or with `dotnet run`.
2. Use the console menu:
   - `1. Add Block`: Enter data and add a new block using the current mode (PoW or PoS).
   - `2. Display Blockchain`: View all blocks, current difficulty (PoW), or validators and stakes (PoS).
   - `3. Validate Blockchain`: Check if the chain is valid.
   - `4. Switch Mode`: Toggle between PoW and PoS.
   - `5. Exit`: Quit the app.

## Key Concepts
- **Block:** Contains data, timestamp, previous hash, hash, and either nonce (PoW) or validator (PoS).
- **Mining (PoW):** Increment nonce until the hash meets the difficulty requirement.
- **Difficulty (PoW):** Number of leading zeros required in the hash.
- **Auto-Adjustment (PoW):** Difficulty changes based on mining speed.
- **Proof of Stake (PoS):** Validator is selected randomly, weighted by their stake, to create the next block.

## Code Structure
- `SimpleBlock`: Represents a block and handles mining (PoW) or validator assignment (PoS).
- `SimpleBlockchain`: Manages the chain, mining, PoS selection, and difficulty adjustment.
- `Program`: Console interface for interaction and mode switching.

## Educational Purpose
This project is for learning and demonstration only. It does not implement advanced blockchain features like transactions, networking, or security.

---
Feel free to explore, modify, and use this code for your blockchain learning journey!
