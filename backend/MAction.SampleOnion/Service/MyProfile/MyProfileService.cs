using AutoMapper;
using MAction.BaseMongoRepository;
using MAction.SampleOnion.Service.ViewModel.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using MAction.BaseClasses.Exceptions;
using MAction.AspNetIdentity.Mongo.Domain;

namespace MAction.SampleOnion.Service.MyProfile
{
    public class MyProfileService : IMyProfileService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyProfileService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> UpdateAsync(MyProfileInputModel model)
        {
            ApplicationUser user = model.ToEntity();
            var requestUser = await userManager.FindByNameAsync(user.UserName);
            var loggedInUserId = userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if(requestUser == null || loggedInUserId != requestUser.Id.ToString())
                throw new ForbiddenExpection("Invalid Requested User!");

            if(user.Email != requestUser.Email)
                throw new ForbiddenExpection("Email cannot be changed!");
            
            requestUser.FirstName = user.FirstName;
            requestUser.LastName = user.LastName;
            requestUser.OrganizationName = user.OrganizationName;
            requestUser.PhoneNumber = user.PhoneNumber;

            var identiryResult = await userManager.UpdateAsync(requestUser);

            if(identiryResult.Succeeded)
                return true;
            else
                return false;
        }

         public async Task<bool> UpdateProfilePasswordAsync(MyProfileChangePasswordInputModel model)
        {
             ApplicationUser user = model.ToEntity();
            var requestUser = await userManager.FindByNameAsync(user.UserName);
            var loggedInUserId = userManager.GetUserId(_httpContextAccessor.HttpContext.User);
            if(requestUser == null || loggedInUserId != requestUser.Id.ToString())
                throw new ForbiddenExpection("Invalid Requested User!");

            requestUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(new ApplicationUser(), model.Password);

            var identiryResult = await userManager.UpdateAsync(requestUser);

            if(identiryResult.Succeeded)
                return true;
            else
                return false;
        }

    }
}