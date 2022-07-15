using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace MAction.AspNetIdentity.Base.Repository;

public class VerificationCodeSingletonRepository : IVerificationCodeSingletonRepository
{
    private readonly IWebHostEnvironment _environment;

    public VerificationCodeSingletonRepository(IWebHostEnvironment environment) => _environment = environment;

    protected ConcurrentDictionary<string, (string VerificationCode, DateTime ExpiryDate)> VerificationCodes { get; set; } = new();

    public string CreateAndSaveVerificationCode(string email, ushort expireAfterMinutes = 5)
    {
        string code;
        if (VerificationCodes.ContainsKey(email))
            code = VerificationCodes[email].VerificationCode;
        else code = CreateVerificationCode();
        (string VerificationCode, DateTime ExpiryDate) verificationCodeValue = new()
        {
            VerificationCode = code,
            ExpiryDate = DateTime.Now.AddMinutes(expireAfterMinutes)
        };
        VerificationCodes.AddOrUpdate(email, verificationCodeValue, (key, oldValue) => verificationCodeValue);
        return code;
    }

    public bool IsVerificationCodeValid(string email, string verificationCode) =>
        VerificationCodes.ContainsKey(email) &&
        VerificationCodes[email].VerificationCode == verificationCode &&
        VerificationCodes[email].ExpiryDate >= DateTime.Now;

    private string CreateVerificationCode(byte characterCount = 6)
    {
        if (_environment.IsDevelopment())
            return "123456";
        else
        {
            StringBuilder stringBuilder = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < characterCount; i++)
                stringBuilder.Insert(index: i, value: rnd.Next(minValue: 0, maxValue: 9).ToString());
            return stringBuilder.ToString();
        }
    }
}