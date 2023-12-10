using System.Net;
using System.Net.Http.Json;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace learn.it.Tests.IntegrationTests
{
    public class AnswersAndProgressControllerIntegrationTests
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
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task AddAnswer_ValidInput_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var answerResponse = await response.Content.ReadFromJsonAsync<AnswerDto>();
            Assert.That(answerResponse.AnswerTime, Is.EqualTo(answer.AnswerTime));
            Assert.That(answerResponse.IsCorrect, Is.EqualTo(answer.IsCorrect));
            Assert.That(answerResponse.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task AddAnswer_ValidInput_GroupVisibility_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var answerResponse = await response.Content.ReadFromJsonAsync<AnswerDto>();
            Assert.That(answerResponse.AnswerTime, Is.EqualTo(answer.AnswerTime));
            Assert.That(answerResponse.IsCorrect, Is.EqualTo(answer.IsCorrect));
            Assert.That(answerResponse.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task AddAnswer_InvalidFlashcardId_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = _flashcards.Count+1,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task AddAnswer_NoAccessToSet_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[2];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task AddAnswer_InvalidAnswerTime_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = -1,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        [Category("CreateProgress")]
        public async Task AddAnswer_ValidInput_CreatesProgress()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(1));
            });
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateProgress")]
        public async Task AddAnswer_ValidInput_UpdatesProgress()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(1));
            });

            // Act
            answer.IsCorrect = false;
            response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(0));
            });
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateProgress")]
        [Category("GrantAchievement")]
        public async Task AddAnswer_ValidInput_UpdatesProgressToMastered()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[0];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(1));
            });

            // Act
            for (var i = 0; i < 4; i++)
            {
                answer.IsCorrect = true;
                response = await client.PostAsJsonAsync("/api/answers", answer);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(true));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(5));
            });

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievements = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievements, Has.Count.EqualTo(3));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Flashcard Mastered")), Is.EqualTo(1));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateProgress")]
        [Category("GrantAchievement")]
        public async Task AddAnswer_ValidInput_UpdatesFlashcardAndSetToMastered()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(1));
            });

            // Act
            for (var i = 0; i < 4; i++)
            {
                answer.IsCorrect = true;
                response = await client.PostAsJsonAsync("/api/answers", answer);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(true));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(5));
            });

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievements = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievements, Has.Count.EqualTo(4));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Flashcard Mastered")), Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Set Mastered")), Is.EqualTo(1));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateProgress")]
        [Category("UpdateFlashcardProgress")]
        [Category("GrantAchievement")]
        public async Task AddAnswer_NeedsSevenConsecutiveCorrectAnswers_ToBeMastered_IfNeedsMoreRepetitions()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(1));
            });

            response = await client.PutAsJsonAsync($"/api/flashcard_progress/{flashcard.FlashcardId}", new UpdateFlashcardUserProgressDto(){NeedsMoreRepetitions = true});
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Act
            for (var i = 0; i < 4; i++)
            {
                answer.IsCorrect = true;
                response = await client.PostAsJsonAsync("/api/answers", answer);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(5));
            });

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievements = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievements, Has.Count.EqualTo(2));

            for (var i = 0; i < 2; i++)
            {
                answer.IsCorrect = true;
                response = await client.PostAsJsonAsync("/api/answers", answer);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(true));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(7));
            });

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            achievements = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievements, Has.Count.EqualTo(4));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Flashcard Mastered")), Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Set Mastered")), Is.EqualTo(1));
        }

        [Test]
        [Category("AddAnswer")]
        [Category("User")]
        [Category("ValidInput")]
        [Category("UpdateProgress")]
        [Category("UpdateFlashcardProgress")]
        [Category("GrantAchievement")]
        public async Task AddAnswer_AfterChangingNeedsMoreRepetitionsFromTrueToFalse_AndAlreadyHasFiveOrMoreConsecutiveCorrectAnswers_ShouldCountAsMastered()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);
            var answer = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answer);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(1));
            });

            response = await client.PutAsJsonAsync($"/api/flashcard_progress/{flashcard.FlashcardId}", new UpdateFlashcardUserProgressDto() { NeedsMoreRepetitions = true });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Act
            for (var i = 0; i < 4; i++)
            {
                answer.IsCorrect = true;
                response = await client.PostAsJsonAsync("/api/answers", answer);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            }

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(false));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(5));
            });

            response = await client.PutAsJsonAsync($"/api/flashcard_progress/{flashcard.FlashcardId}", new UpdateFlashcardUserProgressDto() { NeedsMoreRepetitions = false });
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Assert
            response = await client.GetAsync($"/api/flashcard_progress/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            progress = await response.Content.ReadFromJsonAsync<FlashcardUserProgressDto>();
            Assert.That(progress, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(progress.Flashcard.FlashcardId, Is.EqualTo(answer.FlashcardId));
                Assert.That(progress.User.Username, Is.EqualTo(user.Username));
                Assert.That(progress.IsMastered, Is.EqualTo(true));
                Assert.That(progress.ConsecutiveCorrectAnswers, Is.EqualTo(5));
            });

            response = await client.GetAsync($"/api/users/{user.UserId}/achievements");
            response.EnsureSuccessStatusCode();
            var achievements = await response.Content.ReadFromJsonAsync<List<UserAchievementsDto>>();
            Assert.That(achievements, Has.Count.EqualTo(4));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Flashcard Mastered")), Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Achievement.Name.Contains("Set Mastered")), Is.EqualTo(1));
        }

        [Test]
        [Category("GetAnswerById")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetAnswerById_ShouldReturnAnswer()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);
            var answerDto = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answerDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var answer = await response.Content.ReadFromJsonAsync<AnswerDto>();

            // Act
            response = await client.GetAsync($"/api/answers/{answer.AnswerId}");
            response.EnsureSuccessStatusCode();
            var answerDtoResponse = await response.Content.ReadFromJsonAsync<AnswerDto>();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(answerDtoResponse.AnswerId, Is.EqualTo(answer.AnswerId));
                Assert.That(answerDtoResponse.Flashcard.FlashcardId, Is.EqualTo(answer.Flashcard.FlashcardId));
                Assert.That(answerDtoResponse.User.Username, Is.EqualTo(answer.User.Username));
                Assert.That(answerDtoResponse.AnswerTime, Is.EqualTo(answer.AnswerTime));
                Assert.That(answerDtoResponse.IsCorrect, Is.EqualTo(answer.IsCorrect));
            });
        }

        [Test]
        [Category("GetAnswerById")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetAnswerById_WithNonExistingAnswerId_ShouldReturnNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/answers/-1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetAnswerById")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetAnswerById_WithNonAdmin_AndNotSelf_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);
            var answerDto = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answerDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var answer = await response.Content.ReadFromJsonAsync<AnswerDto>();

            // Act
            client.DefaultRequestHeaders.Remove("Authorization");
            var token2 = await Utilities.LoginUser(_users[2], client);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token2}");
            response = await client.GetAsync($"/api/answers/{answer.AnswerId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("GetAnswerByFlashcardId")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetAnswerByFlashcardId_ShouldReturnAnswer()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);
            var answerDto = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answerDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var answer = await response.Content.ReadFromJsonAsync<AnswerDto>();

            // Act
            response = await client.GetAsync($"/api/answers/flashcard/{flashcard.FlashcardId}");
            response.EnsureSuccessStatusCode();
            var answerDtoResponse = await response.Content.ReadFromJsonAsync<List<AnswerDto>>();

            // Assert
            Assert.That(answerDtoResponse, Has.Count.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(answerDtoResponse[0].AnswerId, Is.EqualTo(answer.AnswerId));
                Assert.That(answerDtoResponse[0].Flashcard.FlashcardId, Is.EqualTo(answer.Flashcard.FlashcardId));
                Assert.That(answerDtoResponse[0].User.Username, Is.EqualTo(answer.User.Username));
                Assert.That(answerDtoResponse[0].AnswerTime, Is.EqualTo(answer.AnswerTime));
                Assert.That(answerDtoResponse[0].IsCorrect, Is.EqualTo(answer.IsCorrect));
            });
        }

        [Test]
        [Category("GetAnswerByFlashcardId")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetAnswerByFlashcardId_WithNonExistingFlashcardId_ShouldReturnNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/answers/flashcard/-1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetAnswerByFlashcardId")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetAnswerByFlashcardId_WithNoAnswers_ShouldReturnEmptySet()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var flashcard = _flashcards[3];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/answers/flashcard/{flashcard.FlashcardId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var answerDtoResponse = await response.Content.ReadFromJsonAsync<List<AnswerDto>>();
            Assert.That(answerDtoResponse, Is.Empty);
        }

        [Test]
        [Category("GetStudySetProgress")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetStudySetProgress_ShouldReturnProgress()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var studySet = _studySets[0];
            var token = await Utilities.LoginUser(user, client);
            var flashcard = _flashcards[0];
            var answerDto = new CreateAnswerDto()
            {
                FlashcardId = flashcard.FlashcardId,
                AnswerTime = 1000,
                IsCorrect = true
            };

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/answers", answerDto);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            var answer = await response.Content.ReadFromJsonAsync<AnswerDto>();

            // Act
            response = await client.GetAsync($"/api/flashcard_progress/set/{studySet.StudySetId}");
            response.EnsureSuccessStatusCode();
            var studySetProgressDto = await response.Content.ReadFromJsonAsync<List<FlashcardUserProgressDto>>();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(studySetProgressDto, Has.Count.EqualTo(1));
                Assert.That(studySetProgressDto[0].Flashcard.FlashcardId, Is.EqualTo(flashcard.FlashcardId));
                Assert.That(studySetProgressDto[0].Flashcard.FlashcardId, Is.EqualTo(answer.Flashcard.FlashcardId));
                Assert.That(studySetProgressDto[0].User.Username, Is.EqualTo(answer.User.Username));
            });
        }

    }
}
