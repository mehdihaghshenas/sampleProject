using MAction.AspNetIdentity.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using Microsoft.AspNetCore.Mvc;

namespace MAction.AspNetIdentity.Base;

public interface IUserService
{
    Task<RegisteredUserDto> LoginUserAsync(LoginViewModel model, CancellationToken cancellationToken);
    Task<RegisteredUserDto> RegisterUserAsync(RegisterViewModel model, SystemRolesEnum role, CancellationToken cancellationToken);

    Task<bool> SendRegistrationVerificationCode(string email, CancellationToken cancellationToken);
    Task<SendLoginVerificationCode_Response> SendLoginVerificationCode(string email, CancellationToken cancellationToken);
    Task<RegisteredUserDto> RegisterUserByEmailAndVerificationCode(string email, string verificationCode, CancellationToken cancellationToken);
    Task<RegisteredUserDto> LoginUserByVerificationCode(string email, string verificationCode, CancellationToken cancellationToken);
    Task<RegisteredUserDto> RegisterAdminUserAsync(RegisterViewModel model, string role, CancellationToken cancellationToken);

    Task<ActionResult<bool>> UpdatePasswordAsync(string userId, string newPassword, CancellationToken cancellationToken);

    Task<IEnumerable<Claim>> GetUserPolicy(string userId, CancellationToken cancellationToken);
    Task SetPolicies(IEnumerable<string> addPolicies, IEnumerable<string> removePolicies,
        string userId, CancellationToken cancellationToken);
}
