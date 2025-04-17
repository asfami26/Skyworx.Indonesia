
namespace Skyworx.Common.Config
{
    public class ConfigExtension
    {
        public JwtConfig Jwt { get; set; }
        public ConnectionStringsConfig ConnectionStrings { get; set; }
    }

    public class JwtConfig
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }

    public class ConnectionStringsConfig
    {
        public string Postgres { get; set; }
    }
}

