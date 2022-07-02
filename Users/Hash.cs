using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Users
{
    internal static class Hash
    {
        private static byte[] GenerateSalt()
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using var rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetNonZeroBytes(salt);

            return salt;
        }

        private static string ToHash(string password, byte[] salt)
        {
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
        
        /// <summary>
        /// Returns hashed password in format "salt==hashed="
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Hashed Password</returns>
        public static string GeneratePassword(string password)
        {
            byte[] salt = GenerateSalt();
            string hashed = ToHash(password, salt);

            return $"{Convert.ToBase64String(salt)}{hashed}";
        }
        
        public static bool CompareHashes(string password, string hashedPassword)
        {
            byte[] salt = Convert.FromBase64String(hashedPassword[..24]);
            string hash = ToHash(password, salt);
            string inputtedHashedPassword = $"{Convert.ToBase64String(salt)}{hash}";
            return inputtedHashedPassword.Equals(hashedPassword);
        }
    }
}