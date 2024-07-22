using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class CryptoUtility
{
    private static string encryptionKey = PlayerPrefs.GetString("Encryption", GenerateEncryptionKey());

    public static string GenerateEncryptionKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] randomBytes = new byte[32]; // 32 bytes for AES-256
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }

    public static string EncryptString(string key, string plainText)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }
        return Convert.ToBase64String(array);
    }

    public static string DecryptString(string key, string cipherText)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}

public class SecureDataStorage
{
    private string filePath = Path.Combine(Application.persistentDataPath, "data.json");

    public void SaveData(object data)
    {
        var jsonData = JsonConvert.SerializeObject(data);
        var encryptedData = Encrypt(jsonData); // Implement Encrypt method to encrypt your data
        File.WriteAllText(filePath, encryptedData);
    }

    public T LoadData<T>()
    {
        if (File.Exists(filePath))
        {
            var encryptedData = File.ReadAllText(filePath);
            var jsonData = Decrypt(encryptedData); // Implement Decrypt method to decrypt your data
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
        return default(T);
    }

    private string Encrypt(string data)
    {
        // Implement encryption logic
        return data; // Placeholder
    }

    private string Decrypt(string data)
    {
        // Implement decryption logic
        return data; // Placeholder
    }
}