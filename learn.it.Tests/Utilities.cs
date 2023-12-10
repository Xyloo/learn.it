using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using Microsoft.AspNetCore.Mvc.Testing;

namespace learn.it.Tests
{
    public static class Utilities
    {
        private class LoginResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }
        }

        public static async Task<string> LoginUser(User user, HttpClient client)
        {
            var login = new LoginUserDto
            {
                Username = user.Username,
                Password = user.Password
            };
            var loginResponse = await client.PostAsJsonAsync("/api/users/login", login);
            loginResponse.EnsureSuccessStatusCode();
            var token = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            return token.Token;
        }
    }
}
