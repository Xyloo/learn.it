using learn.it.Repos.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using learn.it.Models;
using learn.it.Services;
using learn.it.Services.Interfaces;
using Moq;
using learn.it.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using learn.it.Exceptions;
using learn.it.Exceptions.NotFound;
using learn.it.Exceptions.Conflict;

namespace learn.it.Tests.UnitTests
{
    [TestFixture]
    public class AchievementsServiceUnitTests
    {
        private IAchievementsService _achievementsService;
        private Mock<IAchievementsRepository> _achievementsRepository;
        private Mock<IWebHostEnvironment> _webHostEnvironment;
        private Mock<IImageHandler> _imageHandler;

        [SetUp]
        public void SetUp()
        {
            var fiveTotalLogins = new Achievement
            {
                AchievementId = 1,
                Name = "5 Total Logins",
                Description = "Logged in for 5 days!",
                Predicate = "TotalLoginDays >= 5",
                ImagePath = "test.jpg"
            };
            var fiveConsecutiveLogins = new Achievement
            {
                AchievementId = 2,
                Name = "5 Consecutive Logins",
                Description = "Logged in for 5 days in a row!",
                Predicate = "ConsecutiveLoginDays >= 5",
                ImagePath = "test.jpg"
            };
            var tenTotalLogins = new Achievement
            {
                AchievementId = 3,
                Name = "10 Total Logins",
                Description = "Logged in for 10 days!",
                Predicate = "TotalLoginDays >= 10",
                ImagePath = "test.jpg"
            };


            _achievementsRepository = new Mock<IAchievementsRepository>();
            _achievementsRepository.Setup(x => x.GetAchievement(1)).ReturnsAsync(fiveTotalLogins);
            _achievementsRepository.Setup(x => x.GetAchievement(2)).ReturnsAsync(fiveConsecutiveLogins);
            _achievementsRepository.Setup(x => x.GetAchievement(3)).ReturnsAsync(tenTotalLogins);
            _achievementsRepository.Setup(x => x.GetAchievements()).ReturnsAsync(new List<Achievement> { fiveTotalLogins, fiveConsecutiveLogins, tenTotalLogins });
            _achievementsRepository.Setup(x => x.GetAchievementsContainingInPredicate("TotalLoginDays")).ReturnsAsync(new List<Achievement> { fiveTotalLogins, tenTotalLogins });
            _achievementsRepository.Setup(x => x.GetAchievementsContainingInPredicate("ConsecutiveLoginDays")).ReturnsAsync(new List<Achievement> { fiveConsecutiveLogins });
            _achievementsRepository.Setup(x => x.GetAchievementsContainingInPredicate("LoginDays")).ReturnsAsync(new List<Achievement> { fiveConsecutiveLogins, fiveTotalLogins, tenTotalLogins });
            _achievementsRepository.Setup(x => x.AddAchievement(It.IsAny<Achievement>())).ReturnsAsync((Achievement a) => a);
            _achievementsRepository.Setup(x => x.UpdateAchievement(It.IsAny<Achievement>())).ReturnsAsync((Achievement a) => a);
            _achievementsRepository.Setup(x => x.GetUserAchievementsByUserId(1)).ReturnsAsync(new List<UserAchievements> { new()  { AchievementId = 1, UserId = 1, Achievement = fiveTotalLogins} });
            _achievementsRepository.Setup(x => x.GetUserAchievementsByUserId(2)).ReturnsAsync(new List<UserAchievements>());
            _achievementsRepository.Setup(x => x.GrantAchievementToUser(It.IsAny<UserAchievements>())).ReturnsAsync((UserAchievements ua) => ua);

            _webHostEnvironment = new Mock<IWebHostEnvironment>();
            _webHostEnvironment.Setup(x => x.WebRootPath).Returns("C:\\Users\\Test\\source\\repos\\learn.it\\learn.it\\wwwroot");

            _imageHandler = new Mock<IImageHandler>();
            _imageHandler.Setup(x => x.AddImage(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("test.jpg");

            _achievementsService = new AchievementsService(_achievementsRepository.Object, _imageHandler.Object, _webHostEnvironment.Object);
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateOnlyField_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "5 Total Logins",
                Description = "Logged in for 5 days!",
                Predicate = "TotalLoginDays",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateField_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "UserId >= 5",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateField2_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "FlashcardId >= 5",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateOperator_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "TotalLoginDays != 5",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateNoSpaces_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "TotalLoginDays>=5",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateValueNotANumber_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "TotalLoginDays >= five",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void AddAchievement_InvalidPredicateValueNotAnInteger_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "TotalLoginDays >= 5.5",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.AddAchievement(achievement, file));
        }

        [Test]
        [Category("AddAchievement")]
        [Category("CheckPredicateValid")]
        [Category("ValidInput")]
        public async Task AddAchievement_ValidInput_ShouldReturnAchievement()
        {
            var achievement = new Achievement
            {
                Name = "5 Total Logins",
                Description = "Logged in for 5 days!",
                Predicate = "TotalLoginDays >= 5",
                ImagePath = "test.jpg"
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            var result = await _achievementsService.AddAchievement(achievement, file);

            Assert.That(result, Is.EqualTo(achievement));
        }

        [Test]
        [Category("GetAchievement")]
        [Category("InvalidInput")]
        public void GetAchievement_InvalidId_ShouldThrow()
        {
            Assert.ThrowsAsync<AchievementNotFoundException>(async () => await _achievementsService.GetAchievement(-1));
        }

        [Test]
        [Category("GetAchievement")]
        [Category("ValidInput")]
        public async Task GetAchievement_ValidId_ShouldReturnAchievement()
        {
            var result = await _achievementsService.GetAchievement(1);

            Assert.That(result, Is.TypeOf<Achievement>());
        }

        [Test]
        [Category("GetAchievements")]
        [Category("ValidInput")]
        public async Task GetAchievements_ValidInput_ShouldReturnAchievements()
        {
            var result = (await _achievementsService.GetAchievements()).ToList();

            Assert.That(result, Is.TypeOf<List<Achievement>>());
            Assert.That(result, Has.Count.EqualTo(3));
        }

        [Test]
        [Category("UpdateAchievement")]
        [Category("CheckPredicateValid")]
        [Category("InvalidInput")]
        public void UpdateAchievement_InvalidPredicateValueNotAnInteger_ShouldThrow()
        {
            var achievement = new Achievement
            {
                Name = "dummy",
                Description = "dummy!",
                Predicate = "TotalLoginDays >= 5.5",
                ImagePath = "test.jpg"
            };

            Assert.ThrowsAsync<InvalidInputDataException>(async () => await _achievementsService.UpdateAchievement(achievement));
        }

        [Test]
        [Category("UpdateAchievement")]
        [Category("CheckPredicateValid")]
        [Category("ValidInput")]
        public async Task UpdateAchievement_ValidInput_ShouldReturnAchievement()
        {
            var achievement = new Achievement
            {
                Name = "5 Total Logins",
                Description = "Logged in for 5 days!",
                Predicate = "TotalLoginDays >= 5",
                ImagePath = "test.jpg"
            };

            var result = await _achievementsService.UpdateAchievement(achievement);

            Assert.That(result, Is.EqualTo(achievement));
        }

        [Test]
        [Category("UpdateAchievementImage")]
        [Category("ValidInput")]
        public async Task UpdateAchievementImage_ValidInput_ShouldReturnAchievement()
        {
            const string oldPath = "old.jpg";
            var achievement = new Achievement
            {
                Name = "5 Total Logins",
                Description = "Logged in for 5 days!",
                Predicate = "TotalLoginDays >= 5",
                ImagePath = oldPath
            };

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            var result = await _achievementsService.UpdateAchievementImage(achievement, file);

            Assert.That(result.ImagePath, Is.EqualTo("test.jpg"));
        }

        [Test]
        [Category("GrantAchievement")]
        [Category("InvalidInput")]
        public void GrantAchievement_UserAchievementAlreadyExists_ShouldThrow()
        {
            Assert.ThrowsAsync<UserAchievementExistsException>(async () => await _achievementsService.GrantAchievement(1, 1));
        }

        [Test]
        [Category("GrantAchievement")]
        [Category("ValidInput")]
        public async Task GrantAchievement_ValidInput_ShouldReturnUserAchievement()
        {
            var result = await _achievementsService.GrantAchievement(1, 2);

            Assert.That(result, Is.TypeOf<UserAchievements>());
            Assert.Multiple(() =>
            {
                Assert.That(result.AchievementId, Is.EqualTo(2));
                Assert.That(result.UserId, Is.EqualTo(1));
            });
        }

        [Test]
        [Category("GrantAchievementsContainingPredicate")]
        [Category("ValidInput")]
        public async Task GrantAchievementsContainingPredicate_ValidInput_ShouldReturnUserAchievements()
        {
            var user = new User
            {
                UserId = 1,
                Username = "test",
                Password = "test",
                Email = "",
                UserStats = new UserStats
                {
                    TotalLoginDays = 6
                }
            };
            var result = (await _achievementsService.GrantAchievementsContainingPredicate("TotalLoginDays", user)).ToList();

            Assert.That(result, Is.TypeOf<List<Achievement>>());
            Assert.That(result, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("GrantAchievementsContainingPredicate")]
        [Category("ValidInput")]
        public async Task GrantAchievementsContainingPredicate_ValidInput2_ShouldReturnUserAchievements()
        {
            var user = new User
            {
                UserId = 1,
                Username = "test",
                Password = "test",
                Email = "",
                UserStats = new UserStats
                {
                    TotalLoginDays = 11
                }
            };

            var tenTotalLogins = new Achievement
            {
                AchievementId = 3,
                Name = "10 Total Logins",
                Description = "Logged in for 10 days!",
                Predicate = "TotalLoginDays >= 10",
                ImagePath = "test.jpg"
            };
            var result = (await _achievementsService.GrantAchievementsContainingPredicate("TotalLoginDays", user)).ToList();

            Assert.That(result, Is.TypeOf<List<Achievement>>());
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo(tenTotalLogins.Name));
        }

        [Test]
        [Category("GrantAchievementsContainingPredicate")]
        [Category("ValidInput")]
        public async Task GrantAchievementsContainingPredicate_ValidInput3_ShouldReturnUserAchievements()
        {
            var user = new User
            {
                UserId = 2,
                Username = "test",
                Password = "test",
                Email = "",
                UserStats = new UserStats
                {
                    TotalLoginDays = 6
                }
            };

            var result = (await _achievementsService.GrantAchievementsContainingPredicate("TotalLoginDays", user)).ToList();

            Assert.That(result, Is.TypeOf<List<Achievement>>());
            Assert.That(result, Has.Count.EqualTo(1));
        }

        [Test]
        [Category("GrantAchievementsContainingPredicate")]
        [Category("ValidInput")]
        public async Task GrantAchievementsContainingPredicate_ValidInput4_ShouldReturnUserAchievements()
        {
            var user = new User
            {
                UserId = 2,
                Username = "test",
                Password = "test",
                Email = "",
                UserStats = new UserStats
                {
                    TotalLoginDays = 12,
                    ConsecutiveLoginDays = 7
                }
            };

            var result = (await _achievementsService.GrantAchievementsContainingPredicate("LoginDays", user)).ToList();

            Assert.That(result, Is.TypeOf<List<Achievement>>());
            Assert.That(result, Has.Count.EqualTo(3));
        }
    }
}
