using learn.it.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using Microsoft.Extensions.DependencyInjection;

namespace learn.it.Tests.IntegrationTests
{
    [TestFixture]
    public class AchievementsControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
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
        [Category("GetAllAchievements")]
        [Category("User")]
        public async Task GetAllAchievements_User_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync("/api/achievements");
            var data = await response.Content.ReadFromJsonAsync<List<AchievementDto>>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(data, Has.Count.EqualTo(_achievements.Count));
            });
        }

        [Test]
        [Category("GetAllAchievements")]
        [Category("Anonymous")]
        public async Task GetAllAchievements_Anonymous_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/achievements");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("GetAchievementDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetAchievementDetails_ValidData_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync($"/api/achievements/{_achievements[0].AchievementId}");
            var str = await response.Content.ReadAsStringAsync();
            var data = await response.Content.ReadFromJsonAsync<AchievementDto>();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(data, Is.Not.Null);
                Assert.That(data.AchievementId, Is.EqualTo(_achievements[0].AchievementId));
            });
        }

        [Test]
        [Category("GetAchievementDetails")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetAchievementDetails_InvalidData_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync($"/api/achievements/{_achievements.Count + 1}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task AddAchievement_ValidData_ReturnsCreated()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new CreateAchievementDto()
            {
                Name = "New Achievement",
                Description = "New Achievement Description",
                Predicate = "TotalLoginDays >= 3"
            };
            await using var stream = File.OpenRead("Salesforce-logo.png");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"},
                {new StringContent(achievement.Name), "name"},
                {new StringContent(achievement.Description), "description"},
                {new StringContent(achievement.Predicate), "predicate"}
            };
            var response = await client.PostAsync("/api/achievements", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task AddAchievement_InvalidImage_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new CreateAchievementDto()
            {
                Name = "New Achievement",
                Description = "New Achievement Description",
                Predicate = "TotalLoginDays >= 3"
            };
            await using var stream = File.OpenRead("listaLUT.docx");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"},
                {new StringContent(achievement.Name), "name"},
                {new StringContent(achievement.Description), "description"},
                {new StringContent(achievement.Predicate), "predicate"}
            };
            var response = await client.PostAsync("/api/achievements", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task AddAchievement_InvalidPredicate_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new CreateAchievementDto()
            {
                Name = "New Achievement",
                Description = "New Achievement Description",
                Predicate = "asdf>=3"
            };
            await using var stream = File.OpenRead("Salesforce-logo.png");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"},
                {new StringContent(achievement.Name), "name"},
                {new StringContent(achievement.Description), "description"},
                {new StringContent(achievement.Predicate), "predicate"}
            };
            var response = await client.PostAsync("/api/achievements", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task AddAchievement_NoPermissions_ReturnsForbidden()
        {
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new CreateAchievementDto()
            {
                Name = "New Achievement",
                Description = "New Achievement Description",
                Predicate = "TotalLoginDays >= 3"
            };
            await using var stream = File.OpenRead("Salesforce-logo.png");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"},
                {new StringContent(achievement.Name), "name"},
                {new StringContent(achievement.Description), "description"},
                {new StringContent(achievement.Predicate), "predicate"}
            };
            var response = await client.PostAsync("/api/achievements", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("RemoveAchievement")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task RemoveAchievement_ValidData_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.DeleteAsync($"/api/achievements/{_achievements[0].AchievementId}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("RemoveAchievement")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task RemoveAchievement_InvalidData_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.DeleteAsync($"/api/achievements/{_achievements.Count + 1}");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("UpdateAchievement")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task UpdateAchievement_ValidData_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new UpdateAchievementDto()
            {
                Name = "Updated Achievement",
                Description = "Updated Achievement Description",
                Predicate = "TotalLoginDays >= 5"
            };
            var response = await client.PutAsJsonAsync($"/api/achievements/{_achievements[0].AchievementId}", achievement);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("UpdateAchievement")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task UpdateAchievement_InvalidData_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new UpdateAchievementDto()
            {
                Name = "Updated Achievement",
                Description = "Updated Achievement Description",
                Predicate = "TotalLoginDays >= 5"
            };
            var response = await client.PutAsJsonAsync($"/api/achievements/{_achievements.Count + 1}", achievement);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("UpdateAchievement")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task UpdateAchievement_InvalidPredicate_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var achievement = new UpdateAchievementDto()
            {
                Name = "Updated Achievement",
                Description = "Updated Achievement Description",
                Predicate = "asdf>=3"
            };
            var response = await client.PutAsJsonAsync($"/api/achievements/{_achievements[0].AchievementId}", achievement);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("UpdateAchievementImage")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task UpdateAchievementImage_ValidData_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            await using var stream = File.OpenRead("Salesforce-logo.png");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"}
            };
            var response = await client.PutAsync($"/api/achievements/{_achievements[0].AchievementId}/image", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("UpdateAchievementImage")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task UpdateAchievementImage_InvalidData_ReturnsNotFound()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            await using var stream = File.OpenRead("Salesforce-logo.png");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"}
            };
            var response = await client.PutAsync($"/api/achievements/{_achievements.Count + 1}/image", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("UpdateAchievementImage")]
        [Category("Admin")]
        [Category("InvalidInput")]
        public async Task UpdateAchievementImage_InvalidImage_ReturnsBadRequest()
        {
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            await using var stream = File.OpenRead("listaLUT.docx");
            using var formContent = new MultipartFormDataContent
            {
                {new StreamContent(stream), "achievementImage", "Salesforce-logo.png"}
            };
            var response = await client.PutAsync($"/api/achievements/{_achievements[0].AchievementId}/image", formContent);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
