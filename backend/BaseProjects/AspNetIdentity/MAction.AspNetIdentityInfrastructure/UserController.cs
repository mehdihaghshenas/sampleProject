using System.Security.Claims;
using MAction.AspNetIdentity.Base;
using MAction.AspNetIdentity.Base.ViewModel;
using MAction.AspNetIdentity.Infrastructure.Models;
using MAction.BaseClasses;
using MAction.BaseClasses.Exceptions;
using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAction.AspNetIdentity.Infrastructure;

[Route("api/auth/[action]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IBaseServiceDependencyProvider _baseServiceDependencyProvider;

    public UserController(IUserService userService,IBaseServiceDependencyProvider baseServiceDependencyProvider)
    {
        _userService = userService;
        _baseServiceDependencyProvider = baseServiceDependencyProvider;
    }


    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> RegisterClient([FromBody] RegisterViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.RegisterUserAsync(model, SystemRolesEnum.User, cancellationToken);
            return Ok(result);
        }

        return BadRequest("some parameters are not valid");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.LoginUserAsync(model, cancellationToken);
            return Ok(result);
        }

        return BadRequest("some parameters are not valid");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> SendRegistrationVerificationCode([FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var result = await _userService.SendRegistrationVerificationCode(email, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> SendLoginVerificationCode([FromQuery] string email,
        CancellationToken cancellationToken)
    {
        var result = await _userService.SendLoginVerificationCode(email, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> RegisterUserByEmailAndVerificationCode(
        [FromBody] EmailAndVerificationCode_Request request, CancellationToken cancellationToken)
    {
        var result =
            await _userService.RegisterUserByEmailAndVerificationCode(request.Email, request.VerificationCode,
                cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> LoginUserByVerificationCode([FromBody] EmailAndVerificationCode_Request request,
        CancellationToken cancellationToken)
    {
        var result =
            await _userService.LoginUserByVerificationCode(request.Email, request.VerificationCode, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{userId}")]
    public async Task<ActionResult<IEnumerable<Claim>>> GetUserPolicies([FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserPolicy(userId, cancellationToken);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult> SetUserPolicies([FromBody] UserPoliciesInputModel model,
        CancellationToken cancellationToken)
    {
         await _userService.SetPolicies(model.AddPolicies, model.RemovePolicies, model.UserId, cancellationToken);

        return Ok();
    }
}