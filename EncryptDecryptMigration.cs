using PCLCrypto;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PCLCryptoSample
{
    internal class EncryptDecryptMigration
    {
        private static readonly string Password = "MyTestPassword123!";
        private static readonly byte[] Salt = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

        public static void Test()
        {
            var testStrings = new[]
            {
            "Hello, World!",
            "Test123",
            @"{""key"": ""value"", ""number"": 42}",  // JSON string
            "Some longer string with special characters !@#$%^&*()",
            ""  // empty string
        };

            Console.WriteLine("Testing PCLCrypto vs Modern .NET AES Encryption/Decryption\n");

            foreach (var originalText in testStrings)
            {
                Console.WriteLine($"Original Text: {originalText}");

                // Test old implementation (PCLCrypto)
                byte[] oldEncrypted = OldEncryptAes(originalText);
                string oldDecrypted = OldDecryptData(oldEncrypted);

                // Test new implementation (.NET 8)
                byte[] newEncrypted = NewEncryptAes(originalText);
                string newDecrypted = NewDecryptData(newEncrypted);

                // Compare results
                bool encryptionMatches = oldEncrypted.SequenceEqual(newEncrypted);
                bool decryptionMatches = oldDecrypted == newDecrypted;
                bool roundTripSuccessful = originalText == newDecrypted;

                // Display results
                Console.WriteLine("Old Implementation:");
                Console.WriteLine($"  Encrypted (hex): {BitConverter.ToString(oldEncrypted).Replace("-", "")}");
                Console.WriteLine($"  Decrypted: {oldDecrypted}");

                Console.WriteLine("New Implementation:");
                Console.WriteLine($"  Encrypted (hex): {BitConverter.ToString(newEncrypted).Replace("-", "")}");
                Console.WriteLine($"  Decrypted: {newDecrypted}");

                Console.WriteLine("Results:");
                Console.WriteLine($"  Encryption matches: {(encryptionMatches ? "✓ YES" : "✗ NO")}");
                Console.WriteLine($"  Decryption matches: {(decryptionMatches ? "✓ YES" : "✗ NO")}");
                Console.WriteLine($"  Round-trip successful: {(roundTripSuccessful ? "✓ YES" : "✗ NO")}\n");
            }
        }

        // Helper method for key derivation
        private static byte[] CreateDerivedKey(int keyLengthInBytes = 32, int iterations = 1000)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(Password, Salt, iterations, HashAlgorithmName.SHA1))
            {
                return pbkdf2.GetBytes(keyLengthInBytes);
            }
        }

        // Old implementation using PCLCrypto
        private static byte[] OldEncryptAes(string jsonString)
        {
            byte[] key = CreateDerivedKey();
            var aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(PCLCrypto.SymmetricAlgorithm.AesCbcPkcs7);
            var symmetricKey = aes.CreateSymmetricKey(key);
            return WinRTCrypto.CryptographicEngine.Encrypt(symmetricKey, Encoding.UTF8.GetBytes(jsonString));
        }

        private static string OldDecryptData(byte[] data)
        {
            var aes = WinRTCrypto.SymmetricKeyAlgorithmProvider.OpenAlgorithm(PCLCrypto.SymmetricAlgorithm.AesCbcPkcs7);
            byte[] key = CreateDerivedKey();
            var symmetricKey = aes.CreateSymmetricKey(key);
            byte[] bytes = WinRTCrypto.CryptographicEngine.Decrypt(symmetricKey, data);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        // New implementation using modern .NET
        private static byte[] NewEncryptAes(string jsonString)
        {
            byte[] key = CreateDerivedKey();
            byte[] iv = new byte[16];  // AES block size is 16 bytes

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] plaintext = Encoding.UTF8.GetBytes(jsonString);
                    return encryptor.TransformFinalBlock(plaintext, 0, plaintext.Length);
                }
            }
        }

        private static string NewDecryptData(byte[] data)
        {
            byte[] key = CreateDerivedKey();
            byte[] iv = new byte[16];  // AES block size is 16 bytes

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(data, 0, data.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }
    }
}
