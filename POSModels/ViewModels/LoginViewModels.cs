using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POSDb.Data;
using POSDb.EntityModels;
using POSDb.EntityModels.POSModels.Models;
using POSModels.Models;
using POSModels.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace POSModels.ViewModels
{
    public class LoginViewModels: ILoginViewModels
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinuser;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly DateTime currentdate=DateTime.Now;

        public LoginViewModels(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinuser, IHttpContextAccessor httpContextAccessor,ApplicationDbContext conntext)
        {
            _userManager = userManager;
            _signinuser = signinuser;
            _httpContextAccessor = httpContextAccessor;
            _context = conntext;
        }
        public async Task<AllResponseMessage> AllUserLogin(LoginModel lm)
        {
            var resp = new AllResponseMessage();

            try
            {
                // 1. Initial Validation (Null & Empty Check)
                if (lm == null || string.IsNullOrEmpty(lm.Username) || (string.IsNullOrEmpty(lm.Password)))
                {
                    resp.Result = false;
                    resp.Message = "Username and credentials are required";
                    return resp;
                }

                // Role input format normalize karein
                string requestRole = lm.Role?.ToLower();
                if (requestRole != "admin" && requestRole != "staff" && requestRole != "warehouse manager")
                {
                    resp.Result = false;
                    resp.Message = "Invalid Role selected";
                    return resp;
                }

                // 2. User Find (_userManager using Name/Mobile)
                var user = await _userManager.FindByNameAsync(lm.Username);
                if (user == null || user.LockoutEnabled==false)
                {
                    resp.Result = false;
                    resp.Message = "User Not Found or User Locked";
                    return resp;
                }

                // 3. Credentials Check (Password ya PIN jo bhi input ho)
                string secretToVerify = !string.IsNullOrEmpty(lm.Password)?lm.Password:lm.Password;
                var result = await _signinuser.CheckPasswordSignInAsync(user, secretToVerify, false);
                if (!result.Succeeded)
                {
                    resp.Result = false;
                    resp.Message = $"Invalid Password";
                    return resp;
                }

                // 4. Role Verification (Database roles access mapping)
                var userRoles = await _userManager.GetRolesAsync(user);

                // Exact Role check system according to client tabs
                bool isAuthorized = userRoles.Any(r => r.ToLower().Equals(requestRole, StringComparison.OrdinalIgnoreCase));
                if (!isAuthorized)
                {
                    resp.Result = false;
                    resp.Message = $"Access Denied. You do not have '{lm.Role}' permissions.";
                    return resp;
                }

                // 5. Common Claims Configuration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                    new Claim("FullName", user.FullName ?? ""),
                    new Claim(ClaimTypes.Role, requestRole) // Roles based Auth policies ke liye important hai
                };

                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Cookies Client Engine Authentication Sign-In
                await _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                if(userRoles.Any(r => r.ToLower().Equals("admin", StringComparison.OrdinalIgnoreCase)))
                {
                    resp.redirect = "/Home/Index"; // Admin ke liye specific redirection
                }
                else if (userRoles.Any(r => r.ToLower().Equals("staff", StringComparison.OrdinalIgnoreCase)))
                {
                    resp.redirect = "/Billing/Index"; 
                }
                else if (userRoles.Any(r => r.ToLower().Equals("Warehouse Manager", StringComparison.OrdinalIgnoreCase)))
                {
                    resp.redirect = "/Warehouse/Dashboard"; 
                }
                
               
                // 6. Common Success Return Block
                resp.Result = true;
                resp.Message = "Login Successfully";
               
                resp.Extra = new Dictionary<int, string>
                {
                    { 1, user.Id ?? "" },
                    { 2, user.FullName ?? "" },
                    { 3, user.PhoneNumber ?? "" },
                    { 4, userRoles.First() ?? "" }
                };

                return resp;
            }
            catch (Exception ex)
            {
                return new AllResponseMessage
                {
                    Result = false,
                    Message = "Something went wrong",
                    Error = ex.Message
                };
            }
        }
        public async Task<AllResponseMessage> AllStaffLogin(LoginModel lm)
        {
            try
            {
                string StaffCode = lm.Username;
                AllResponseMessage resp = new AllResponseMessage();

                // 🔹 Validation
                if (lm == null || string.IsNullOrEmpty(StaffCode) || string.IsNullOrEmpty(lm.Password))
                {
                    resp.Result = false;
                    resp.Message = "Mobile or Password is required";
                    return resp;
                }

                // 🔹 User find (अगर Mobile को UserName में store किया है)
                var user = await _userManager.FindByNameAsync(StaffCode);

                if (user == null || user.LockoutEnabled==false)
                {
                    resp.Result = false;
                    resp.Message = "User Not Found or User Locked";
                    return resp;
                }

                // 🔹 Password check
                var result = await _signinuser.CheckPasswordSignInAsync(user, lm.Password, false);

                if (!result.Succeeded)
                {
                    resp.Result = false;
                    resp.Message = "Invalid Username or Password";
                    return resp;
                }

                // Role Check
                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Any(x => x.Equals("Staff", StringComparison.OrdinalIgnoreCase)))
                {
                    resp.Result = false;
                    resp.Message = "Access Denied. Staff Login Only.";
                    return resp;
                }
                // 🔥 Claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
            new Claim("FullName", user.FullName ?? ""),
            new Claim(ClaimTypes.Role, "staff")
        };

                // 🔥 Identity + Principal
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);

                // 🔥 Sign-in
                await _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                // 🔹 Success response
                resp.Result = true;
                resp.Message = "Login Successfully";
                resp.Extra = new Dictionary<int, string>
                    {
                        { 1, user.Id??"" },
                        { 2, user.FullName ?? "" },
                        { 3, user.PhoneNumber ?? "" }
                    };

                return resp; // ✅ IMPORTANT
            }
            catch (Exception ex)
            {
                return new AllResponseMessage
                {
                    Result = false,
                    Message = "Something went wrong",
                    Error = ex.Message
                };
            }
        }


        public async Task<AllResponseMessage> SaveStaff(StaffModel rm)
        {
            try
            {
                AllResponseMessage resp = new AllResponseMessage();

                if (rm == null || string.IsNullOrEmpty(rm.StaffCode) ||
                    string.IsNullOrEmpty(rm.Mobileno) || string.IsNullOrEmpty(rm.Password))
                {
                    resp.Result = false;
                    resp.Message = "StaffCode, Mobile or Password is required";
                    return resp;
                }

                // ================= UPDATE ===================
                if (rm.Id > 0)
                {
                    var staff = await _context.tblStaff
                        .FirstOrDefaultAsync(x => x.Id == rm.Id);

                    if (staff == null)
                    {
                        resp.Result = false;
                        resp.Message = "Staff not found";
                        return resp;
                    }

                    // Duplicate StaffCode check except current record
                    bool staffCodeExist = await _context.tblStaff
                        .AnyAsync(x => x.StaffCode == rm.StaffCode && x.Id != rm.Id);

                    if (staffCodeExist)
                    {
                        resp.Result = false;
                        resp.Message = "Staff Code Already Exist";
                        return resp;
                    }

                    // Identity User
                    var user = await _userManager.FindByNameAsync(staff.StaffCode);

                    if (user == null)
                    {
                        resp.Result = false;
                        resp.Message = "Identity user not found";
                        return resp;
                    }

                    // Username changed
                    if (user.UserName != rm.StaffCode)
                    {
                        var userExist = await _userManager.FindByNameAsync(rm.StaffCode);

                        if (userExist != null)
                        {
                            resp.Result = false;
                            resp.Message = "User already exists with this Staff Code";
                            return resp;
                        }

                        user.UserName = rm.StaffCode;
                    }

                    user.FullName = rm.Name;
                    user.PhoneNumber = rm.Mobileno;
                    user.Email = rm.Email;

                    var updateUser = await _userManager.UpdateAsync(user);

                    if (!updateUser.Succeeded)
                    {
                        resp.Result = false;
                        resp.Message = string.Join(", ", updateUser.Errors.Select(x => x.Description));
                        return resp;
                    }

                    // Password Update
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    user.PasswordHash = passwordHasher.HashPassword(user, rm.Password);

                    await _userManager.UpdateAsync(user);

                    // Update Staff Table
                    staff.Name = rm.Name;
                    staff.Address = rm.Address;
                    staff.AadharCardNo = rm.AadharCardNo;
                    staff.StaffCode = rm.StaffCode;
                    staff.Mobileno = rm.Mobileno;
                    staff.DOB = rm.DOB;
                    staff.Password = rm.Password;
                    staff.Emailid = rm.Email;
                    staff.Shopid = rm.Shopid;

                    _context.tblStaff.Update(staff);
                    await _context.SaveChangesAsync();

                    resp.Result = true;
                    resp.Message = "Staff Updated Successfully";
                    return resp;
                }

                // ================= INSERT ===================

                bool codeExist = await _context.tblStaff
                    .AnyAsync(x => x.StaffCode == rm.StaffCode);

                if (codeExist)
                {
                    resp.Result = false;
                    resp.Message = "Staff Code Already Exist";
                    return resp;
                }

                var existingUser = await _userManager.FindByNameAsync(rm.StaffCode);

                if (existingUser != null)
                {
                    resp.Result = false;
                    resp.Message = "User already exists with this Staff Code";
                    return resp;
                }

                var newUser = new ApplicationUser
                {
                    UserName = rm.StaffCode,
                    PhoneNumber = rm.Mobileno,
                    FullName = rm.Name,
                    Email = rm.Email,
                    Password = rm.Password
                };

                var result = await _userManager.CreateAsync(newUser, rm.Password);

                if (!result.Succeeded)
                {
                    resp.Result = false;
                    resp.Message = string.Join(", ", result.Errors.Select(e => e.Description));
                    return resp;
                }

                await _userManager.AddToRoleAsync(newUser, "Staff");

                EStaffModel es = new EStaffModel
                {
                    Name = rm.Name,
                    Address = rm.Address,
                    DOJ = DateTime.Now,
                    AadharCardNo = rm.AadharCardNo,
                    StaffCode = rm.StaffCode,
                    Mobileno = rm.Mobileno,
                    DOB = rm.DOB,
                    Password = rm.Password,
                    Emailid = rm.Email,
                    Shopid = rm.Shopid
                };

                _context.tblStaff.Add(es);
                await _context.SaveChangesAsync();

                resp.Result = true;
                resp.Message = "Staff Created Successfully";

                resp.Extra = new Dictionary<int, string>
        {
            {1,newUser.Id},
            {2,newUser.FullName},
            {3,newUser.PhoneNumber},
            {4,"Staff"}
        };

                return resp;
            }
            catch (Exception ex)
            {
                return new AllResponseMessage
                {
                    Result = false,
                    Message = "Something went wrong",
                    Error = ex.Message
                };
            }
        }
        public async Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // ── UPDATE ─────────────────────────────────────────────────────────
                if (wm.Id > 0)
                {
                    var existing = await _context.tblwarehouse
                        .FirstOrDefaultAsync(x => x.Id == wm.Id);

                    if (existing == null)
                    {
                        return new AllResponseMessage
                        {
                            Result = false,
                            Message = "Warehouse not found."
                        };
                    }

                    // Update entity fields
                    existing.WarehouseName = wm.WarehouseName;
                    existing.WarehouseCode = wm.WarehouseCode;
                    existing.ContactNumber = wm.ContactNumber;
                    existing.Email = wm.Email;
                    existing.Address = wm.Address;
                    existing.City = wm.City;
                    existing.State = wm.State;
                    existing.Pincode = wm.Pincode;
                    existing.Description = wm.Description;
                    existing.NoOfShop = wm.NoOfShop > 5 ? 5 : wm.NoOfShop;
                    existing.Password = wm.Password;
                    existing.UpdatedDate = DateTime.Now;

                    // Sync Identity user password if changed
                    var identityUser = await _userManager.FindByNameAsync(existing.WarehouseCode);

                    if (identityUser != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                        var passwordResult = await _userManager.ResetPasswordAsync(identityUser, token, wm.Password);

                        if (!passwordResult.Succeeded)
                        {
                            await transaction.RollbackAsync();

                            return new AllResponseMessage
                            {
                                Result = false,
                                Message = string.Join(", ",
                                    passwordResult.Errors.Select(x => x.Description))
                            };
                        }

                        // Sync name & contact on Identity user as well
                        identityUser.FullName = wm.WarehouseName;
                        identityUser.PhoneNumber = wm.ContactNumber;
                        identityUser.Email = wm.Email;
                        identityUser.Password = wm.Password;

                        await _userManager.UpdateAsync(identityUser);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new AllResponseMessage
                    {
                        Result = true,
                        Message = "Warehouse updated successfully."
                    };
                }

                // ── INSERT ─────────────────────────────────────────────────────────
                var existingIdentityUser =
                    await _userManager.FindByNameAsync(wm.WarehouseCode);

                if (existingIdentityUser != null)
                {
                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Warehouse Code already exists."
                    };
                }

                var user = new ApplicationUser
                {
                    UserName = wm.WarehouseCode,
                    FullName = wm.WarehouseName,
                    PhoneNumber = wm.ContactNumber,
                    Email = wm.Email,
                    Password = wm.Password
                };

                var createUserResult = await _userManager.CreateAsync(user, wm.Password);

                if (!createUserResult.Succeeded)
                {
                    await transaction.RollbackAsync();

                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = string.Join(", ",
                            createUserResult.Errors.Select(x => x.Description))
                    };
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Warehouse Manager");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    await transaction.RollbackAsync();

                    return new AllResponseMessage
                    {
                        Result = false,
                        Message = "Role assignment failed."
                    };
                }

                var warehouseEntity = new EWarehouseModel
                {
                    WarehouseName = wm.WarehouseName,
                    WarehouseCode = wm.WarehouseCode,
                    ContactNumber = wm.ContactNumber,
                    Email = wm.Email,
                    Address = wm.Address,
                    City = wm.City,
                    State = wm.State,
                    Pincode = wm.Pincode,
                    Description = wm.Description,
                    NoOfShop = wm.NoOfShop > 5 ? 5 : wm.NoOfShop,
                    CreatedDate = DateTime.Now,
                    Password = wm.Password
                };

                await _context.tblwarehouse.AddAsync(warehouseEntity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new AllResponseMessage
                {
                    Result = true,
                    Message = "Warehouse created successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new AllResponseMessage
                {
                    Result = false,
                    Message = ex.Message
                };
            }
        }


        //public async Task<AllResponseMessage> SaveWarehouse(WarehouseModel wm)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var existingIdentityUser =
        //            await _userManager.FindByNameAsync(wm.WarehouseCode);

        //        if (existingIdentityUser != null)
        //        {
        //            return new AllResponseMessage
        //            {
        //                Result = false,
        //                Message = "Warehouse Code already exists."
        //            };
        //        }

        //        var user = new ApplicationUser
        //        {
        //            UserName = wm.WarehouseCode,
        //            FullName = wm.WarehouseName,
        //            PhoneNumber = wm.ContactNumber,
        //            Email = wm.Email,
        //            Password = wm.Password
        //        };

        //        var createUserResult = await _userManager.CreateAsync(user, wm.Password);

        //        if (!createUserResult.Succeeded)
        //        {
        //            await transaction.RollbackAsync();

        //            return new AllResponseMessage
        //            {
        //                Result = false,
        //                Message = string.Join(", ",
        //                    createUserResult.Errors.Select(x => x.Description))
        //            };
        //        }

        //        var roleResult =
        //            await _userManager.AddToRoleAsync(user, "Warehouse Manager");

        //        if (!roleResult.Succeeded)
        //        {
        //            await _userManager.DeleteAsync(user);
        //            await transaction.RollbackAsync();

        //            return new AllResponseMessage
        //            {
        //                Result = false,
        //                Message = "Role assignment failed."
        //            };
        //        }

        //        var warehouseEntity = new EWarehouseModel
        //        {
        //            WarehouseName = wm.WarehouseName,
        //            WarehouseCode = wm.WarehouseCode,
        //            ContactNumber = wm.ContactNumber,
        //            Email = wm.Email,
        //            Address = wm.Address,
        //            City = wm.City,
        //            Pincode = wm.Pincode,
        //            Description = wm.Description,
        //            NoOfShop = wm.NoOfShop > 5 ? 5 : wm.NoOfShop,
        //            CreatedDate = DateTime.Now,
        //            Password = wm.Password
        //        };

        //        await _context.tblwarehouse.AddAsync(warehouseEntity);
        //        await _context.SaveChangesAsync();

        //        await transaction.CommitAsync();

        //        return new AllResponseMessage
        //        {
        //            Result = true,
        //            Message = "Warehouse created successfully."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();

        //        return new AllResponseMessage
        //        {
        //            Result = false,
        //            Message = ex.Message
        //        };
        //    }
        //}


    }
}
