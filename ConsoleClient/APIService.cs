using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ConsoleClient
{
    internal class APIService
    {
        public static async Task<TOutput?> MakeRequest<TInput, TOutput>(string apiUrl, string method, string? apiKey, TInput? payload)
        {
            TOutput? response = default(TOutput);

            string? json = (payload != null) ? JsonSerializer.Serialize(payload) : null;
            HttpResponseMessage httpResponse = await SendRequest(apiUrl, method, apiKey, json);
            if (httpResponse.IsSuccessStatusCode)
            {
                string responseBody = await httpResponse.Content.ReadAsStringAsync();
                JsonSerializerOptions options = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
                Model.ApiResponse? apiResponse = JsonSerializer.Deserialize<Model.ApiResponse>(responseBody, options);
                response = (!string.IsNullOrWhiteSpace(apiResponse?.Value?.ToString())) ? JsonSerializer.Deserialize<TOutput>(apiResponse.Value.ToString() ?? "", options) : default(TOutput);
            }
            else
            {
                Console.WriteLine($"Error code: {httpResponse.StatusCode}");
            }
            return response;
        }
        private static async Task<HttpResponseMessage> SendRequest(string apiUrl, string method, string? apiKey, string? payload)
        {
            using (HttpClient client = new HttpClient())
            {
                if (apiKey != null) client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpContent? content = (!string.IsNullOrEmpty(payload)) ? new StringContent(payload, Encoding.UTF8, "application/json") : null;

                switch (method.ToUpper())
                {
                    case "GET":
                        return await client.GetAsync(apiUrl);
                    case "POST":
                        return await client.PostAsync(apiUrl, content);
                    case "PUT":
                        return await client.PutAsync(apiUrl, content);
                    case "PATCH":
                        return await client.PatchAsync(apiUrl, content);
                    case "DELETE":
                        return await client.DeleteAsync(apiUrl);
                    default:
                        throw new ArgumentException($"HTTP method unknown: {method}");
                }

            }
        }
        public static bool ValidateToken(string Token)
        {
            string issuer = "JWTLogingToken";
            string audience = "LogingUsers";
            string secretKey = "this is my custom Secret key for authentication";
            try
            {
                TokenValidationParameters validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                };

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(Token, validationParameters, out validatedToken);

                Console.WriteLine("Valid token");
                Console.WriteLine($"Emisor (Issuer): {principal?.Claims?.FirstOrDefault(c => c.Type == "iss")?.Value}");
                Console.WriteLine($"Audiencia (Audience): {principal?.Claims?.FirstOrDefault(c => c.Type == "aud")?.Value}");
                return true;
            }
            catch (SecurityException ex)
            {
                Console.WriteLine("Validate token security exception: " + ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Validate token excepcion: " + ex.Message);
                return false;
            }
        }
    }
}
