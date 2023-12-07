using System.Text;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using learn.it.Services;
using learn.it.Services.Interfaces;
using learn.it.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using learn.it.Exceptions.Conflict;
using learn.it.Exceptions.NotFound;
using learn.it.Models.Dtos.Request;
using Moq;

namespace learn.it.Tests.UnitTests
{
    [TestFixture]
    public class UsersServiceUnitTests
    {
        private IUsersService _usersService;
        private Mock<IUsersRepository> _usersRepository;
        private Mock<IPermissionsRepository> _permissionsRepository;
        private Mock<IWebHostEnvironment> _webHostEnvironment;
        private Mock<IImageHandler> _imageHandler;

        private User CreateMockUser()
        {
            User mockUser = new()
            {
                UserId = 0,
                Username = "",
                Email = "",
                Password = "veryStrongPassword",
                Permissions = new Permission { PermissionId = 2, Name = "User" },
                UserStats = new UserStats(),
                UserPreferences = new UserPreferences()
            };
            return mockUser;
        }

        [SetUp]
        public void Setup()
        {
            var adminPermission = new Permission { PermissionId = 1, Name = "Admin" };
            var userPermission = new Permission { PermissionId = 2, Name = "User" };

            var adminUser = new User
            {
                UserId = 1,
                Username = "testAdmin",
                Email = "admin@admin.com",
                Password = "testAdmin1",
                Permissions = adminPermission,
                UserStats = new UserStats(),
                UserPreferences = new UserPreferences()
            };

            var normalUser = new User
            {
                UserId = 2,
                Username = "testUser",
                Email = "user@user.com",
                Password = "testUser1",
                Permissions = userPermission,
                UserStats = new UserStats(),
                UserPreferences = new UserPreferences()
            };

            _usersRepository = new Mock<IUsersRepository>();
            _usersRepository.Setup(x => x.GetUserById(1)).ReturnsAsync(adminUser);
            _usersRepository.Setup(x => x.GetUserById(2)).ReturnsAsync(normalUser);
            _usersRepository.Setup(x => x.GetUserByEmail("user@user.com")).ReturnsAsync(normalUser);
            _usersRepository.Setup(x => x.GetUserByEmail("admin@admin.com")).ReturnsAsync(adminUser);
            _usersRepository.Setup(x => x.GetUserByUsername("testUser")).ReturnsAsync(normalUser);
            _usersRepository.Setup(x => x.GetUserByUsername("testAdmin")).ReturnsAsync(adminUser);
            _usersRepository.Setup(x => x.GetAllUsers()).ReturnsAsync(new List<User> { adminUser, normalUser });
            _usersRepository.Setup(x => x.CreateUser(It.IsAny<User>())).ReturnsAsync((User user) => user);
            _usersRepository.Setup(x => x.UpdateUser(It.IsAny<User>())).ReturnsAsync((User user) => user);

            
            _permissionsRepository = new Mock<IPermissionsRepository>();
            _permissionsRepository.Setup(x => x.GetPermissionByName("User")).ReturnsAsync(userPermission);
            _permissionsRepository.Setup(x => x.GetPermissionByName("Admin")).ReturnsAsync(adminPermission);
            _permissionsRepository.Setup(x => x.GetPermissionById(1)).ReturnsAsync(adminPermission);
            _permissionsRepository.Setup(x => x.GetPermissionById(2)).ReturnsAsync(userPermission);
            _permissionsRepository.Setup(x => x.GetAllPermissions())
                .ReturnsAsync(new List<Permission> { adminPermission, userPermission });

            _webHostEnvironment = new Mock<IWebHostEnvironment>();
            _webHostEnvironment.Setup(x => x.WebRootPath).Returns("C:\\Users\\Test\\source\\repos\\learn.it\\learn.it\\wwwroot");

            _imageHandler = new Mock<IImageHandler>();
            _imageHandler.Setup(x => x.AddImage(It.IsAny<IFormFile>(), It.IsAny<string>())).ReturnsAsync("test.jpg");

            _usersService = new UsersService(_usersRepository.Object, _permissionsRepository.Object, _webHostEnvironment.Object, _imageHandler.Object);
        }

