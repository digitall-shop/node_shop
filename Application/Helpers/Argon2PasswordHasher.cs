using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;


namespace Application.Helpers;

public static class PasswordHasher
{
    /// <summary>
    /// Hashes the specified password using the Argon id algorithm and returns
    /// an encoded string containing the hash, salt, and configuration parameters.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>
    /// A string in the standard Argon2 encoded format, including salt and
    /// algorithm parameters.
    /// </returns>
    public static string HashPassword(string password)
    {
        // Generate a 16-byte cryptographically strong random salt
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);

        // Configure Argon2id parameters
        var config = new Argon2Config()
        {
            Type       = Argon2Type.HybridAddressing, 
            Version    = Argon2Version.Nineteen,
            Password   = Encoding.UTF8.GetBytes(password),
            Salt       = salt,
            TimeCost   = 4,              // Number of iterations
            MemoryCost = 1 << 16,        // 64 MiB
            Lanes      = Environment.ProcessorCount,
            Threads    = Environment.ProcessorCount,
            HashLength = 32              // 32-byte output
        };

        // Perform the hashing operation
        using var argon2     = new Argon2(config);
        using var hashResult = argon2.Hash();

        // Encode the hash and parameters into a single string
        return config.EncodeString(hashResult.Buffer);
    }

    /// <summary>
    /// Verifies that the provided plaintext password matches the encoded
    /// Argon2 hash string.
    /// </summary>
    /// <param name="encodedHash">
    /// The Argon2 encoded hash string to verify against (must include salt
    /// and parameters).
    /// </param>
    /// <param name="password">The plaintext password to verify.</param>
    /// <returns>
    /// <c>true</c> if the password matches the hash; otherwise, <c>false</c>.
    /// </returns>
    public static bool VerifyPassword(string? encodedHash, string? password)
    {
        return Argon2.Verify(encodedHash, password);
    }
    
}