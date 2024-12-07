using PCLCrypto;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace PCLCryptoSample
{
    internal class DerivedKeyMigration
    {
        private static readonly string Password = "MyTestPassword123!";
        private static readonly byte[] Salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        public static void Test()
        {
            // Test cases with different key lengths and iterations
            var testCases = new[]
            {
                (keyLength: 32, iterations: 1000),
                (keyLength: 16, iterations: 2000),
                (keyLength: 64, iterations: 500)
            };

            Console.WriteLine("Comparing PCLCrypto vs Modern .NET Key Derivation\n");

            foreach (var (keyLength, iterations) in testCases)
            {
                Console.WriteLine($"Test Case: Key Length = {keyLength} bytes, Iterations = {iterations}");

                // Old implementation
                byte[] oldKey = OldCreateDerivedKey(keyLength, iterations);

                // New implementation
                byte[] newKey = NewCreateDerivedKey(keyLength, iterations);

                // Compare results
                bool areEqual = oldKey.SequenceEqual(newKey);

                // Display results
                Console.WriteLine($"Old Implementation: {BitConverter.ToString(oldKey).Replace("-", "")}");
                Console.WriteLine($"New Implementation: {BitConverter.ToString(newKey).Replace("-", "")}");
                Console.WriteLine($"Match: {(areEqual ? "✓ YES" : "✗ NO")}\n");
            }
        }

        // Old implementation using PCLCrypto
        private static byte[] OldCreateDerivedKey(int keyLengthInBytes = 32, int iterations = 1000)
        {
            byte[] key = NetFxCrypto.DeriveBytes.GetBytes(Password, Salt, iterations, keyLengthInBytes);
            return key;
        }

        // New implementation using modern .NET
        private static byte[] NewCreateDerivedKey(int keyLengthInBytes = 32, int iterations = 1000)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(Password, Salt, iterations, HashAlgorithmName.SHA1))
            {
                return pbkdf2.GetBytes(keyLengthInBytes);
            }
        }
    }
}
