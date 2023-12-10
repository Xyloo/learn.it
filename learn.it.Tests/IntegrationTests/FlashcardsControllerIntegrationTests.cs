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
    public class FlashcardsControllerIntegrationTests
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
        [Category("GetFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetFlashcard_ValidInput_ReturnsFlashcard()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/{flashcard.FlashcardId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.FlashcardId, Is.EqualTo(flashcard.FlashcardId));
        }

        [Test]
        [Category("GetFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetFlashcard_InvalidInput_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/{_flashcards.Count + 1}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetFlashcard_NoAccess_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var flashcard = _flashcards[2];
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/{flashcard.FlashcardId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetFlashcard")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task GetFlashcard_Admin_ReturnsFlashcard()
        {
            // Arrange
            var client = _factory.CreateClient();
            var flashcard = _flashcards[2];
            var user = _users[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/{flashcard.FlashcardId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.FlashcardId, Is.EqualTo(flashcard.FlashcardId));
        }

        [Test]
        [Category("GetFlashcardsInSet")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetFlashcardsInSet_ValidInput_ReturnsFlashcards()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcards = _flashcards.Where(f => f.StudySet.StudySetId == studySet.StudySetId).ToList();
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/fromSet/{studySet.StudySetId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardsResponse = await response.Content.ReadFromJsonAsync<List<FlashcardDto>>();
            Assert.That(flashcardsResponse.Count, Is.EqualTo(flashcards.Count));
        }

        [Test]
        [Category("GetFlashcardsInSet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetFlashcardsInSet_InvalidInput_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/fromSet/{_studySets.Count + 1}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetFlashcardsInSet")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetFlashcardsInSet_NoAccess_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var studySet = _studySets[1];
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/fromSet/{studySet.StudySetId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetFlashcardsInSet")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task GetFlashcardsInSet_Admin_ReturnsFlashcards()
        {
            // Arrange
            var client = _factory.CreateClient();
            var studySet = _studySets[1];
            var user = _users[0];
            var flashcards = _flashcards.Where(f => f.StudySet.StudySetId == studySet.StudySetId).ToList();
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/flashcards/fromSet/{studySet.StudySetId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardsResponse = await response.Content.ReadFromJsonAsync<List<FlashcardDto>>();
            Assert.That(flashcardsResponse.Count, Is.EqualTo(flashcards.Count));
        }

        [Test]
        [Category("CreateTextFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateTextFlashcard_ValidInput_ReturnsFlashcard()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateTextFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Term = "Test Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/flashcards/", flashcard);

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.Term, Is.EqualTo(flashcard.Term));
            Assert.That(flashcardResponse.Definition, Is.EqualTo(flashcard.Definition));
        }

        [Test]
        [Category("CreateTextFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateTextFlashcard_InvalidSetInDto_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = new CreateTextFlashcardDto()
            {
                StudySetId = _studySets.Count + 1,
                Term = "Test Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/flashcards/", flashcard);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("CreateTextFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateTextFlashcard_NoAccessToSetInDto_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = new CreateTextFlashcardDto()
            {
                StudySetId = _studySets[1].StudySetId,
                Term = "Test Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/flashcards/", flashcard);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("CreateTextFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateTextFlashcard_NoAccess_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[1];
            var flashcard = new CreateTextFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Term = "Test Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/flashcards/", flashcard);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("CreateTextFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateTextFlashcard_ValidInput_GrantsAchievement()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateTextFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Term = "Test Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/flashcards/", flashcard);

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.FlashcardId, Is.Not.EqualTo(0));

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievementsResponse = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievementsResponse.Count, Is.EqualTo(3)); //need to take logins into account
            Assert.That(achievementsResponse.Count(a => a.Achievement.Name.Contains("Flashcard")), Is.EqualTo(1));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateImageFlashcard_ValidInput_ReturnsFlashcard()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            var file = File.OpenRead("Salesforce-logo.png");
            multipartContent.Add(new StreamContent(file), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            multipartContent.Add(new StringContent(flashcard.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            response.EnsureSuccessStatusCode();
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.Term.EndsWith(".webp"));
            Assert.That(flashcardResponse.Definition, Is.EqualTo(flashcard.Definition));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateImageFlashcard_InvalidSetInDto_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = _studySets.Count + 1,
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            var file = File.OpenRead("Salesforce-logo.png");
            multipartContent.Add(new StreamContent(file), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            multipartContent.Add(new StringContent(flashcard.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateImageFlashcard_NoAccessToSetInDto_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = _studySets[1].StudySetId,
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            var file = File.OpenRead("Salesforce-logo.png");
            multipartContent.Add(new StreamContent(file), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            multipartContent.Add(new StringContent(flashcard.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateImageFlashcard_NoImageInDto_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            multipartContent.Add(new StringContent(flashcard.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateImageFlashcard_NoDefinitionInDto_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = studySet.StudySetId
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            var file = File.OpenRead("Salesforce-logo.png");
            multipartContent.Add(new StreamContent(file), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateImageFlashcard_InvalidImageInDto_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            var file = File.OpenRead("listaLUT.docx");
            multipartContent.Add(new StreamContent(file), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            multipartContent.Add(new StringContent(flashcard.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateImageFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateImageFlashcard_ValidInput_GrantsAchievement()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var flashcard = new CreateImageFlashcardDto()
            {
                StudySetId = studySet.StudySetId,
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var multipartContent = new MultipartFormDataContent();
            var file = File.OpenRead("Salesforce-logo.png");
            multipartContent.Add(new StreamContent(file), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcard.StudySetId.ToString()), "StudySetId");
            multipartContent.Add(new StringContent(flashcard.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsync("/api/flashcards/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.FlashcardId, Is.Not.EqualTo(0));

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievementsResponse = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievementsResponse.Count, Is.EqualTo(3)); //need to take logins into account
            Assert.That(achievementsResponse.Count(a => a.Achievement.Name.Contains("Flashcard")), Is.EqualTo(1));
        }

        [Test]
        [Category("UpdateToTextFlashcard")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task UpdateToTextFlashcard_ValidInput_ReturnsOk_WithChangingSet_AsAdmin()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[0];
            var flashcard = _flashcards[0];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "Test Term",
                Definition = "Test Definition",
                StudySetId = _studySets[1].StudySetId
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/flashcards/{flashcard.FlashcardId}", flashcardDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.Definition, Is.EqualTo(flashcardDto.Definition));
        }

        [Test]
        [Category("UpdateToTextFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task UpdateToTextFlashcard_ValidInput_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "New Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/flashcards/{flashcard.FlashcardId}", flashcardDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.Definition, Is.EqualTo(flashcardDto.Definition));
        }

        [Test]
        [Category("UpdateToTextFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateToTextFlashcard_InvalidInput_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "New Term",
                Definition = "Test Definition",
                StudySetId = _studySets[1].StudySetId
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/flashcards/{flashcard.FlashcardId}", flashcardDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("UpdateToTextFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task UpdateToTextFlashcard_ValidInput_ReturnsOK()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "New Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/flashcards/{flashcard.FlashcardId}", flashcardDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.Definition, Is.EqualTo(flashcardDto.Definition));
            Assert.That(flashcardResponse.Term, Is.EqualTo(flashcardDto.Term));
        }

        [Test]
        [Category("UpdateToTextFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateToTextFlashcard_NoTermIfPreviouslyImage_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsJsonAsync($"/api/flashcards/{flashcard.FlashcardId}", flashcardDto);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("UpdateToImageFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task UpdateToImageFlashcard_ValidInput_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "New Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var image = File.OpenRead("Salesforce-logo.png");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(image), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcardDto.Term), "Term");
            multipartContent.Add(new StringContent(flashcardDto.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsync($"/api/flashcards/{flashcard.FlashcardId}/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var flashcardResponse = await response.Content.ReadFromJsonAsync<FlashcardDto>();
            Assert.That(flashcardResponse.Definition, Is.EqualTo(flashcardDto.Definition));
            Assert.That(flashcardResponse.Term.EndsWith(".webp"));
        }

        [Test]
        [Category("UpdateToImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateToImageFlashcard_InvalidInput_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "New Term",
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var image = File.OpenRead("Salesforce-logo.png");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(image), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcardDto.Term), "Term");
            multipartContent.Add(new StringContent(flashcardDto.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsync($"/api/flashcards/{_flashcards.Count+1}/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("UpdateToImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateToImageFlashcard_NoTermIfPreviouslyText_ReturnsOK()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Definition = "Test Definition"
            };
            var token = await Utilities.LoginUser(user, client);

            var image = File.OpenRead("Salesforce-logo.png");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(image), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcardDto.Definition), "Definition");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsync($"/api/flashcards/{flashcard.FlashcardId}/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("UpdateToImageFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateToImageFlashcard_NotAValidImage_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var flashcardDto = new UpdateFlashcardDto()
            {
                Term = "New Term"
            };
            var token = await Utilities.LoginUser(user, client);

            var image = File.OpenRead("listaLUT.docx");
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(image), "image", "Salesforce-logo.png");
            multipartContent.Add(new StringContent(flashcardDto.Term), "Term");
            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PutAsync($"/api/flashcards/{flashcard.FlashcardId}/withImage", multipartContent);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("DeleteFlashcard")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task DeleteFlashcard_ValidInput_ReturnsNoContent()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/flashcards/{flashcard.FlashcardId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        [Category("DeleteFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task DeleteFlashcard_InvalidInput_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/flashcards/{_flashcards.Count+1}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("DeleteFlashcard")]
        [Category("Anonymous")]
        public async Task DeleteFlashcard_UnauthorizedUser_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var flashcard = _flashcards[0];

            // Act
            var response = await client.DeleteAsync($"/api/flashcards/{flashcard.FlashcardId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        [Category("DeleteFlashcard")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task DeleteFlashcard_NotOwnedByUser_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.DeleteAsync($"/api/flashcards/{flashcard.FlashcardId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

    }
}