        [Test]
        [Category("CreateUser")]
        [Category("InvalidInput")]
        public void CreateUser_EmailExists_ShouldThrow()
        {
            var user = CreateMockUser();
            user.Username = "notTakenUsername";
            user.Email = "user@user.com";

            Assert.ThrowsAsync<EmailExistsException>(() => _usersService.CreateUser(user));
        }

        [Test]
        [Category("CreateUser")]
        [Category("InvalidInput")]
        public void CreateUser_UsernameExists_ShouldThrow()
        {
            var user = CreateMockUser();
            user.Username = "testUser";
            user.Email = "notTakenEmail@user.com";

            Assert.ThrowsAsync<UsernameExistsException>(() => _usersService.CreateUser(user));
        }

        [Test]
        [Category("CreateUser")]
        [Category("ValidInput")]
        public async Task CreateUser_ValidInput_ShouldReturnUser()
        {
            var user = CreateMockUser();
            user.Username = "notTakenUsername";
            user.Email = "notTakenEmail@user.com";

            var userResult = await _usersService.CreateUser(user);

            Assert.That(userResult, Is.EqualTo(user));
        }

        [Test]
        [Category("DeleteUser")]
        [Category("InvalidInput")]
        public void DeleteUser_UserDoesNotExist_ShouldThrow()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _usersService.DeleteUser(3));
        }

        [Test]
        [Category("DeleteUser")]
        [Category("ValidInput")]
        public void DeleteUser_UserExists_ShouldNotThrow()
        {
            Assert.That(async () => await _usersService.DeleteUser(1), Throws.Nothing);
        }

        [Test]
        [Category("GetUserByIdOrUsername")]
        [Category("InvalidInput")]
        public void GetUserByIdOrUsername_UsernameDoesNotExist_ShouldThrow()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _usersService.GetUserByIdOrUsername("notExistingUser"));
        }

        [Test]
        [Category("GetUserByIdOrUsername")]
        [Category("InvalidInput")]
        public void GetUserByIdOrUsername_UserIdDoesNotExist_ShouldThrow()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _usersService.GetUserByIdOrUsername((-1).ToString()));
        }

        [Test]
        [Category("GetUserByIdOrUsername")]
        [Category("ValidInput")]
        public async Task GetUserByIdOrUsername_UsernameExists_ShouldReturnUser()
        {
            var user = await _usersService.GetUserByIdOrUsername("testUser");

            Assert.That(user.Username, Is.EqualTo("testUser"));
        }

        [Test]
        [Category("GetUserByIdOrUsername")]
        [Category("ValidInput")]
        public async Task GetUserByIdOrUsername_UserIdExists_ShouldReturnUser()
        {
            var user = await _usersService.GetUserByIdOrUsername("1");

            Assert.That(user.UserId, Is.EqualTo(1));
        }

        [Test]
        [Category("GetAllUsers")]
        [Category("ValidInput")]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            var users = await _usersService.GetAllUsers();

            Assert.That(users.Count, Is.EqualTo(2));
        }

        [Test]
        [Category("UpdateUser")]
        [Category("InvalidInput")]
        public void UpdateUser_UserDoesNotExist_ShouldThrow()
        {
            var user = CreateMockUser();
            user.UserId = 3;

            Assert.ThrowsAsync<UserNotFoundException>(() => _usersService.UpdateUser(user.UserId, new UpdateUserDto()));
        }

        [Test]
        [Category("UpdateUser")]
        [Category("InvalidInput")]
        public void UpdateUser_ChangingUsernameToExistingOne_IfNotSelf_ShouldThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2; // testUser
            
            var newDto = new UpdateUserDto
            {
                Username = "testAdmin"
            };

            Assert.ThrowsAsync<UsernameExistsException>(() => _usersService.UpdateUser(user.UserId, newDto));
        }

        [Test]
        [Category("UpdateUser")]
        [Category("InvalidInput")]
        public void UpdateUser_ChangingEmailToExistingOne_IfNotSelf_ShouldThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2; // testUser

            var newDto = new UpdateUserDto
            {
                Email = "admin@admin.com"
            };

            Assert.ThrowsAsync<EmailExistsException>(() => _usersService.UpdateUser(user.UserId, newDto));
        }

        [Test]
        [Category("UpdateUser")]
        [Category("ValidInput")]
        public async Task UpdateUser_ValidInput_ShouldReturnUpdatedUser()
        {
            var user = CreateMockUser();
            user.UserId = 2; // testUser

            var newDto = new UpdateUserDto
            {
                Username = "newUsername",
                Email = "newEmail@email.com"
            };

            var updatedUser = await _usersService.UpdateUser(user.UserId, newDto);
            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.Username, Is.EqualTo(newDto.Username));
                Assert.That(updatedUser.Email, Is.EqualTo(newDto.Email));
            });
        }

        [Test]
        [Category("UpdateUser")]
        [Category("ValidInput")]
        public async Task UpdateUser_ChangingUsernameToTheSame_ShouldUpdateUserAndNotThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2; // testUser
            user.Username = "newUsername";

            var newDto = new UpdateUserDto
            {
                Username = "newUsername"
            };

            var updatedUser = await _usersService.UpdateUser(user.UserId, newDto);
            Assert.That(updatedUser.Username, Is.EqualTo(newDto.Username));
        }

        [Test]
        [Category("UpdateUser")]
        [Category("ValidInput")]
        public async Task UpdateUser_ChangingEmailToTheSame_ShouldUpdateUserAndNotThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2; // testUser
            user.Email = "newEmail@email.com";

            var newDto = new UpdateUserDto
            {
                Email = user.Email
            };

            var updatedUser = await _usersService.UpdateUser(user.UserId, newDto);

            Assert.That(updatedUser.Email, Is.EqualTo(newDto.Email));
        }

        [Test]
        [Category("UpdateUser")]
        [Category("ValidInput")]
        public async Task UpdateUser_ChangingPassword_ShouldUpdateUserAndNotThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2; // testUser

            var newDto = new UpdateUserDto
            {
                Password = "newPassword"
            };

            var updatedUser = await _usersService.UpdateUser(user.UserId, newDto);

            Assert.That(updatedUser.Password, Is.Not.EqualTo(newDto.Password)); //should be hashed
        }

        [Test]
        [Category("VerifyPassword")]
        [Category("InvalidInput")]
        public async Task VerifyPassword_PasswordDoesNotMatch_ShouldReturnFalse()
        {
            var user = CreateMockUser();
            var password = "superDifficultPassword";
            user.Username = "notExistingUser";
            user.Password = password;
            
            user = await _usersService.CreateUser(user); // to hash the password

            var result = _usersService.VerifyPassword(user, "wrongPassword");
            Assert.That(result, Is.False);
        }

        [Test]
        [Category("VerifyPassword")]
        [Category("InvalidInput")]
        public async Task VerifyPassword_PasswordMatches_ShouldReturnTrue()
        {
            var user = CreateMockUser();
            var password = "superDifficultPassword";
            user.Username = "notExistingUser";
            user.Password = password;

            user = await _usersService.CreateUser(user); // to hash the password

            var result = _usersService.VerifyPassword(user, password);
            Assert.That(result, Is.True);
        }

        [Test]
        [Category("UpdateUserAvatar")]
        [Category("ValidInput")]
        public async Task UpdateUserAvatar_ShouldSetAvatar_AndNotThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2;
            user.Avatar = null;

            IFormFile file = new FormFile(new MemoryStream("This is a dummy file"u8.ToArray()), 0, 0, "Data", "dummy.txt");

            var userWithAvatar = await _usersService.UpdateUserAvatar(user, file);

            Assert.That(userWithAvatar.Avatar, Is.EqualTo("test.jpg"));
        }

        [Test]
        [Category("DeleteUserAvatar")]
        [Category("ValidInput")]
        public async Task DeleteUserAvatar_ShouldSetAvatarToNull_AndNotThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2;
            user.Avatar = "test.jpg";

            await _usersService.DeleteUserAvatar(user);

            Assert.That(user.Avatar, Is.Null);
        }

        [Test]
        [Category("DeleteUserAvatar")]
        [Category("ValidInput")]
        public async Task DeleteUserAvatar_ShouldReturnIfAvatarAlreadyNull_AndNotThrow()
        {
            var user = CreateMockUser();
            user.UserId = 2;
            user.Avatar = null;

            await _usersService.DeleteUserAvatar(user);

            Assert.That(user.Avatar, Is.Null);
        }
    }
}