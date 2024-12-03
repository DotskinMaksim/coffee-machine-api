using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace CoffeeMachineAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsAdmin { get; set; } = false;

        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            if (!IsPasswordStrong(password))
            {
                throw new ArgumentException("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.");
            }

            PasswordHash = HashPassword(password);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool CheckPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            return HashPassword(password) == PasswordHash;
        }

        public static bool IsPasswordStrong(string password)
        {
            const int MinLength = 8;

            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;

            if (password.Length < MinLength)
            {
                return false;
            }

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpperCase = true;
                else if (char.IsLower(c)) hasLowerCase = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecialChar = true;

                if (hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar)
                {
                    return true;
                }
            }

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }
}