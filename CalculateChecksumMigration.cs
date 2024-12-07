using PCLCrypto;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PCLCryptoSample
{
    internal class CalculateChecksumMigration
    {
        public static void Test()
        {
            string[] testStrings = new string[]
            {
            "Hello, World!",
            "Test123",
            "Some longer string with special characters !@#$%^&*()",
            "",  // empty string
            "1234567890"
            };

            Console.WriteLine("Comparing PCLCrypto vs Modern .NET Cryptography implementations\n");
            Console.WriteLine("Format: Input String => Old Hash : New Hash : Match?\n");

            foreach (var testString in testStrings)
            {
                // Calculate using old implementation
                byte[] oldHash = OldCalculateChecksum(testString);

                // Calculate using new implementation
                byte[] newHash = NewCalculateChecksum(testString);

                // Compare results
                bool areEqual = oldHash.SequenceEqual(newHash);

                // Convert hashes to hex strings for display
                string oldHashHex = BitConverter.ToString(oldHash).Replace("-", "");
                string newHashHex = BitConverter.ToString(newHash).Replace("-", "");

                Console.WriteLine($"Input: {testString}");
                Console.WriteLine($"Old Implementation: {oldHashHex}");
                Console.WriteLine($"New Implementation: {newHashHex}");
                Console.WriteLine($"Match: {(areEqual ? "✓ YES" : "✗ NO")}");
                Console.WriteLine();
            }
        }

        // Old implementation using PCLCrypto
        public static byte[] OldCalculateChecksum(string s)
        {
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(PCLCrypto.HashAlgorithm.Sha256);
            byte[] inputBytes = Encoding.UTF8.GetBytes(s);
            byte[] hash = hasher.HashData(inputBytes);
            return hash;
        }

        // New implementation using modern .NET
        public static byte[] NewCalculateChecksum(string s)
        {
            var hasher = SHA256.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(s);
            return hasher.ComputeHash(inputBytes);
        }
    }
}
