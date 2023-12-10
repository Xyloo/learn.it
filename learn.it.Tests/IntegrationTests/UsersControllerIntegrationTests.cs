using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace learn.it.Tests.IntegrationTests
{
    [TestFixture]
    public class UsersControllerIntegrationTests
    {
        private  WebApplicationFactory<Program> _factory;
        private List<User> _users;
        private List<Achievement> _achievements;
        private List<Group> _groups;
        private LearnitDbContext _db;

        [SetUp]
        public void Init()
        {
            _factory = new TestWebApplicationFactory<Program>();
            _db = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<LearnitDbContext>();
            var data = DatabaseSeeder.Prepare(_db);
            _users = data.Users;
            _achievements = data.Achievements;
            _groups = data.Groups;
        }

        [Test]
        [Category("Admin")]
        [Category("GetAllUsers")]
        public async Task GetAllUsers_ReturnsAllUsers_IfAdmin()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[0], client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<SelfUserResponseDto>>();

            Assert.That(users.Count, Is.EqualTo(4));
        }

        [Test]
        [Category("User")]
        [Category("GetAllUsers")]
        public async Task GetAllUsers_ReturnsForbidden_IfNotAdmin()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync("/api/users");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("Anonymous")]
        [Category("GetAllUsers")]
        public async Task GetAllUsers_ReturnsUnauthorized_IfAnonymous()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/users");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("User")]
        [Category("Login")]
        public async Task Login_UpdatedStats_IfValidCredentials()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/users/{_users[1].UserId}");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<SelfUserResponseDto>();

            Assert.That(data.LastLogin, Is.Not.Null);
            Assert.That(data.UserStats.ConsecutiveLoginDays, Is.EqualTo(1));
            Assert.That(data.UserStats.TotalLoginDays, Is.EqualTo(1));
            Assert.That(data.UserId, Is.EqualTo(_users[1].UserId));
        }

        [Test]
        [Category("User")]
        [Category("Login")]
        [Category("GetUserAchievements")]
        public async Task Login_GrantsAchievements_IfValidLogin()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/users/{_users[1].UserId}");
            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadFromJsonAsync<SelfUserResponseDto>();

            Assert.That(data.LastLogin, Is.Not.Null);
            Assert.That(data.UserStats.ConsecutiveLoginDays, Is.EqualTo(1));
            Assert.That(data.UserStats.TotalLoginDays, Is.EqualTo(1));

            response = await client.GetAsync($"/api/users/{_users[1].UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievementsData = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            achievementsData.Sort((a,b) => a.Achievement.AchievementId - b.Achievement.AchievementId);

            Assert.That(achievementsData.Count, Is.EqualTo(2));
            Assert.That(achievementsData[0].Achievement.Name, Is.EqualTo(_achievements[0].Name));
            Assert.That(achievementsData[1].Achievement.Name, Is.EqualTo(_achievements[1].Name));
        }

        [Test]
        [Category("InvalidInput")]
        [Category("Login")]
        public async Task Login_ReturnsBadRequest_IfInvalidCredentials()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/users/login", new LoginUserDto()
            {
                Username = "testUser1",
                Password = "InvalidPassword1"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("InvalidInput")]
        [Category("Login")]
        public async Task Login_ReturnsNotFound_IfUserDoesNotExist()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/users/login", new LoginUserDto()
            {
                Username = "nonexistinguser1",
                Password = "InvalidPassword1"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("InvalidInput")]
        [Category("Login")]
        public async Task Login_ReturnsBadRequest_IfDataDoesntMatchRequirements()
        {
            var client = _factory.CreateClient();
            var response = await client.PostAsJsonAsync("/api/users/login", new LoginUserDto()
            {
                Username = "aaa",
                Password = "asdf"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("User")]
        [Category("Login")]
        public async Task Login_CreatesLogs_IfCredentialsValid()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/users/logins");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<List<Login>>();
            Assert.That(data, Has.Count.Positive);
            Assert.That(data[0].IsSuccessful, Is.EqualTo(true));
        }

        [Test]
        [Category("User")]
        [Category("Login")]
        public async Task Login_CreatesLogs_IfCredentialsInvalid()
        {
            var client = _factory.CreateClient();
            var failedLogin = await client.PostAsJsonAsync("/api/users/login", new LoginUserDto()
            {
                Username = "testUser1",
                Password = "InvalidPassword1"
            });

            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/users/logins");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<List<Login>>();
            Assert.That(data, Has.Count.Positive);
            Assert.That(data[0].IsSuccessful, Is.EqualTo(false));
        }

        [Test]
        [Category("User")]
        [Category("Logout")]
        public async Task AfterValidLogout_JwtIsBlacklisted()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/users/logout");
            response.EnsureSuccessStatusCode();

            response = await client.GetAsync("/api/users/logins");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("User")]
        [Category("InvalidInput")]
        [Category("UpdateUser")]
        public async Task UpdateUser_ReturnsBadRequest_IfInvalidData()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/users/{_users[1].UserId}", new UpdateUserDto()
            {
                Username = "testUser1",
                Password = "invalid"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("User")]
        [Category("InvalidInput")]
        [Category("UpdateUser")]
        public async Task UpdateUser_ReturnsForbidden_IfNotSelf()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/users/{_users[2].UserId}", new UpdateUserDto()
            {
                Username = "testUser500",
                Password = "testUser500"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("Admin")]
        [Category("ValidInput")]
        [Category("UpdateUser")]
        public async Task UpdateUser_ReturnsOK_IfAdmin()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[0], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/users/{_users[2].UserId}", new UpdateUserDto()
            {
                Username = "testUser500",
                Password = "testUser500"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/users/{_users[2].UserId}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<SelfUserResponseDto>();
            Assert.That(data.Username, Is.EqualTo("testUser500"));
        }

        [Test]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateUser")]
        public async Task UpdateUser_ReturnsOK_IfSelf()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/users/{_users[1].UserId}", new UpdateUserDto()
            {
                Username = "testUser500",
                Password = "testUser500"
            });

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/users/{_users[1].UserId}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<SelfUserResponseDto>();
            Assert.That(data.Username, Is.EqualTo("testUser500"));
        }

        [Test]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateUserAvatar")]
        public async Task UpdateUserAvatar_ReturnsOK_IfFileIsImage()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            await using var stream = File.OpenRead("Salesforce-logo.png");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "avatar", "Salesforce-logo.png"}
            };
            var response = await client.PostAsync($"/api/users/avatar/{_users[1].UserId}", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/users/{_users[1].UserId}");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<SelfUserResponseDto>();
            Assert.That(data.Avatar, Is.Not.Null);
        }

        [Test]
        [Category("User")]
        [Category("InvalidInput")]
        [Category("UpdateUserAvatar")]
        public async Task UpdateUserAvatar_ReturnsBadRequest_IfFileIsNotAnImage()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            await using var stream = File.OpenRead("listaLUT.docx");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "avatar", "listaLUT.png"}
            };
            var response = await client.PostAsync($"/api/users/avatar/{_users[1].UserId}", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("User")]
        [Category("ValidInput")]
        [Category("GetUserGroups")]
        public async Task GetUserGroups_ReturnsOK_IfUserExists()
        {
            var client = _factory.CreateClient();
            var token = await Utilities.LoginUser(_users[1], client);

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/users/{_users[1].UserId}/groups");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var data = await response.Content.ReadFromJsonAsync<List<GroupDto>>();
            Assert.That(data, Has.Count.EqualTo(1));
            Assert.That(data[0].GroupId, Is.EqualTo(_groups[0].GroupId));
        }
    }
}
