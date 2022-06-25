using MAction.BaseClasses.InputModels;
using MAction.BaseClasses.OutpuModels;
using MAction.SampleOnion.Service.Company;
using MAction.SampleOnion.Service.MyProfile;
using MAction.SampleOnion.Service.ViewModel.Input;
using MAction.SampleOnion.Service.ViewModel.Output;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MAction.SampleOnion.Infrastructure
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyProfileController : ControllerBase
    {
        private readonly IMyProfileService _myProfileService;

        public MyProfileController(IMyProfileService myProfileService)
        {   
            _myProfileService = myProfileService;
        }

        [HttpPost("Update")]
        public async Task<ActionResult<bool>> Update([FromBody] MyProfileInputModel inputs)
        {
            return await _myProfileService.UpdateAsync(inputs);
        }

        [HttpPost("UpdatePassword")]
        public async Task<ActionResult<bool>> UpdatePassword([FromBody] MyProfileChangePasswordInputModel inputs)
        {
            return await _myProfileService.UpdateProfilePasswordAsync(inputs);
        }
    }
}
