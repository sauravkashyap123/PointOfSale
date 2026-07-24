using POSModels.Models;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface IAuthService
    {
        Task<AllResponseMessage> AllStaffLoginAsync(LoginModel lm);
        Task<AllResponseMessage> AllUserLoginAsync(LoginModel lm);
        Task<AllResponseMessage> SaveStaffAsync(StaffModel sm);
        Task<AllResponseMessage> SaveWarehouseAsync(WarehouseModel wm);
    }
}
