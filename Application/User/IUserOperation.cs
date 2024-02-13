using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User {
    public interface IUserOperation {
        Task<Domain.User> Create(Domain.User user);
        Task<AutorizationResponse> Login(string account, string password);
        Task<AutorizationResponse> TokenLogin(string account, string token, string refreshToken);
        Task<Domain.User[]?> GetAllUsers();
        Task<Domain.User?> GetByAccount(string account);
        Task<Domain.User> Update(Domain.User user);
        Task<Domain.User?> DeleteByAccount(string account);
    }
}
