using MAction.AspNetIdentity.Mongo.Domain;
using MAction.SampleOnion.Service.Company;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.SampleOnion.Infrastructure
{
    [Obsolete]
    [Route("api/[controller]")]
    [ApiController]
    public class SampleDataController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICompanyServiceWithExpression _companyService;

        public SampleDataController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICompanyServiceWithExpression companyService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _companyService = companyService;
        }

        [HttpPost("Create")]
        public async Task<ActionResult<bool>> Create()
        {
            if (_userManager.Users.FirstOrDefault() != null)
                return false;

            var adminRole = new ApplicationRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            var userRole = new ApplicationRole
            {
                Name = "User",
                NormalizedName = "USER"
            };
            await _roleManager.CreateAsync(adminRole);
            await _roleManager.CreateAsync(userRole);
            //admin user
            var admin = new ApplicationUser()
            {
                FirstName = "Super",
                LastName = "Admin",
                UserName = "admin",
                Email = "admin@admin.com",
            };
            await _userManager.CreateAsync(admin, "adminadmin");
            await _userManager.AddToRoleAsync(admin, "admin");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(admin);
            await _userManager.ConfirmEmailAsync(admin, token);
            //user user
            var user = new ApplicationUser()
            {
                FirstName = "user",
                LastName = "user",
                UserName = "user",
                Email = "user@user.com",
            };
            await _userManager.CreateAsync(user, "useruser");
            await _userManager.AddToRoleAsync(user, "user");
            token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _userManager.ConfirmEmailAsync(user, token);

            await _companyService.InsertAsync(new Service.ViewModel.Input.SaleCompanyInputModel() { Id = 1000, Name = "Test" });
            await _companyService.InsertAsync(new Service.ViewModel.Input.SaleCompanyInputModel() { Id = 1001, Name = "Mehdi Haghshenas" });

            return true;

        }
    }
}
