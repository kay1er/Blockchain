using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

public class Block
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Data { get; set; }
    public string PreviousHash { get; set; }
    public string Hash { get; set; }

    public Block(int index, DateTime timestamp, string data, string previousHash = "")
    {
        Index = index;
        Timestamp = timestamp;
        Data = data;
        PreviousHash = previousHash;
        Hash = CalculateHash();
    }

    public string CalculateHash()
    {
        string rawData = Index + Timestamp.ToString() + Data + PreviousHash;
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
}

public class Blockchain
{
    public List<Block> Chain { get; set; }

    public Blockchain()
    {
        Chain = new List<Block>();
        Chain.Add(CreateGenesisBlock());
    }

    private Block CreateGenesisBlock()
    {
        return new Block(0, DateTime.Now, "Genesis Block", "0");
    }

    public Block GetLatestBlock()
    {
        return Chain[Chain.Count - 1];
    }

    public void AddBlock(string data)
    {
        Block latestBlock = GetLatestBlock();
        Block newBlock = new Block(latestBlock.Index + 1, DateTime.Now, data, latestBlock.Hash);
        Chain.Add(newBlock);
    }

    public bool IsValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            Block current = Chain[i];
            Block previous = Chain[i - 1];

            if (current.Hash != current.CalculateHash())
                return false;
            if (current.PreviousHash != previous.Hash)
                return false;
        }
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Blockchain myChain = new Blockchain();
        myChain.AddBlock("Dữ liệu 1");
        myChain.AddBlock("Dữ liệu 2");

        foreach (var block in myChain.Chain)
        {
            Console.WriteLine($"Index: {block.Index}");
            Console.WriteLine($"Time: {block.Timestamp}");
            Console.WriteLine($"Data: {block.Data}");
            Console.WriteLine($"Hash: {block.Hash}");
            Console.WriteLine($"Previous: {block.PreviousHash}");
            Console.WriteLine("-----------------------------------");
        }

        Console.WriteLine("Blockchain hợp lệ? " + myChain.IsValid());
    }
}
