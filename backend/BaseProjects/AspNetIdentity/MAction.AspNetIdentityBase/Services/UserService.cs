using System.Text;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MAction.BaseClasses.Helpers;
using MAction.BaseClasses.Exceptions;
using MAction.AspNetIdentity.Base.ViewModel;
using MAction.AspNetIdentity.Base.Repository;
using MAction.BaseClasses;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using System.Linq;
using System.Linq.Dynamic.Core;
using MAction.AspNetIdentity.Base.Entities;

namespace MAction.AspNetIdentity.Base.Services;

public class UserService<TUser, TRole, TKey> : IUserService
    where TUser : IdentityUser<TKey>, IUser, IBaseEntity, new()
    where TRole : IdentityRole<TKey>, IRole, new()
    where TKey : IEquatable<TKey>
{
    private readonly SignInManager<TUser> _signInManager;
    private readonly UserManager<TUser> _userManager;
    private readonly RoleManager<TRole> _roleManager;
    private readonly IUserEmailSender _currentEmailSender;
    private readonly IVerificationCodeSingletonRepository _verificationCodeRepository;
    private readonly IJWTService _jwtServices;
    private readonly IBaseServiceDependencyProvider _dependencyProvider;
    private readonly IRoleService _roleService;


    public UserService(//IUserRepository userRepository, 
        SignInManager<TUser> signInManager,
        UserManager<TUser> userManager, RoleManager<TRole> roleManager,
        IJWTService jwtServices, IUserEmailSender currentEmailSender,
        IVerificationCodeSingletonRepository verificationCodeRepository, IBaseServiceDependencyProvider dependencyProvider, IRoleService roleService)
    {
        //_userRepository = userRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtServices = jwtServices;
        _currentEmailSender = currentEmailSender;
        _verificationCodeRepository = verificationCodeRepository;
        _dependencyProvider = dependencyProvider;
        _roleService = roleService;
    }

    private TUser GetByEmailOrPhoneNumber(string input)
    {
        var users = _userManager.Users.Where(p => p.Email == input || p.PhoneNumber == input || p.UserName == input);
        return users.FirstOrDefault();
    }
    public async Task<RegisteredUserDto> RegisterUserAsync(RegisterViewModel model, SystemRolesEnum role,
        CancellationToken cancellationToken)
    {
        var isRepeatedEmailPhone = _userManager.Users
            .FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber || x.Email == model.Email);
        if (isRepeatedEmailPhone != null)
        {
            throw new BadRequestException("can not use this phone or email");
        }

        if (model == null)
            throw new BadRequestException("model is null");


        if (model.Password != model.ConfirmPassword)
            throw new BadRequestException("passwords do not match");

        string username = GetUserName(model.Email, model.PhoneNumber);

        if (string.IsNullOrEmpty(username))
            throw new BadRequestException("Username is empty");

        var user = GetByEmailOrPhoneNumber(model.PhoneNumber);

        if (user != null)
            throw new BadRequestException("PhoneNumber is already taken");

        user = GetByEmailOrPhoneNumber(model.Email);

        if (user != null)
            throw new BadRequestException("Email is already taken");

        TUser newUser = new TUser
        {
            UserName = username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
        newUser.SetRequiredDateForInsert(_dependencyProvider);

        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (result.Succeeded)
        {
            await _roleManager.CreateAsync(new TRole()
            { Name = role.ToString(), NormalizedName = role.ToString().ToUpper() });
            await _userManager.AddToRoleAsync(newUser, role.ToString());
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            token = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(token));

            //TODO Complete Structures
            var confimationLink =
                Path.Combine("Test.dafatreeshoma.com.api", "Auth", "EmailValidation").Replace(@"\", "/") + "?Code=" +
                token + "&EmailorPhoneNumber=" + newUser.Email;

            var res = new TUser();
            List<string> Messages = new List<string>();

            try
            {
                await _currentEmailSender.SendEmailAsync(
                    newUser.Email,
                    "Confirm your email",
                    $"please confirm your email by clicking the link bellow : <br /><a href= '{confimationLink}' >Confirm Your Email here </a> or pate this to your browser : <br/> {confimationLink}"
                );
                Messages.Add($"Register Succeed");
                Messages.Add($"Confirmation Email sent to {newUser.Email} .");
            }
            catch
            {
                Messages.Add($"Register Not Succeed");
                Messages.Add($"Confirmation Email Not sent to {newUser.Email} .");
            }
            finally
            {
                res = await _userManager.FindByNameAsync(username);
                //res = _userRepository.GetAll().FirstOrDefault(u => u.NormalizedUserName == username.ToUpper());
            }

            return new RegisteredUserDto()
            {
                Email = res.Email,
                FirstName = res.FirstName,
                FullName = res.FullName,
                LastName = res.LastName,
                UserId = res.Id,
                IsRequiresTwoFactor = res.TwoFactorEnabled,
            };
        }

        throw new BadRequestException(string.Join(',', result.Errors.Select(p => p.Description)));
    }

    public async Task<RegisteredUserDto> LoginUserAsync(LoginViewModel model, CancellationToken cancellationToken)
    {
        SignInResult result;
        TUser user;
        if (string.IsNullOrEmpty(model.EmailorPhoneNumber))
            throw new BadRequestException("UserName is empty.");

        user = GetByEmailOrPhoneNumber(model.EmailorPhoneNumber);


        if (user == null)
            throw new UnauthorizedException("Username or password invalid");
        if (!user.EmailConfirmed)
        {
            throw new ForbiddenExpection("email not verified yet.");
        }

        result = await _signInManager.PasswordSignInAsync(user, model.Password, true, lockoutOnFailure: false);

        var roles = await _userManager.GetRolesAsync(user);
        if (result.Succeeded)
        {
            var token = await _jwtServices.GenerateTokenAsync(user.Id);
            var registeredUser = new RegisteredUserDto();

            registeredUser.Token = token;

            registeredUser.UserId = user.Id;
            registeredUser.FullName = user.FullName;
            registeredUser.FirstName = user.FirstName;
            registeredUser.LastName = user.LastName;
            registeredUser.Email = user.Email;
            registeredUser.Role = string.Join(',', roles);
            return registeredUser;
        }
        //To do complete Two factor

        if (result.IsLockedOut)
        {
            throw new BadRequestException("User account locked out.");
        }

        throw new UnauthorizedException("invalid username or password. please try again.");
    }

    public async Task<bool> SendRegistrationVerificationCode(string email, CancellationToken cancellationToken)
    {
        CheckEmail(email);

        TUser user = GetByEmailOrPhoneNumber(email);
        if (user is not null)
            throw new BadRequestException("Email is already taken");

        string verificationCode = _verificationCodeRepository.CreateAndSaveVerificationCode(email);
        await _currentEmailSender.SendEmailAsync(
            toEmailAddress: email,
            emailSubject: "Registration Verification Code",
            verificationCode: verificationCode);

        return true;
    }

    public async Task<SendLoginVerificationCode_Response> SendLoginVerificationCode(string email,
        CancellationToken cancellationToken)
    {
        CheckEmail(email);

        var user = GetByEmailOrPhoneNumber(email);
        if (user is null)
            throw new NotFoundException("User with this email not found ");

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            string verificationCode = _verificationCodeRepository.CreateAndSaveVerificationCode(email);
            await _currentEmailSender.SendEmailAsync(
                toEmailAddress: email,
                emailSubject: "Login Verification Code",
                verificationCode: verificationCode);
            return new SendLoginVerificationCode_Response(userHasPassword: false, sendVerificationCode: true);
        }

        return new SendLoginVerificationCode_Response(userHasPassword: true, sendVerificationCode: false);
    }

    public async Task<RegisteredUserDto> RegisterUserByEmailAndVerificationCode(string email, string verificationCode,
        CancellationToken cancellationToken)
    {
        CheckEmail(email);
        if (string.IsNullOrEmpty(verificationCode))
            throw new BadRequestException("VerificationCode is empty.");
        TUser user = GetByEmailOrPhoneNumber(email);
        if (user is not null)
            throw new BadRequestException("Email is already taken");

        if (_verificationCodeRepository.IsVerificationCodeValid(email, verificationCode))
        {
            TUser newUser = new TUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false
            };
            newUser.SetRequiredDateForInsert(_dependencyProvider);

            IdentityResult result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                TRole userRole = await _roleManager.FindByNameAsync(SystemRolesEnum.User.ToString());
                if (userRole is null)
                {
                    userRole = new TRole()
                    {
                        Name = SystemRolesEnum.User.ToString(),
                        NormalizedName = SystemRolesEnum.User.ToString().ToUpper()
                    };
                    await _roleManager.CreateAsync(userRole);
                }

                await _userManager.AddToRoleAsync(newUser, userRole.Name.ToLower());

                await _signInManager.SignInAsync(newUser, isPersistent: true);

                string token = await _jwtServices.GenerateTokenAsync(newUser.Id);
                return new RegisteredUserDto()
                {
                    Token = token,
                    UserId = newUser.Id,
                    Email = newUser.Email,
                };
            }
            else throw new BadRequestException(string.Join(',', result.Errors.Select(p => p.Description)));
        }

        throw new BadRequestException("VerificationCode or Email is not valid");
    }

    public async Task<RegisteredUserDto> LoginUserByVerificationCode(string email, string verificationCode,
        CancellationToken cancellationToken)
    {
        CheckEmail(email);
        if (string.IsNullOrEmpty(verificationCode))
            throw new BadRequestException("VerificationCode is empty.");

        if (_verificationCodeRepository.IsVerificationCodeValid(email, verificationCode) is not true)
            throw new BadRequestException("Verification code is not valid");

        TUser user = GetByEmailOrPhoneNumber(email);
        if (user is null)
            throw new UnauthorizedException("User with this email not found");

        await _signInManager.SignInAsync(user, isPersistent: true);

        string token = await _jwtServices.GenerateTokenAsync(user.Id);
        return new RegisteredUserDto()
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            LastName = user.LastName,
            FullName = user.FullName,
            FirstName = user.FirstName,
        };
    }

    public async Task<RegisteredUserDto> RegisterAdminUserAsync(RegisterViewModel model, string role,
        CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToManageUsers))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToManageUsers}");

        // Execute the validator
        var validator = new RegisterViewModel.RegisterAdminValidator();
        var results = await validator.ValidateAsync(model, cancellationToken);

        // Inspect any validation failures.
        var success = results.IsValid;
        var failures = results.Errors;

        if (!success)
            throw new BadRequestException(failures.FirstOrDefault()?.ErrorMessage);

        if (model == null)
            throw new BadRequestException("model is null");


        if (model.Password != model.ConfirmPassword)
            throw new BadRequestException("passwords do not match");

        var username = GetUserName(model.Email, model.PhoneNumber);

        if (string.IsNullOrEmpty(username))
            throw new BadRequestException("Username is empty");

        var user = GetByEmailOrPhoneNumber(model.PhoneNumber);

        if (user != null)
            throw new BadRequestException("PhoneNumber is already taken");

        user = GetByEmailOrPhoneNumber(model.Email);

        if (user != null)
            throw new BadRequestException("Email is already taken");

        var newUser = new TUser
        {
            UserName = username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
        newUser.SetRequiredDateForInsert(_dependencyProvider);

        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(',', result.Errors.Select(p => p.Description)));

        await _userManager.AddToRoleAsync(newUser, role);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

        await _userManager.ConfirmEmailAsync(newUser, token);

        var res = await _userManager.FindByNameAsync(username);
        res.PhoneNumberConfirmed = true;

        await _userManager.UpdateAsync(res);

        return new RegisteredUserDto
        {
            Email = res.Email,
            FirstName = res.FirstName,
            FullName = res.FullName,
            LastName = res.LastName,
            UserId = res.Id,
            IsRequiresTwoFactor = res.TwoFactorEnabled,
            Role = role
        };
    }

    public async Task<ActionResult<bool>> UpdatePasswordAsync(string userId, string newPassword,
        CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToManageUsers))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToManageUsers}");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new ForbiddenExpection("Invalid Requested User!");
        if (await _userManager.HasPasswordAsync(user))
            await _userManager.RemovePasswordAsync(user);
        await _userManager.AddPasswordAsync(user, newPassword);

        user.PasswordHash = new PasswordHasher<TUser>().HashPassword(new TUser(), newPassword);

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<IEnumerable<Claim>> GetUserPolicy(string userId, CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToViewPermission))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToViewPermission}");

        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.GetClaimsAsync(user);

        return result;
    }

    public async Task SetPolicies(IEnumerable<string> addPolicies, IEnumerable<string> removePolicies,
        string userId, CancellationToken cancellationToken)
    {
        if (!_dependencyProvider.IsCurrentUserAuthorize(SecurityPolicies.AllowToManagePermission))
            throw new ForbiddenExpection($"You need {SecurityPolicies.AllowToManagePermission}");

        var user = await _userManager.FindByIdAsync(userId);
        var currentClaims = await _userManager.GetClaimsAsync(user);

        foreach (var add in addPolicies)
        {
            var claim = new Claim(PolicyLoader.CustomClaimTypes.ClaimType, add);
            if (currentClaims.FirstOrDefault(claim2 => claim2.Value == claim.Value) != null)
                continue;
            await _userManager.AddClaimAsync(user, claim);
        }

        foreach (var delete in removePolicies)
        {
            var claim = new Claim(PolicyLoader.CustomClaimTypes.ClaimType, delete);
            if (currentClaims.FirstOrDefault(claim2 => claim2.Value == claim.Value) != null)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }
        }
    }

    private static string GetUserName(string email, string phoneNumber)
    {
        var username = string.Empty;

        if (!string.IsNullOrEmpty(email))
        {
            if (IsEmailValid(email))
                username = email;
        }
        else if (!string.IsNullOrEmpty(phoneNumber))
            username = phoneNumber;

        return username;
    }

    public static bool IsEmailValid(string emailaddress)
    {
        try
        {
            MailAddress m = new MailAddress(emailaddress);

            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private void CheckEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BadRequestException("Email is empty.");

        if (IsEmailValid(email) is not true)
            throw new BadRequestException("Email is not valid.");
    }
}