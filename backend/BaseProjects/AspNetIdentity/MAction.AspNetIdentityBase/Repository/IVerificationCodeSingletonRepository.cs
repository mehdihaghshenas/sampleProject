namespace MAction.AspNetIdentity.Base.Repository
{
    public interface IVerificationCodeSingletonRepository
    {
        string CreateAndSaveVerificationCode(string email, ushort expireAfterMinutes = 2);
        bool IsVerificationCodeValid(string email, string verificationCode);
    }
}