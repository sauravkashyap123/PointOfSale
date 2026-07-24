using POSModels.Models;
using POSModels.Services;
using System.Threading.Tasks;

namespace POSModels.ViewModels
{
    public class AuthService : IAuthService
    {
        private readonly ILoginViewModels _loginViewModels;

        public AuthService(ILoginViewModels loginViewModels)
        {
            _loginViewModels = loginViewModels;
        }

        public Task<AllResponseMessage> AllStaffLoginAsync(LoginModel lm)
        {
            return _loginViewModels.AllStaffLogin(lm);
        }

        public Task<AllResponseMessage> AllUserLoginAsync(LoginModel lm)
        {
            return _loginViewModels.AllUserLogin(lm);
        }

        public Task<AllResponseMessage> SaveStaffAsync(StaffModel sm)
        {
            return _loginViewModels.SaveStaff(sm);
        }

        public Task<AllResponseMessage> SaveWarehouseAsync(WarehouseModel wm)
        {
            return _loginViewModels.SaveWarehouse(wm);
        }
    }
}
