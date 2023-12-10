using learn.it.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace learn.it.Tests.IntegrationTests
{
    public class AnswersControllerIntegrationTests
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

    }
}
