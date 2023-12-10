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
    public class StudySetControllerIntegratonTests
    {
        private WebApplicationFactory<Program> _factory;
        private List<User> _users;
        private List<Achievement> _achievements;
        private List<Group> _groups;
        private List<StudySet> _studySets;
        private List<Flashcard> _flashcards;
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
            _studySets = data.StudySets;
            _flashcards = data.Flashcards;
        }

        [Test]
        [Category("GetPublicStudySets")]
        [Category("Anonymous")]
        public async Task GetPublicStudySets_ReturnsStudySets()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/studysets/");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<List<BasicStudySetDto>>();
            Assert.That(responseString, Has.Count.EqualTo(1));
        }

        [Test]
        [Category("GetPublicStudySets")]
        [Category("User")]
        public async Task GetPublicStudySets_AsUser_ReturnsStudySets()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/studysets/");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<List<BasicStudySetDto>>();
            Assert.That(responseString, Has.Count.EqualTo(1));
        }

        [Test]
        [Category("GetAllStudySets")]
        [Category("User")]
        public async Task GetAllStudySets_AsUser_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/studysets/all");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("GetAllStudySets")]
        [Category("Admin")]
        public async Task GetAllStudySets_AsAdmin_ReturnsSets()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/studysets/all");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<List<BasicStudySetDto>>();
            Assert.That(responseString, Has.Count.EqualTo(3));
        }

        [Test]
        [Category("GetStudySetDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetStudySetDetails_AsUser_ReturnsStudySet()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = _studySets[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/{studySet.StudySetId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.That(responseString.StudySetId, Is.EqualTo(studySet.StudySetId));
        }

        [Test]
        [Category("GetStudySetDetails")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetStudySetDetails_InvalidSetId_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/{_studySets.Count+1}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetStudySetDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetStudySetDetails_AsUser_ReturnsStudySetWithFlashcards()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = _studySets[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/{studySet.StudySetId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.That(responseString.Flashcards, Has.Count.EqualTo(2));
        }

        [Test]
        [Category("GetStudySetDetails")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetStudySetDetails_AsUserWithNoAccess_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = _studySets[1];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/{studySet.StudySetId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetStudySetDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetStudySetDetails_AsUserWithAccess_WithGroupVisibility_ReturnsStudySet()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var studySet = _studySets[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/{studySet.StudySetId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.That(responseString.StudySetId, Is.EqualTo(studySet.StudySetId));
        }

        [Test]
        [Category("GetStudySetsContainingName")]
        [Category("Anonymous")]
        [Category("ValidInput")]
        public async Task GetStudySetsContainingName_AsAnonymous_ReturnsStudySets()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/api/studysets/find/testSet");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<List<BasicStudySetDto>>();
            Assert.That(responseString, Has.Count.EqualTo(1));
        }

        [Test]
        [Category("GetStudySetsContainingName")]
        [Category("Anonymous")]
        [Category("InvalidInput")]
        public async Task GetStudySetsContainingName_AsAnonymous_NoSetsFound_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"/api/studysets/find/invalid");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetStudySetsContainingName")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetStudySetsContainingName_AsUser_ReturnsStudySets_ConsideringVisibility()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/find/testSet");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<List<BasicStudySetDto>>();
            Assert.That(responseString, Has.Count.EqualTo(2));
        }

        [Test]
        [Category("GetStudySetsContainingName")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetStudySetsContainingName_AsUser_TryingToFindSetWithNoAccess_ShouldReturnNotFound_IfNoMatches()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/find/testSet2");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetStudySetsContainingName")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task GetStudySetsContainingName_AsAdmin_ReturnsStudySets_RegardlessOfVisibility()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/studysets/find/testSet");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<List<BasicStudySetDto>>();
            Assert.That(responseString, Has.Count.EqualTo(3));
        }

        [Test]
        [Category("CreateStudySet")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateStudySet_AsUser_ReturnsCreatedStudySet()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new CreateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = null,
                Visibility = Visibility.Private
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/studysets", studySet);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.That(responseString.Name, Is.EqualTo(studySet.Name));
        }

        [Test]
        [Category("CreateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateStudySet_AsUser_WhenNotPartOfGroup_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new CreateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = _groups[1].GroupId,
                Visibility = Visibility.Group
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/studysets", studySet);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateStudySet_AsUser_WhenGroupNotProvided_ButVisibilityIsGroup_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new CreateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = null,
                Visibility = Visibility.Group
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/studysets", studySet);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateStudySet_AsUser_WhenGroupProvided_ButVisibilityNotGroup_ReturnsCreatedWithGroupIgnored()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new CreateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = _groups[0].GroupId,
                Visibility = Visibility.Public
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/studysets", studySet);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.That(responseString.Group, Is.Null);
            Assert.That(responseString.Visibility, Is.EqualTo(Visibility.Public));
        }

        [Test]
        [Category("CreateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateStudySet_AsUser_WhenGroupProvided_ButVisibilityNotGroup_ReturnsCreatedWithGroupIgnored2()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new CreateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = _groups[0].GroupId,
                Visibility = Visibility.Private
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/studysets", studySet);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.That(responseString.Group, Is.Null);
            Assert.That(responseString.Visibility, Is.EqualTo(Visibility.Private));
        }

        [Test]
        [Category("UpdateStudySet")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task UpdateStudySet_AsUser_ReturnsUpdatedStudySet()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new UpdateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = null,
                Visibility = Visibility.Private
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/studysets/{_studySets[0].StudySetId}", studySet);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.Multiple(() =>
            {
                Assert.That(responseString.Name, Is.EqualTo(studySet.Name));
                Assert.That(responseString.Description, Is.EqualTo(studySet.Description));
                Assert.That(responseString.Visibility, Is.EqualTo(studySet.Visibility));
                Assert.That(responseString.Group, Is.Null);
            });
        }

        [Test]
        [Category("UpdateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateStudySet_AsUser_WhenNotOwner_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new UpdateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = null,
                Visibility = Visibility.Private
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/studysets/{_studySets[0].StudySetId}", studySet);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("UpdateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]

        public async Task
            UpdateStudySet_AsUser_WhenGroupProvided_ButVisibilityNotGroup_ReturnsUpdatedStudySetWithGroupIgnored()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new UpdateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                GroupId = _groups[0].GroupId,
                Visibility = Visibility.Public
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/studysets/{_studySets[0].StudySetId}", studySet);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadFromJsonAsync<StudySetDto>();
            Assert.Multiple(() =>
            {
                Assert.That(responseString.Name, Is.EqualTo(studySet.Name));
                Assert.That(responseString.Description, Is.EqualTo(studySet.Description));
                Assert.That(responseString.Visibility, Is.EqualTo(Visibility.Public));
                Assert.That(responseString.Group, Is.Null);
            });
        }

        [Test]
        [Category("UpdateStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateStudySet_AsUser_WhenGroupNotProvided_ButVisibilityGroup_ReturnBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);
            var studySet = new UpdateStudySetDto
            {
                Name = "testSet3",
                Description = "testSet3",
                Visibility = Visibility.Group
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/studysets/{_studySets[0].StudySetId}", studySet);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("DeleteStudySet")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task DeleteStudySet_AsUser_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/studysets/{_studySets[0].StudySetId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        [Category("DeleteStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task DeleteStudySet_AsUser_WhenNotOwner_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/studysets/{_studySets[0].StudySetId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("DeleteStudySet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task DeleteStudySet_AsUser_WhenStudySetDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/studysets/{_studySets.Count+1}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("DeleteStudySet")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task DeleteStudySet_AsAdmin_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/studysets/{_studySets[0].StudySetId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
