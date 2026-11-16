namespace ArtConnect.Framework
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string Artist = "Artist";
        public const string Customer = "Customer";

        public static readonly string[] All = { Admin, Artist, Customer };
    }
}
