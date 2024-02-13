using Application;
using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Presentation.Controllers.User
{
    public class UserController : BaseApiController
    {

        private readonly IUserOperation _userOperation;

        public UserController(Infrastructure.User.Context userContext, IAutorization autorization, IConfiguration configuration) => _userOperation = new UserOperation(userContext, autorization, configuration);

        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] Domain.User user)
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var newUser = await _userOperation.Create(user);
                return new OkObjectResult(newUser);
            });
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromQuery] string userAccount, [FromQuery] string password)
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var tokenAutorized = await _userOperation.Login(userAccount, password);
                return new OkObjectResult(tokenAutorized);
            });
        }

        [HttpPost("TokenLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> TokenLogin([FromQuery] string userAccount, [FromQuery] string token, [FromQuery] string refreshToken)
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var tokenAutorized = await _userOperation.TokenLogin(userAccount, token, refreshToken);
                return new OkObjectResult(tokenAutorized);
            });
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var users = await _userOperation.GetAllUsers();
                return new OkObjectResult(users);
            });
        }

        [HttpGet("{account}")]
        public async Task<IActionResult> GetUserByAccount(string account)
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var user = await _userOperation.GetByAccount(account);
                return new OkObjectResult(user);
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Domain.User user)
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var updatedUser = await _userOperation.Update(user);
                return new OkObjectResult(updatedUser);
            });
        }

        [HttpDelete("{account}")]
        public async Task<IActionResult> DeleteByAccount(string account)
        {
            return await _userOperation.ExecuteAndHandleErrorsAsync(async () =>
            {
                var deletedUser = await _userOperation.DeleteByAccount(account);
                return new OkObjectResult(deletedUser);
            });
        }
    }
}
