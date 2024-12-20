namespace UserSystem.API.Models.Helpers
{
    public class AppSettings
    {
        public JwtTokenSettings JwtToken { get; set; }
        public ConnectionStringsSettings ConnectionStrings { get; set; }
        public string AllowedHosts { get; set; }
        public string Environment { get; set; }
    }

    public class JwtTokenSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int MinutesToExpiration { get; set; }
    }
    public class ConnectionStringsSettings
    {
        public string UserSystemDBContext { get; set; }
    }
}