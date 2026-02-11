using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using SMART.ERP.Domain.Settings;

namespace SMART.ERP.Application.Services.CardEncryptionService;

public class CardEncryptionService : ICardEncryptionService
{
    private readonly byte[] _key;

    public CardEncryptionService(IOptions<EncryptionSettings> encryptionSettings)
    {
        _key = Convert.FromBase64String(encryptionSettings.Value.CardEncryptionKey);
        if (_key.Length != 32)
        {
            throw new ArgumentException("La clave de encriptación debe ser de 256 bits (32 bytes).");
        }
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // Prepend IV to ciphertext
        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Extract IV (first 16 bytes)
        var iv = new byte[16];
        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        aes.IV = iv;

        var cipherBytes = new byte[fullCipher.Length - iv.Length];
        Buffer.BlockCopy(fullCipher, iv.Length, cipherBytes, 0, cipherBytes.Length);

        using var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return System.Text.Encoding.UTF8.GetString(plainBytes);
    }
}
