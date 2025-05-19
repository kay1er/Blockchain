using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

public class Block
{
    public int Index { get; set; }           // Vị trí của khối trong chuỗi
    public DateTime Timestamp { get; set; }   // Thời điểm khối được tạo
    public string Data { get; set; }          // Dữ liệu chứa trong khối
    public string PreviousHash { get; set; }  // Hash của khối trước đó
    public string Hash { get; set; }          // Hash của khối hiện tại

    public Block(int index, DateTime timestamp, string data, string previousHash = "")
    {
        Index = index;
        Timestamp = timestamp;
        Data = data;
        PreviousHash = previousHash;
        Hash = CalculateHash();  // Tự động tính toán hash khi khối được tạo
    }

    public string CalculateHash()
    {
        // Kết hợp tất cả dữ liệu của khối thành một chuỗi
        string rawData = Index + Timestamp.ToString() + Data + PreviousHash;

        // Sử dụng SHA256 để tạo hash
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            StringBuilder builder = new StringBuilder();

            // Chuyển đổi byte array thành chuỗi hex
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));

            return builder.ToString();
        }
    }
}

public class Blockchain
{
    public List<Block> Chain { get; set; }  // Danh sách các khối

    public Blockchain()
    {
        Chain = new List<Block>();
        Chain.Add(CreateGenesisBlock());  // Tạo khối genesis khi khởi tạo blockchain
    }

    // Tạo khối genesis (khối đầu tiên)
    private Block CreateGenesisBlock()
    {
        return new Block(0, DateTime.Now, "Genesis Block", "0");
    }

    // Lấy khối mới nhất trong chuỗi
    public Block GetLatestBlock()
    {
        return Chain[Chain.Count - 1];
    }

    // Thêm khối mới vào chuỗi
    public void AddBlock(string data)
    {
        Block latestBlock = GetLatestBlock();
        Block newBlock = new Block(latestBlock.Index + 1, DateTime.Now, data, latestBlock.Hash);
        Chain.Add(newBlock);
    }

    // Kiểm tra tính hợp lệ của blockchain
    public bool IsValid()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            Block current = Chain[i];
            Block previous = Chain[i - 1];

            // Kiểm tra hash hiện tại có đúng không
            if (current.Hash != current.CalculateHash())
                return false;

            // Kiểm tra liên kết với khối trước
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
        // Tạo blockchain mới
        Blockchain myChain = new Blockchain();

        // Thêm các khối vào blockchain
        myChain.AddBlock("Dữ liệu 1");
        myChain.AddBlock("Dữ liệu 2");

        // Hiển thị thông tin từng khối
        foreach (var block in myChain.Chain)
        {
            Console.WriteLine($"Index: {block.Index}");
            Console.WriteLine($"Time: {block.Timestamp}");
            Console.WriteLine($"Data: {block.Data}");
            Console.WriteLine($"Hash: {block.Hash}");
            Console.WriteLine($"Previous: {block.PreviousHash}");
            Console.WriteLine("-----------------------------------");
        }

        // Kiểm tra tính hợp lệ của blockchain
        Console.WriteLine("Blockchain hợp lệ? " + myChain.IsValid());
    }
}
