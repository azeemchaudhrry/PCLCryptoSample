# PCLCrypto to .NET 8 Cryptography Migration

This repository contains the migration of legacy PCLCrypto cryptographic implementations to modern .NET 8 cryptography APIs. The migration ensures compatibility while leveraging the improved security and performance features of .NET 8's cryptographic stack.

## Overview

The migration covers the following cryptographic operations:
1. Checksum calculation (SHA-256)
2. Key derivation (PBKDF2-SHA1)
3. AES encryption (CBC mode with PKCS7 padding)
4. AES decryption

## Migration Details

### Checksum Calculation
- Migrated from `WinRTCrypto.HashAlgorithmProvider` to `System.Security.Cryptography.SHA256`
- Maintains identical hash output
- Improved performance with modern implementations

### Key Derivation
- Migrated from `NetFxCrypto.DeriveBytes` to `System.Security.Cryptography.Rfc2898DeriveBytes`
- Maintains compatibility with PBKDF2-SHA1
- Proper resource disposal with `using` statements

### AES Encryption/Decryption
- Migrated from `WinRTCrypto.SymmetricKeyAlgorithmProvider` to `System.Security.Cryptography.Aes`
- Maintains CBC mode and PKCS7 padding for compatibility
- Explicit IV handling for better security control

## Security Considerations

While maintaining compatibility with legacy implementations, consider these security enhancements for production use:
1. Use unique random IVs for each encryption operation
2. Implement message authentication (HMAC)
3. Increase PBKDF2 iteration count (recommend 100,000+)
4. Consider upgrading to stronger algorithms where possible:
   - PBKDF2-SHA256 instead of PBKDF2-SHA1
   - AES-GCM instead of AES-CBC

## Usage

Replace the old PCLCrypto implementations with their .NET 8 counterparts:

```csharp
// Old PCLCrypto implementation
var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
byte[] hash = hasher.HashData(inputBytes);

// New .NET 8 implementation
byte[] hash = SHA256.HashData(inputBytes);
```

## Dependencies

- .NET 8 SDK
- Original PCLCrypto package (for testing only)

## Migration Testing

Run the included test programs to verify the migration:
1. `dotnet run --project PCLCryptoSample`

## Contributing

Feel free to submit issues and enhancement requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
