namespace SMART.ERP.Application.Services.NewEncryptionService
{
    public interface INewEncryptionService
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);

        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}
