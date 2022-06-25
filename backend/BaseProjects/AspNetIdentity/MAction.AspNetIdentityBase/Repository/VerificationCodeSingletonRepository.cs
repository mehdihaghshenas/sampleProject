using System.Text;

namespace MAction.AspNetIdentity.Base.Repository;

public class VerificationCodeSingletonRepository : IVerificationCodeSingletonRepository
{
    protected List<(string VerificationCode, string Email, DateTime ExpiryDate)> VerificationCodes { get; set; } = new();

    public string CreateAndSaveVerificationCode(string email, ushort expireAfterMinutes = 2)
    {
        RemoveExpiredVerificationCodes();
        string code = CreateVerificationCode();
        VerificationCodes.Add((
            VerificationCode: code,
            Email: email,
            ExpiryDate: DateTime.Now.AddMinutes(expireAfterMinutes)));
        return code;
    }

    public bool IsVerificationCodeValid(string email, string verificationCode)
    {
        RemoveExpiredVerificationCodes();
        return VerificationCodes.Any(v => v.VerificationCode == verificationCode && v.Email == email && v.ExpiryDate >= DateTime.Now);
    }

    private void RemoveExpiredVerificationCodes() => VerificationCodes?.RemoveAll(v => v.ExpiryDate < DateTime.Now);

    private string CreateVerificationCode(byte characterCount = 6)
    {
        StringBuilder stringBuilder = new StringBuilder();
        Random rnd = new Random();
        for (int i = 0; i < characterCount; i++)
            stringBuilder.Insert(index: i, value: rnd.Next(minValue: 0, maxValue: 9).ToString());
        return stringBuilder.ToString();
    }
}