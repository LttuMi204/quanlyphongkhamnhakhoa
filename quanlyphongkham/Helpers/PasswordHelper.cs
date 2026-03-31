using System.Security.Cryptography;
using System.Text;

namespace quanlyphongkham.Helpers
{
    public static class PasswordHasher
    {
        private const int SALT_SIZE = 32; // bytes
        private const char SEPARATOR = '$';

        public static string Hash(string plainPassword)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SALT_SIZE);
            var salt = Convert.ToBase64String(saltBytes);

            var hash = ComputeSHA256(salt + plainPassword);

            return $"{salt}{SEPARATOR}{hash}";
        }

        public static bool Verify(string plainPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash) || !storedHash.Contains(SEPARATOR))
                return false;

            var parts = storedHash.Split(SEPARATOR, 2);
            var salt = parts[0];
            var expectedHash = parts[1];

            var actualHash = ComputeSHA256(salt + plainPassword);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(actualHash),
                Encoding.UTF8.GetBytes(expectedHash)
            );
        }

        public static bool IsPlaintext(string storedValue)
        {
            return !storedValue.Contains(SEPARATOR);
        }

        private static string ComputeSHA256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
