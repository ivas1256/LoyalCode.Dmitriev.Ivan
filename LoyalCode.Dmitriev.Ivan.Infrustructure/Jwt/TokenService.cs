using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LoyalCode.Dmitriev.Ivan.Infrustructure.Jwt
{
    public interface ITokenService
    {
        Task AddToBlacklist(string tokenId);
        string GenerateToken(Domain.User user);
        int? GetUserIdFromToken(string token);
        Task<bool> IsBlacklisted(string tokenId);
    }

    internal class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public TokenService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public string GenerateToken(Domain.User user)
        {
            //TODO Config to class
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }

        public Task AddToBlacklist(string tokenId)
        {
            var key = tokenId;
            _memoryCache.Set(key, true);
            return Task.CompletedTask;
        }

        public Task<bool> IsBlacklisted(string tokenId)
        {
            var key = tokenId;
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
