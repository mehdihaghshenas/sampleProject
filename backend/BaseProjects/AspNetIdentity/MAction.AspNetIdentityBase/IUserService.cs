using MAction.AspNetIdentity.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.Base;

public interface IUserService
{
    Task<RegisteredUserDto> LoginUserAsync(LoginViewModel model, CancellationToken cancellationToken);
    Task<RegisteredUserDto> RegisterUserAsync(RegisterViewModel model, SystemRolesEnum role, CancellationToken cancellationToken);

    Task<bool> SendRegistrationVerificationCode(string email, CancellationToken cancellationToken);
    Task<SendLoginVerificationCode_Response> SendLoginVerificationCode(string email, CancellationToken cancellationToken);
    Task<RegisteredUserDto> RegisterUserByEmailAndVerificationCode(string email, string verificationCode, CancellationToken cancellationToken);
    Task<RegisteredUserDto> LoginUserByVerificationCode(string email, string verificationCode, CancellationToken cancellationToken);
}
