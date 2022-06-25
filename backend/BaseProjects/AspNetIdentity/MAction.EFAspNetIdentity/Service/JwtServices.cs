using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MAction.AspNetIdentity.Base;
using MAction.AspNetIdentity.EFCore.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace MAction.AspNetIdentity.EFCore.Service;

public class JwtServices: IJWTService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    public JwtServices(IOptionsSnapshot<JwtSettings> jwtSettings, RoleManager<ApplicationRole> rolemanager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = rolemanager;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }
    public async Task<string> GenerateTokenAsync(object userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var secretKey = Encoding.UTF8.GetBytes(_jwtSettings.IdentitySecretKey);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
            SecurityAlgorithms.HmacSha256Signature);

        var claims = await _getClaimsAsync(user);
        var descriptor = new SecurityTokenDescriptor()
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(_jwtSettings.NotBeforeMinutes),
            Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpirationMinutes),
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims),
        };

        var jwtHandler = new JwtSecurityTokenHandler();
        var securityToken = jwtHandler.CreateToken(descriptor);
        var token = jwtHandler.WriteToken(securityToken);
        return token;
    }


    private async Task<IList<Claim>> GetRoleClaimsAsync(IList<string> roles)
    {
        List<Claim> roleClaims = new();
        foreach (var r in roles)
        {
            var role = await _roleManager.FindByNameAsync(r);
            roleClaims.AddRange(await _roleManager.GetClaimsAsync(role));
        }
        return roleClaims;
    }
    private async Task<List<Claim>> _getClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim> { };

        var roles = await _userManager.GetRolesAsync(user);
        var isAdmin = roles.Contains(SystemRolesEnum.Admin.ToString());
        if (isAdmin)
        {
            var list = PolicyLoader.GetAllPolicies();
            foreach (var item in list)
            {
                claims.Add(new Claim(PolicyLoader.CustomClaimTypes.ClaimType, item.Value));
            }
            var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
            claims.AddRange(userRoles);
        }
        else
        {
            var userRoles = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();
            var userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
            var roleClaims = await GetRoleClaimsAsync(roles).ConfigureAwait(false);
            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);
            claims.AddRange(userRoles);
        }
        var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
        claims.AddRange(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("fullName", user.FirstName + " " + user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Aud,_jwtSettings.Audience),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer),
            new Claim(securityStampClaimType, user.SecurityStamp)});
        return claims;
    }
}