namespace POSModels.Models
{
    public enum UserRole
    {
        Admin = 1,
        Manager = 2,
        Cashier = 3,
        WarehouseStaff = 4,
        DefaultRole = 99
    }

    public static class UserRoleExtensions
    {
        public static string ToRoleName(this UserRole role)
        {
            switch (role)
            {
                case UserRole.Admin:
                    return "Admin";
                case UserRole.Manager:
                    return "Manager";
                case UserRole.Cashier:
                    return "Cashier";
                case UserRole.WarehouseStaff:
                    return "WarehouseStaff";
                default:
                    return "DefaultRole";
            }
        }
    }
}
