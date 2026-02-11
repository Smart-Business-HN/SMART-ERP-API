namespace SMART.ERP.Application.Services.CardEncryptionService;

public interface ICardEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string cipherText);
}
