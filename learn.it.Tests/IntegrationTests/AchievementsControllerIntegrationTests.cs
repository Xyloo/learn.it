using learn.it.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using learn.it.Models.Dtos.Response;
using Microsoft.Extensions.DependencyInjection;

namespace learn.it.Tests.IntegrationTests
{
    [TestFixture]
    public class AchievementsControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private List<Permission> _permissions;
        private List<User> _users;
        private List<Achievement> _achievements;
        private List<Group> _groups;
        private LearnitDbContext _db;

        public AchievementsControllerIntegrationTests()
        {
        }

        [SetUp]
        public void Init()
        {
            _factory = new TestWebApplicationFactory<Program>();
            _db = _factory.Services.CreateScope().ServiceProvider.GetRequiredService<LearnitDbContext>();
            (_permissions, _users, _achievements, _groups) = DatabaseSeeder.Prepare(_db);
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
    }
}
