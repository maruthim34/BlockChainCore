using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

// Simple Block class supporting both PoW and PoS
public class SimpleBlock
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; }
    public string PreviousHash { get; set; }
    public string Hash { get; set; }
    public int Nonce { get; set; } // Used for mining (PoW)
    public string Validator { get; set; } // Used for PoS

    public SimpleBlock(int index, DateTime timestamp, string data, string previousHash, string validator = null)
    {
        Index = index;
        Timestamp = timestamp;
        Data = data;
        PreviousHash = previousHash;
        Nonce = 0;
        Validator = validator;
    }

    // Calculates the SHA256 hash for the block including nonce
    public string CalculateHash()
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string rawData = Index + Timestamp.ToString() + Data + PreviousHash + Nonce + Validator;
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }

    // Mines the block by finding a hash with required leading zeros (PoW)
    public void MineBlock(int difficulty)
    {
        string target = new string('0', difficulty);
        do
        {
            Nonce++;
            Hash = CalculateHash();
        } while (!Hash.StartsWith(target));
    }
}

// Simple Blockchain class supporting PoW and PoS
public class SimpleBlockchain
{
    public List<SimpleBlock> Chain { get; set; }
    public int Difficulty { get; set; } = 4; // Initial difficulty
    private int blocksPerAdjustment = 5; // Adjust difficulty every 5 blocks
    private TimeSpan targetBlockTime = TimeSpan.FromSeconds(10); // Target time per block
    private DateTime lastAdjustmentTime;
    public bool UsePoS { get; set; } = false; // Switch between PoW and PoS

    // Validators and their stakes for PoS
    public Dictionary<string, int> Validators { get; set; } = new Dictionary<string, int>
    {
        { "Alice", 50 },
        { "Bob", 30 },
        { "Carol", 20 }
    };
    private Random rand = new Random();

    public SimpleBlockchain()
    {
        Chain = new List<SimpleBlock> { CreateGenesisBlock() };
        lastAdjustmentTime = DateTime.Now;
    }

    public SimpleBlock CreateGenesisBlock()
    {
        var block = new SimpleBlock(0, DateTime.Now, "Genesis Block", "0", "Genesis");
        block.Hash = block.CalculateHash();
        return block;
    }

    public SimpleBlock GetLatestBlock()
    {
        return Chain[Chain.Count - 1];
    }

    // Adds a new block using PoW or PoS
    public void AddBlock(string data)
    {
        SimpleBlock latestBlock = GetLatestBlock();
        if (UsePoS)
        {
            // Proof of Stake: select validator based on stake
            string validator = SelectValidatorByStake();
            SimpleBlock newBlock = new SimpleBlock(latestBlock.Index + 1, DateTime.Now, data, latestBlock.Hash, validator);
            newBlock.Hash = newBlock.CalculateHash();
            Chain.Add(newBlock);
            Console.WriteLine($"Block created by validator: {validator}");
        }
        else
        {
            // Proof of Work: mine the block
            SimpleBlock newBlock = new SimpleBlock(latestBlock.Index + 1, DateTime.Now, data, latestBlock.Hash);
            Console.WriteLine($"Mining block {newBlock.Index}...");
            DateTime start = DateTime.Now;
            newBlock.MineBlock(Difficulty);
            DateTime end = DateTime.Now;
            Chain.Add(newBlock);
            Console.WriteLine($"Block mined! Hash: {newBlock.Hash} (Nonce: {newBlock.Nonce}, Time: {(end-start).TotalSeconds:F2}s)");
            AdjustDifficultyIfNeeded(end);
        }
    }

    // Selects a validator randomly, weighted by stake (PoS)
    private string SelectValidatorByStake()
    {
        int totalStake = 0;
        foreach (var stake in Validators.Values)
            totalStake += stake;
        int pick = rand.Next(totalStake);
        int cumulative = 0;
        foreach (var kvp in Validators)
        {
            cumulative += kvp.Value;
            if (pick < cumulative)
                return kvp.Key;
        }
        return "Unknown";
    }

    // Adjusts difficulty based on mining speed (PoW only)
    private void AdjustDifficultyIfNeeded(DateTime currentTime)
    {
        if (UsePoS) return; // No difficulty adjustment in PoS
        if (Chain.Count % blocksPerAdjustment == 0)
        {
            TimeSpan actualTime = currentTime - lastAdjustmentTime;
            double expectedTime = blocksPerAdjustment * targetBlockTime.TotalSeconds;
            if (actualTime.TotalSeconds < expectedTime * 0.5)
            {
                Difficulty++;
                Console.WriteLine($"Difficulty increased to {Difficulty}");
            }
            else if (actualTime.TotalSeconds > expectedTime * 1.5 && Difficulty > 1)
            {
                Difficulty--;
                Console.WriteLine($"Difficulty decreased to {Difficulty}");
            }
            lastAdjustmentTime = currentTime;
        }
    }

    // Validates the integrity of the blockchain
    public bool IsValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            SimpleBlock current = Chain[i];
            SimpleBlock previous = Chain[i - 1];
            if (current.Hash != current.CalculateHash()) return false;
            if (current.PreviousHash != previous.Hash) return false;
            if (!UsePoS && !current.Hash.StartsWith(new string('0', Difficulty))) return false;
        }
        return true;
    }
}

// Main program to interact with the simple blockchain
class Program
{
    static void Main(string[] args)
    {
        // Simple blockchain with mining (PoW) and PoS simulation
        SimpleBlockchain blockchain = new SimpleBlockchain();
        while (true)
        {
            // Display menu options
            Console.WriteLine("1. Add Block");
            Console.WriteLine("2. Display Blockchain");
            Console.WriteLine("3. Validate Blockchain");
            Console.WriteLine("4. Switch Mode (Current: " + (blockchain.UsePoS ? "PoS" : "PoW") + ")");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            string option = Console.ReadLine();
            if (option == "1")
            {
                // Add a new block with user-provided data
                Console.Write("Enter block data: ");
                string data = Console.ReadLine();
                blockchain.AddBlock(data);
                Console.WriteLine();
            }
            else if (option == "2")
            {
                // Display all blocks in the blockchain
                foreach (var block in blockchain.Chain)
                {
                    Console.WriteLine($"Index: {block.Index}");
                    Console.WriteLine($"Timestamp: {block.Timestamp}");
                    Console.WriteLine($"Data: {block.Data}");
                    Console.WriteLine($"Previous Hash: {block.PreviousHash}");
                    Console.WriteLine($"Hash: {block.Hash}");
                    if (blockchain.UsePoS)
                        Console.WriteLine($"Validator: {block.Validator}");
                    else
                        Console.WriteLine($"Nonce: {block.Nonce}");
                    Console.WriteLine();
                }
                if (!blockchain.UsePoS)
                    Console.WriteLine($"Current Difficulty: {blockchain.Difficulty}\n");
                else
                {
                    Console.WriteLine("Validators and Stakes:");
                    foreach (var kvp in blockchain.Validators)
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    Console.WriteLine();
                }
            }
            else if (option == "3")
            {
                // Validate the blockchain's integrity
                Console.WriteLine(blockchain.IsValid() ? "Blockchain is valid!" : "Blockchain is NOT valid!");
            }
            else if (option == "4")
            {
                // Switch between PoW and PoS
                blockchain.UsePoS = !blockchain.UsePoS;
                Console.WriteLine("Mode switched to " + (blockchain.UsePoS ? "PoS" : "PoW") + "\n");
            }
            else if (option == "5")
            {
                // Exit the program
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Try again.\n");
            }
        }
    }
}
