using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class FileEncryptor
{
    static void Main()
    {
        Console.WriteLine("File Encryption and Decryption Tool");

        Console.Write("Enter the path of the file to encrypt/decrypt: ");
        string filePath = Console.ReadLine();

        Console.Write("Enter a password: ");
        string password = Console.ReadLine();

        Console.Write("Choose operation (encrypt/decrypt): ");
        string operation = Console.ReadLine().ToLower();

        try
        {
            if (operation == "encrypt")
            {
                EncryptFile(filePath, password);
                Console.WriteLine("File encrypted successfully.");
            }
            else if (operation == "decrypt")
            {
                DecryptFile(filePath, password);
                Console.WriteLine("File decrypted successfully.");
            }
            else
            {
                Console.WriteLine("Invalid operation. Please enter 'encrypt' or 'decrypt'.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static void EncryptFile(string filePath, string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GenerateKey(password, aesAlg.KeySize);
            aesAlg.IV = aesAlg.Key;

            using (FileStream inputFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream encryptedFileStream = new FileStream(filePath + ".encrypted", FileMode.Create, FileAccess.Write))
            using (ICryptoTransform encryptor = aesAlg.CreateEncryptor())
            using (CryptoStream cryptoStream = new CryptoStream(encryptedFileStream, encryptor, CryptoStreamMode.Write))
            {
                inputFileStream.CopyTo(cryptoStream);
            }
        }
    }

    static void DecryptFile(string filePath, string password)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = GenerateKey(password, aesAlg.KeySize);
            aesAlg.IV = aesAlg.Key;

            using (FileStream encryptedFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (FileStream decryptedFileStream = new FileStream(filePath.Replace(".encrypted", ".decrypted"), FileMode.Create, FileAccess.Write))
            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
            using (CryptoStream cryptoStream = new CryptoStream(decryptedFileStream, decryptor, CryptoStreamMode.Write))
            {
                encryptedFileStream.CopyTo(cryptoStream);
            }
        }
    }

    static byte[] GenerateKey(string password, int keySize)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Adjust the key size if necessary
            Array.Resize(ref key, keySize / 8);

            return key;
        }
    }
}
