using Microsoft.AspNetCore.Http;
using ProgrammingPOE.Models;
using System.Text.Json;

namespace ProgrammingPOE.Services
{
    public class AuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DataService _dataService;

        public AuthService(IHttpContextAccessor httpContextAccessor, DataService dataService)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataService = dataService;
        }

        public bool Login(string email, string password)
        {
            var user = _dataService.AuthenticateUser(email, password);
            if (user != null)
            {
                var userJson = JsonSerializer.Serialize(user);
                _httpContextAccessor.HttpContext.Session.SetString("CurrentUser", userJson);
                return true;
            }
            return false;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("CurrentUser");
        }

        public ApplicationUser GetCurrentUser()
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("CurrentUser");
            if (!string.IsNullOrEmpty(userJson))
            {
                return JsonSerializer.Deserialize<ApplicationUser>(userJson);
            }
            return null;
        }

        public bool IsAuthenticated() => GetCurrentUser() != null;

        public bool IsInRole(string role)
        {
            var user = GetCurrentUser();
            return user?.Role == role;
        }
    }
}