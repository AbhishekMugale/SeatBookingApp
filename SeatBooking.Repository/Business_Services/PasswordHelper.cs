using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace SeatBooking.Repository.Business_Services;
public class PasswordHelper
{
    // Hashes the input password using SHA256 algorithm
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
        }
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    
    // Compares a plaintext password with a hashed password
    public static bool ComparePasswords(string plainTextPassword, string hashedPassword)
    {
        if (string.IsNullOrEmpty(plainTextPassword))
        {
            throw new ArgumentNullException(nameof(plainTextPassword), "Password cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(hashedPassword))
        {
            throw new ArgumentNullException(nameof(hashedPassword), "Hashed password cannot be null or empty.");
        }

        // Hash the input password using the same hashing algorithm
        string hashedInputPassword = HashPassword(plainTextPassword);

        // Compare the hashes
        return string.Equals(hashedInputPassword, hashedPassword, StringComparison.OrdinalIgnoreCase);
    }
}
