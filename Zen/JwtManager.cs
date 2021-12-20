namespace Zen
{
    public interface IJwtManager
    {
        public string GetToken();
    }

    public class JwtManager : IJwtManager
    {
        static JwtHeader? _jwtHeader;

        public JwtManager(IConfiguration configuration)
        {
            if (_jwtHeader == null)
            {
                var jwtKey = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256);
                _jwtHeader = new JwtHeader(signingCredentials);
            }
        }

        public string GetToken()
        {
            var payload = new JwtPayload
            {
                { "sub", 1 },
                { "roles", new List<string>{ "Super" } },
                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
                { "exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds()},
            };
            var securityToken = new JwtSecurityToken(_jwtHeader, payload);
            var token = (new JwtSecurityTokenHandler()).WriteToken(securityToken);

            return token;
        }
    }
}
