using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using POSDb.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSDb.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 🔹 Roles
            string[] roles = { "Admin", "Warehouse Manager","Store Access","Staff" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 🔹 Admin User
            var adminPhone = "8210997961";

            // 🔹 Phone number se user find karo
            var adminUser = userManager.Users
                .FirstOrDefault(u => u.PhoneNumber == adminPhone);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminPhone,          // username phone number rakh diya
                    Email = "admin@gmail.com",      // email optional hai but dena better hai
                    FullName = "Admin",
                    PhoneNumber = adminPhone,
                    PhoneNumberConfirmed = true     // optional (OTP skip karne ke liye)
                };

                await userManager.CreateAsync(user, "Admin@123");

                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
