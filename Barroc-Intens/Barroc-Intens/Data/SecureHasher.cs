using System;
using System.Security.Cryptography;

/// <summary>
/// Hashes passwords using PBKDF2.
/// (Based on https://stackoverflow.com/a/73125177)
/// </summary>
/// <example>
/// <![CDATA[
/// // Hash:
/// string password = "uns4f3p4ssw0rd";
/// string hashed = SecureHasher.Hash(password);
/// 
/// // Verify:
/// string enteredPassword = "uns4f3p4ssw0rd";
/// bool isPasswordCorrect = SecureHasher.Verify(enteredPassword, hashed);
/// ]]>
/// </example>
public static class SecureHasher
{
    /// <summary>
    /// Size of salt.
    /// </summary>
    private const int saltSize = 16; // 128 bits

    /// <summary>
    /// Size of hash.
    /// </summary>
    private const int keySize = 32; // 256 bits

    /// <summary>
    /// Delimiter used to seperate hash, salt, iterations and algorithm in a hashed password.
    /// </summary>
    private const char segmentDelimiter = ':';

    /// <summary>
    /// The hashing algorithm used.
    /// </summary>
    private static readonly HashAlgorithmName algorithm = HashAlgorithmName.SHA512;

    /// <summary>
    /// Creates a hash from a password.
    /// </summary>
    /// <param name="password">The password to be hashed.</param>
    /// <param name="iterations">Number of iterations (10000 by default).</param>
    /// <returns>The hash.</returns>
    public static string Hash(string password, int iterations = 10000)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            iterations,
            algorithm,
            keySize
        );

        // Combine with delimiter, so we can later split and retrieve the hash, salt, iterations and algorithm.
        // This way we can change the algorithm or number of iterations without breaking existing hashes.
        return string.Join(
            segmentDelimiter,
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt),
            iterations,
            algorithm
        );
    }

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="inputPassword">The password to hash and then check.</param>
    /// <param name="hashedPassword">The hashed password to check against.</param>
    /// <returns>Could be verified?</returns>
    public static bool Verify(string inputPassword, string hashedPassword)
    {
        // Split to get the hash, salt, iterations and algorithm.
        string[] segments = hashedPassword.Split(segmentDelimiter);

        byte[] hash = Convert.FromBase64String(segments[0]);
        byte[] salt = Convert.FromBase64String(segments[1]);
        int iterations = int.Parse(segments[2]);
        HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

        // Hash the password with the same salt, iterations and algorithm.
        // We compare that to the existing hashed password. If they match, the password is correct.
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
            inputPassword,
            salt,
            iterations,
            algorithm,
            hash.Length
        );

        // By using CryptographicOperations.FixedTimeEquals we prevent timing attacks.
        // A timing attack is when an attacker can guess the password by measuring the time it takes to hash the password.
        // This is because the hash function will return as soon as it finds a mismatching byte.
        // With that information the attacker can guess the password byte by byte.
        // By using CryptographicOperations.FixedTimeEquals we make sure that the hash function will always run for the same amount of time.
        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}
