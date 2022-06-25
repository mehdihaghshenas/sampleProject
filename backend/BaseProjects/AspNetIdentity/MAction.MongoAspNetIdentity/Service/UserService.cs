using System.Text;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MAction.AspNetIdentity.Base;
using MAction.BaseClasses.Exceptions;
using MAction.AspNetIdentity.Mongo.Domain;
using MAction.AspNetIdentity.Base.ViewModel;
using MAction.AspNetIdentity.Base.Repository;
using MAction.AspNetIdentity.Mongo.Repository;

namespace MAction.AspNetIdentity.Mongo.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUserEmailSender _currentEmailSender;
    private readonly IVerificationCodeSingletonRepository _verificationCodeRepository;
    private readonly IJWTService _jwtServices;


    public UserService(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
        IJWTService jwtServices, IUserEmailSender currentEmailSender, IVerificationCodeSingletonRepository verificationCodeRepository)
    {
        _userRepository = userRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtServices = jwtServices;
        _currentEmailSender = currentEmailSender;
        _verificationCodeRepository = verificationCodeRepository;
    }
    public async Task<RegisteredUserDto> RegisterUserAsync(RegisterViewModel model, SystemRolesEnum role, CancellationToken cancellationToken)
    {
        var isAdminuser = _userRepository.GetAll().FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber || x.Email == model.Email);
        if (isAdminuser != null)
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

        var user = await _userRepository.GetByEmailOrPhoneNumber(model.PhoneNumber, cancellationToken);

        if (user != null)
            throw new BadRequestException("PhoneNumber is already taken");

        user = await _userRepository.GetByEmailOrPhoneNumber(model.Email, cancellationToken);

        if (user != null)
            throw new BadRequestException("Email is already taken");

        ApplicationUser newUser = new ApplicationUser
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

        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (result.Succeeded)
        {
            await _roleManager.CreateAsync(new ApplicationRole() { Name = role.ToString(), NormalizedName = role.ToString().ToUpper() });
            await _userManager.AddToRoleAsync(newUser, role.ToString());
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            token = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(token));

            //TODO Complete Structures
            var confimationLink = Path.Combine("Test.dafatreeshoma.com.api", "Auth", "EmailValidation").Replace(@"\", "/") + "?Code=" + token + "&EmailorPhoneNumber=" + newUser.Email;

            var res = new ApplicationUser();
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
        ApplicationUser user;
        if (string.IsNullOrEmpty(model.EmailorPhoneNumber))
            throw new BadRequestException("UserName is empty.");

        user = await _userRepository.GetByEmailOrPhoneNumber(model.EmailorPhoneNumber, cancellationToken);


        if (user == null)
            throw new UnauthorizedException("Username or password invalid");
        if (!user.EmailConfirmed)
        {
            throw new ForbiddenExpection("email not verified yet.");
        }
        result = await _signInManager.PasswordSignInAsync(user, model.Password, true, lockoutOnFailure: false);



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

        ApplicationUser user = await _userRepository.GetByEmailOrPhoneNumber(email, cancellationToken);
        if (user is not null)
            throw new BadRequestException("Email is already taken");

        string verificationCode = _verificationCodeRepository.CreateAndSaveVerificationCode(email);
        await _currentEmailSender.SendEmailAsync(
            toEmailAddress: email,
            emailSubject: "Registration Verification Code",
            verificationCode: verificationCode);

        return true;
    }
    public async Task<SendLoginVerificationCode_Response> SendLoginVerificationCode(string email, CancellationToken cancellationToken)
    {
        CheckEmail(email);

        ApplicationUser user = await _userRepository.GetByEmailOrPhoneNumber(email, cancellationToken);
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

    public async Task<RegisteredUserDto> RegisterUserByEmailAndVerificationCode(string email, string verificationCode, CancellationToken cancellationToken)
    {
        CheckEmail(email);
        if (string.IsNullOrEmpty(verificationCode))
            throw new BadRequestException("VerificationCode is empty.");
        ApplicationUser user = await _userRepository.GetByEmailOrPhoneNumber(email, cancellationToken);
        if (user is not null)
            throw new BadRequestException("Email is already taken");

        if (_verificationCodeRepository.IsVerificationCodeValid(email, verificationCode))
        {
            ApplicationUser newUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false
            };
            IdentityResult result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                ApplicationRole userRole = await _roleManager.FindByNameAsync(SystemRolesEnum.User.ToString());
                if (userRole is null)
                {
                    userRole = new ApplicationRole() { Name = SystemRolesEnum.User.ToString(), NormalizedName = SystemRolesEnum.User.ToString().ToUpper() };
                    await _roleManager.CreateAsync(userRole);
                }
                await _userManager.AddToRoleAsync(newUser, userRole.Name);

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
        throw new BadRequestException(message: "VerificationCode or Email is not valid");
    }
    public async Task<RegisteredUserDto> LoginUserByVerificationCode(string email, string verificationCode, CancellationToken cancellationToken)
    {
        CheckEmail(email);
        if (string.IsNullOrEmpty(verificationCode))
            throw new BadRequestException("VerificationCode is empty.");

        if (_verificationCodeRepository.IsVerificationCodeValid(email, verificationCode))
        {
            ApplicationUser user = await _userRepository.GetByEmailOrPhoneNumber(email, cancellationToken);
            if (user is null)
                throw new UnauthorizedException("Not Unauthorize");

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

        throw new UnauthorizedException("Not Unauthorize");
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