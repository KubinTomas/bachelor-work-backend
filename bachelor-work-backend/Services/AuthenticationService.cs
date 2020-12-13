using bachelor_work_backend.Models;
using bachelor_work_backend.Models.Authentication;
using bachelor_work_backend.Services.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services
{
    public class AuthenticationService
    {
        public IConfiguration Configuration { get; private set; }
        public StagApiService StagApiService { get; private set; }

        public AuthenticationService(IConfiguration configuration, StagApiService stagApiService)
        {
            Configuration = configuration;
            StagApiService = stagApiService;
        }

        public AuthenticationResult Authorize()
        {
            if (IsLoginValid())
            {
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(CustomClaims.StagToken, "true"),
                    new Claim(CustomClaims.UserId, "0"),
                };

                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:44380",
                    audience: "http://localhost:4200",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: signingCredentials
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

                return new AuthenticationResult(tokenString);
            }
            else
            {
                return new AuthenticationResult();
            }
        }

        public bool IsLoginValid()
        {
            return true;
        }

        public async Task<User> GetStagUserAsync(string wscookie)
        {
            return await StagApiService.StagUserApiService.GetStagUserAsync(wscookie);
        }

        public User GetDbUser(int userId)
        {
            throw new NotImplementedException("DbUser side is not implemented yet");
        }
    }
}
