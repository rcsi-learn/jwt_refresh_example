using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class Option
    {
        internal class Level
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public Action Process { get; set; } = () => { };
        }
        public string API_URL { get; set; }
        public string UserAccount { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public List<Level> Options { get; set; } = new List<Level>();
        public List<Level> UserOperations { get; set; } = new List<Level>();
        public Option()
        {
            API_URL = "http://localhost:5035/api/User";
            UserAccount = string.Empty;
            Token = string.Empty;
            RefreshToken = string.Empty;
            Options = new List<Level> {
                new Level() { Id = 1, Title = "Create User", Process = CreateUser() },
                new Level() { Id = 2, Title = "Login", Process = Login() },
                new Level() { Id = 3, Title = "Token Login", Process = TokenLogin() },
                new Level() { Id = 4, Title = "Get All Users", Process = GetAllUsers() },
                new Level() { Id = 5, Title = "Get user by name", Process = GetUserByAccount() },
                new Level() { Id = 6, Title = "Update user", Process = UpdateUser() },
                new Level() { Id = 7, Title = "Delete user", Process = DeleteUser() },
            };
        }
        public void Run()
        {
            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine($"{Options[i].Id}) {Options[i].Title}");
            }
            var userOption = Console.ReadLine();
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Id.ToString() == userOption) Options[i].Process.Invoke();
                //Console.Clear();
            }
        }
        private Action CreateUser()
        {
            return () =>
            {
                Console.Clear();
                Console.Write("Name: ");
                string? name = Console.ReadLine();
                Console.Write("User: ");
                string? account = Console.ReadLine();
                Console.Write("Password: ");
                string? password = Console.ReadLine();
                var newUser = new { Id = new Guid(), Name = name, Account = account, Password = password };
                var request = APIService.MakeRequest<object, dynamic>($"{API_URL}/Create", "POST", null, newUser).Result;
                JsonElement nameElement = default;
                JsonElement accountElement = default;
                if (request?.TryGetProperty("name", out nameElement)) name = nameElement.GetString();
                if (request?.TryGetProperty("account", out accountElement)) account = accountElement.GetString();
                Console.Clear();
                Console.WriteLine($"Created user for Name: {name}; Account: {account}");
                Console.ReadLine();
            };
        }
        private Action Login()
        {
            return () =>
            {
                Console.Write("User: ");
                string? account = Console.ReadLine();
                UserAccount = account ?? string.Empty;
                Console.Write("Password: ");
                string? password = Console.ReadLine();
                var userLoggin = new { Account = account, Password = password };
                Model.AutorizationResponse? request = APIService.MakeRequest<object, Model.AutorizationResponse>($"{API_URL}/Login?userAccount={userLoggin.Account}&password={userLoggin.Password}", "POST", null, null).Result;
                if (request != null && request.Success)
                {
                    Token = request?.Token ?? string.Empty;
                    RefreshToken = request?.RefreshToken ?? string.Empty;
                    if (APIService.ValidateToken(Token))
                    {
                        Console.WriteLine("token: " + request?.Token);
                        Console.WriteLine("refresh token: " + request?.RefreshToken);
                    }
                }
                else
                {
                    Console.WriteLine("Error: " + request?.Message);
                }
                Console.ReadLine();
                Console.Clear();
            };
        }
        private Action TokenLogin()
        {
            return () =>
            {
                var tokenLogin = new { userAccount = UserAccount, token = Token, refreshToken = RefreshToken };
                Model.AutorizationResponse? request = APIService.MakeRequest<object, Model.AutorizationResponse>($"{API_URL}/TokenLogin?userAccount={tokenLogin.userAccount}&token={tokenLogin.token}&refreshToken={tokenLogin.refreshToken}", "POST", null, null).Result;
                if (request != null && request.Success)
                {
                    Token = request?.Token ?? string.Empty;
                    RefreshToken = request?.RefreshToken ?? string.Empty;
                    if (APIService.ValidateToken(Token))
                    {
                        Console.WriteLine("token: " + request?.Token);
                        Console.WriteLine("refresh token: " + request?.RefreshToken);
                    }
                }
                else
                {
                    Console.WriteLine("Error: " + request?.Message);
                }
                Console.ReadLine();
                Console.Clear();
            };
        }
        private Action GetAllUsers()
        {
            return () =>
            {
                Model.User[]? requestResponse = APIService.MakeRequest<object, Model.User[]>($"{API_URL}/GetAll", "GET", Token, null).Result;
                string? name = string.Empty, userAccount = string.Empty;
                for (int i = 0; i < requestResponse?.Length; i++)
                {
                    Console.WriteLine($"Name: {requestResponse[i].Name}; Account: {requestResponse[i].Account}");
                }
                Console.ReadLine();
                Console.Clear();
            };
        }
        private Action GetUserByAccount()
        {
            return () =>
            {
                Console.Write("account: ");
                string? account = Console.ReadLine();
                dynamic? request = APIService.MakeRequest<object, dynamic>($"{API_URL}/{account}", "GET", Token, null).Result;
                string? name = string.Empty, userAccount = string.Empty;
                JsonElement nameElement = default;
                JsonElement accountElement = default;
                if (request?.TryGetProperty("name", out nameElement)) name = nameElement.GetString();
                if (request?.TryGetProperty("account", out accountElement)) userAccount = accountElement.GetString();
                Console.Clear();
                Console.WriteLine($"Name: {name}; Account: {userAccount}");
                Console.ReadLine();
                Console.Clear();
            };
        }
        private Action UpdateUser()
        {
            return () =>
            {
                Console.Write("name: ");
                string? name = Console.ReadLine();
                Console.Write("account: ");
                string? account = Console.ReadLine();
                Console.Write("password: ");
                string? password = Console.ReadLine();
                var user = new { Name = name, Account = account, Password = password };
                dynamic? request = APIService.MakeRequest<object, dynamic>($"{API_URL}/Update", "PUT", Token, user).Result;
                JsonElement nameElement = default;
                JsonElement accountElement = default;
                if (request?.TryGetProperty("name", out nameElement)) name = nameElement.GetString();
                if (request?.TryGetProperty("account", out accountElement)) account = accountElement.GetString();
                Console.Clear();
                Console.WriteLine($"Name: {name}; Account: {account}");
                Console.ReadLine();
                Console.Clear();
            };
        }
        private Action DeleteUser()
        {
            return () =>
            {
                Console.Write("account: ");
                string? account = Console.ReadLine();
                dynamic? request = APIService.MakeRequest<object, dynamic>($"{API_URL}/{account}", "DELETE", Token, null).Result;
                string? name = string.Empty;
                JsonElement nameElement = default;
                JsonElement accountElement = default;
                if (request?.TryGetProperty("name", out nameElement)) name = nameElement.GetString();
                if (request?.TryGetProperty("account", out accountElement)) account = accountElement.GetString();
                Console.Clear();
                Console.WriteLine($"Deleted Name: {name}; Account: {account}");
                Console.ReadLine();
                Console.Clear();
            };
        }
    }
}