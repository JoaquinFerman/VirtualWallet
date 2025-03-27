using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PrimeraWebAPI.Services {
    public class EnviromentVariables {
        private readonly IConfiguration _configuration;

        public EnviromentVariables(IConfiguration configuration) {
            _configuration = configuration;
        }

        public SymmetricSecurityKey GetJwtKey() {
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key)) {
                throw new Exception("JWT key is not configured.");
            }
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }
}