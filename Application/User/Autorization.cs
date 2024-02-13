using Domain;
using Infrastructure.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Application.User
{
    public class Autorization : IAutorization
    {
        private readonly IConfiguration _configuration;
        private readonly Context _context;

        public Autorization(IConfiguration configuration, Context context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(string userAccount, Guid idToken, DateTime creation, DateTime expiration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userAccount),
            new Claim(JwtRegisteredClaimNames.Jti, idToken.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                notBefore: creation, //validFrom
                expires: expiration, //validTo
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        // This code is replaced because generate invalid chars for query string, but if the refresh token is sended in the body you can use this method
        public string GenerateRefreshTokenAnyChar()
        {
            byte[] byteArray = new byte[64];
            string response = "";
            using (var randomNumber = RandomNumberGenerator.Create())
            {
                randomNumber.GetBytes(byteArray);
                response = Convert.ToBase64String(byteArray);
            }
            return response;
        }
        public string GenerateRefreshToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            byte[] byteArray = new byte[64];

            using (var randomNumber = RandomNumberGenerator.Create())
            {
                randomNumber.GetBytes(byteArray);
            }

            char[] result = new char[byteArray.Length];

            for (int i = 0; i < byteArray.Length; i++)
            {
                result[i] = chars[byteArray[i] % chars.Length];
            }

            return new string(result);
        }

        public async Task<AutorizationResponse> SaveRefreshTokenHistory(Guid idToken, Guid idUser, string token, string refreshToken, DateTime creation, DateTime expirationRefreshToken)
        {
            RefreshTokenHistory refreshTokenHistory = new RefreshTokenHistory()
            {
                IdTokenHistory = idToken,
                IdUser = idUser,
                Token = token,
                RefreshToken = refreshToken,
                CreatedDate = creation,
                ExpirationDate = expirationRefreshToken
            };
            await _context.RefreshTokenHistories.AddAsync(refreshTokenHistory);
            await _context.SaveChangesAsync();
            return new AutorizationResponse() { Token = token, RefreshToken = refreshToken, Success = true, Message = "OK" };
        }
    }
}
