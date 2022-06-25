using System.Threading.Tasks;
using MAction.SampleOnion.Service.ViewModel.Input;

namespace MAction.SampleOnion.Service.MyProfile
{
    public interface IMyProfileService
    {
        Task<bool> UpdateAsync(MyProfileInputModel model);
        Task<bool> UpdateProfilePasswordAsync(MyProfileChangePasswordInputModel model);
    }
}