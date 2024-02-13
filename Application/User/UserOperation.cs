
using System.IdentityModel.Tokens.Jwt;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace Application.User
{
    public class UserOperation : IUserOperation
    {
        private readonly Infrastructure.User.Context _context;
        private readonly IConfiguration _configuration;
        private readonly IAutorization _autorization;
        public UserOperation(Infrastructure.User.Context context, IAutorization autorization, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _autorization = autorization;
        }
        public async Task<AutorizationResponse> Login(string account, string password)
        {
            var userFound = await GetByAccount(account);
            if (userFound == null) return new AutorizationResponse() { Success = false, Message = "User not found.", Token = string.Empty, RefreshToken = string.Empty };
            if (userFound.Password != password) return new AutorizationResponse() { Success = false, Message = "Password incorrect", Token = string.Empty, RefreshToken = string.Empty };
            Guid idToken = Guid.NewGuid();
            DateTime creation = DateTime.UtcNow; // must be UTC for security reasons
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpirationMinutes"])); // must be UTC for security reasons
            DateTime expirationRefreshToken = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationMinutes"])); // must be UTC for security reasons
            return await _autorization.SaveRefreshTokenHistory(idToken, userFound.Id, _autorization.GenerateToken(userFound.Account, idToken, creation, expiration), _autorization.GenerateRefreshToken(), creation, expirationRefreshToken);
        }
        public async Task<AutorizationResponse> TokenLogin(string account, string token, string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenExpired = tokenHandler.ReadJwtToken(token);
            if (tokenExpired.ValidTo > DateTime.UtcNow) return new AutorizationResponse() { Success = false, Message = "Token is not expired.", Token = string.Empty, RefreshToken = string.Empty };

            string? subjectClaim = tokenExpired.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrWhiteSpace(subjectClaim)) return new AutorizationResponse() { Success = false, Message = "Subject empty.", Token = string.Empty, RefreshToken = string.Empty };
            if (account != subjectClaim) return new AutorizationResponse() { Success = false, Message = "Invalid subject.", Token = string.Empty, RefreshToken = string.Empty };

            string? jwtIdClaimString = tokenExpired.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrWhiteSpace(jwtIdClaimString)) return new AutorizationResponse() { Success = false, Message = "JWT id empty.", Token = string.Empty, RefreshToken = string.Empty };

            string? jwtIssuer = tokenExpired.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Iss)?.Value;
            if (string.IsNullOrWhiteSpace(jwtIdClaimString)) return new AutorizationResponse() { Success = false, Message = "No issuer.", Token = string.Empty, RefreshToken = string.Empty };
            if (jwtIssuer != _configuration["Jwt:Issuer"]) return new AutorizationResponse() { Success = false, Message = "Invalid token issuer.", Token = string.Empty, RefreshToken = string.Empty };

            string? jwtAud = tokenExpired.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value;
            if (string.IsNullOrWhiteSpace(jwtIdClaimString)) return new AutorizationResponse() { Success = false, Message = "No Audience.", Token = string.Empty, RefreshToken = string.Empty };
            if (jwtAud != _configuration["Jwt:Audience"]) return new AutorizationResponse() { Success = false, Message = "Invalid token audience.", Token = string.Empty, RefreshToken = string.Empty };

            Guid? jwtId = Guid.Parse(jwtIdClaimString);
            RefreshTokenHistory? tokenHistory = _context.RefreshTokenHistories.FirstOrDefault(x => x.IdTokenHistory == jwtId);
            if (tokenHistory == null) return new AutorizationResponse() { Success = false, Message = "Token not found.", Token = string.Empty, RefreshToken = string.Empty };

            if (tokenHistory.Token != token) return new AutorizationResponse() { Success = false, Message = "Invalid token.", Token = string.Empty, RefreshToken = string.Empty };
            if (tokenHistory.RefreshToken != refreshToken) return new AutorizationResponse() { Success = false, Message = "Invalid refresh token.", Token = string.Empty, RefreshToken = string.Empty };
            if (!tokenHistory.Active) return new AutorizationResponse() { Success = false, Message = "Refresh token expired.", Token = string.Empty, RefreshToken = string.Empty };

            Guid idToken = Guid.NewGuid();
            DateTime creation = DateTime.UtcNow; // must be UTC for security reasons
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpirationMinutes"])); // must be UTC for security reasons
            DateTime expirationRefreshToken = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationMinutes"])); // must be UTC for security reasons
            return await _autorization.SaveRefreshTokenHistory(idToken, tokenHistory.IdUser, _autorization.GenerateToken(account, idToken, creation, expiration), _autorization.GenerateRefreshToken(), creation, expirationRefreshToken);
        }
        public async Task<Domain.User> Create(Domain.User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Id = Guid.NewGuid();
            var newUser = _context.Users.Add(user).Entity;
            await _context.SaveChangesAsync();
            return newUser;
        }
        public async Task<Domain.User[]?> GetAllUsers()
        {
            var users = await _context.Users.ToArrayAsync();
            return users;
        }
        public async Task<Domain.User?> GetByAccount(string account)
        {
            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Account == account);
            return user;
        }
        public async Task<Domain.User> Update(Domain.User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Account == user.Account);
            if (existingUser == null) throw new InvalidOperationException($"User with username {user.Account} not found.");
            existingUser.Name = user.Name;
            existingUser.Account = user.Account;
            existingUser.Password = user.Password;
            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task<Domain.User?> DeleteByAccount(string account)
        {
            if (string.IsNullOrWhiteSpace(account)) throw new ArgumentNullException(nameof(account));
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Account == account);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return user;
        }
    }
}
