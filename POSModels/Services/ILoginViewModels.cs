using POSModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.Services
{
    public interface ILoginViewModels
    {
        public Task<AllResponseMessage> AllStaffLogin(LoginModel lm);
        public Task<AllResponseMessage> AllUserLogin(LoginModel lm);
        public Task<AllResponseMessage> SaveStaff(StaffModel sm);
        public Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm);
    }
}
