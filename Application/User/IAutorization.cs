using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User
{
    public interface IAutorization
    {
        string GenerateToken(string userAccount, Guid idToken, DateTime creation, DateTime expiration);
        string GenerateRefreshToken();
        Task<AutorizationResponse> SaveRefreshTokenHistory(Guid idToken, Guid idUser, string token, string refreshToken, DateTime creation, DateTime expiration);
    }
}
