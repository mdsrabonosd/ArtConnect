namespace ArtConnect.Framework
{
    public class UrlRewrite
    {
        // Must be 'const' so the compiler can evaluate it inside attributes
        public class Account
        {
            public const string Login = "login";
            public const string Logout = "logout";
            public const string Register = "register";
            public const string Profile = "profile";
            public const string ChangePassword = "change-password";
            public const string AccessDenied = "access-denied";
        }

        public class Admin
        {
            public const string Dashboard = "admin/dashboard";
        }
    }
}
