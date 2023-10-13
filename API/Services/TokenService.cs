using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager) {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTKey"]));
            _userManager = userManager;
        }
        public async Task<string> CreateToken(AppUser appUser)
        {
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.UniqueName, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.NameId, appUser.Id.ToString())
            };
            var roles = await _userManager.GetRolesAsync(appUser);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            var sign = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var desc = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = sign,
                Expires = DateTime.Now.AddDays(7)

            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(desc);

            return tokenHandler.WriteToken(token);
        }
    }
}