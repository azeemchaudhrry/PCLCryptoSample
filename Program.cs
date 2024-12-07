using System;

namespace PCLCryptoSample
{
    class Program
    {
        static void Main(string[] args)
        {
            CalculateChecksumMigration.Test();
            DerivedKeyMigration.Test();
            EncryptDecryptMigration.Test();

            Console.ReadKey();
        }
    }
}
