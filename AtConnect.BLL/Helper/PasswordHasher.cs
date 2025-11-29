using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        // create a strong random salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // derive a 32-byte key using PBKDF2
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 350000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        // combine salt + hash
        byte[] result = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, result, salt.Length, hash.Length);

        // convert to Base64 for DB
        return Convert.ToBase64String(result);
    }
    public static bool Verify(string password, string storedHash)
    {
        byte[] bytes = Convert.FromBase64String(storedHash);

        // extract salt
        byte[] salt = new byte[16];
        Buffer.BlockCopy(bytes, 0, salt, 0, 16);

        // extract hash
        byte[] storedHashBytes = new byte[32];
        Buffer.BlockCopy(bytes, 16, storedHashBytes, 0, 32);

        // hash input with same salt
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 350000, HashAlgorithmName.SHA256);
        byte[] computedHash = pbkdf2.GetBytes(32);

        // compare
        return CryptographicOperations.FixedTimeEquals(storedHashBytes, computedHash);
    }

}
