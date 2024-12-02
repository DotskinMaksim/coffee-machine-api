using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace CoffeeMachineAPI.Models;

public class User
{
    public int Id { get; set; }

    [EmailAddress] [StringLength(255)] public string Email { get; set; }

    public byte[] PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsAdmin { get; set; }

    public void SetPassword(string password)
    {
        byte[] salt = GenerateSalt();
        PasswordHash = HashPassword(password, salt);
    }

    public bool CheckPassword(string password)
    {
        byte[] salt = ExtractSalt(PasswordHash);
        byte[] hash = HashPassword(password, salt);
        return CompareHashes(PasswordHash, hash);
    }

    private byte[] GenerateSalt()
    {
        var salt = new byte[16]; // 128-bit salt
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }

        return salt;
    }

    private byte[] HashPassword(string password, byte[] salt)
    {
        return KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);
    }

    private byte[] ExtractSalt(byte[] hash)
    {
        byte[] salt = new byte[16];
        Array.Copy(hash, 0, salt, 0, 16);
        return salt;
    }

    private bool CompareHashes(byte[] hash1, byte[] hash2)
    {
        for (int i = 0; i < hash1.Length; i++)
        {
            if (hash1[i] != hash2[i])
            {
                return false;
            }
        }

        return true;
    }
}