using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using learn.it.Exceptions;
using learn.it.Exceptions.NotFound;
using learn.it.Models;
using learn.it.Models.Dtos.Request;
using learn.it.Repos.Interfaces;
using learn.it.Services;
using learn.it.Services.Interfaces;
using Moq;

namespace learn.it.Tests.UnitTests
{
    [TestFixture]
    public class GroupsServiceUnitTests
    {
        private IGroupsService _groupsService;
        private Mock<IGroupsRepository> _groupsRepositoryMock;
        private Mock<IUsersRepository> _usersRepositoryMock;

        [SetUp]
        public void SetUp()
        {
            var user1 = new User
            {
                UserId = 1,
                Username = "testUser1",
                Password = "testUser1",
                Email = "user1@user.com"
            };

            var user2 = new User
            {
                UserId = 2,
                Username = "testUser2",
                Password = "testUser2",
                Email = "user2@user.com"
            };

            var user3 = new User
            {
                UserId = 3,
                Username = "testUser3",
                Password = "testUser3",
                Email = "user3@user.com"
            };

            var group1 = new Group
            {
                GroupId = 1,
                Name = "testGroup1",
                Creator = user1,
                Users = new List<User> { user1, user2 }
            };
            user1.GroupCreator = new List<Group> {group1};
            user1.Groups = new List<Group> { group1};
            user2.Groups = new List<Group> { group1 };

            _groupsRepositoryMock = new Mock<IGroupsRepository>();
            _groupsRepositoryMock.Setup(g => g.GetGroupById(1)).ReturnsAsync(group1);
            _groupsRepositoryMock.Setup(g => g.CreateGroup(It.IsAny<Group>())).ReturnsAsync((Group g) => g);
            _groupsRepositoryMock.Setup(g => g.UpdateGroup(It.IsAny<Group>())).ReturnsAsync((Group g) => g);
            _groupsRepositoryMock.Setup(g => g.CreateGroupJoinRequest(It.IsAny<GroupJoinRequest>())).ReturnsAsync((GroupJoinRequest gjr) => gjr);

            _usersRepositoryMock = new Mock<IUsersRepository>();
            _usersRepositoryMock.Setup(u => u.GetUserById(1)).ReturnsAsync(user1);
            _usersRepositoryMock.Setup(u => u.GetUserById(2)).ReturnsAsync(user2);
            _usersRepositoryMock.Setup(u => u.GetUserById(3)).ReturnsAsync(user3);

            _groupsService = new GroupsService(_groupsRepositoryMock.Object, _usersRepositoryMock.Object);
        }

        [Test]
        [Category("CreateGroup")]
        [Category("InvalidInput")]
        public void CreateGroup_NoCreator_ThrowsException()
        {
            var group = new Group
            {
                GroupId = 1,
                Name = "testGroup1",
                Creator = new User()
            };

            Assert.ThrowsAsync<UserNotFoundException>(() => _groupsService.CreateGroup(group));
        }

        [Test]
        [Category("CreateGroup")]
        [Category("ValidInput")]
        public async Task CreateGroup_ValidInput_ReturnsGroup()
        {
            var user = new User
            {
                UserId = 1,
                Username = "testUser"
            };

            var group = new Group
            {
                GroupId = 1,
                Name = "testGroup1",
                Creator = user,
                Users = new List<User>
                {
                    user
                }
            };

            var result = await _groupsService.CreateGroup(group);
            Assert.That(result, Is.EqualTo(group));
        }

        [Test]
        [Category("RemoveGroup")]
        [Category("InvalidInput")]
        public void RemoveGroup_GroupDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<GroupNotFoundException>(() => _groupsService.RemoveGroup(2));
        }

        [Test]
        [Category("RemoveGroup")]
        [Category("ValidInput")]
        public void RemoveGroup_ValidInput_DoesNotThrow()
        {
            Assert.DoesNotThrowAsync(async() => await _groupsService.RemoveGroup(1));
        }

        [Test]
        [Category("IsUserInGroup")]
        [Category("InvalidInput")]
        public void IsUserInGroup_GroupDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<GroupNotFoundException>(() => _groupsService.IsUserInGroup(1, 2));
        }

        [Test]
        [Category("IsUserInGroup")]
        [Category("ValidInput")]
        public async Task IsUserInGroup_ValidInput_ReturnsTrue()
        {
            var result = await _groupsService.IsUserInGroup(1, 1);
            Assert.That(result, Is.True);
        }

        [Test]
        [Category("IsUserInGroup")]
        [Category("ValidInput")]
        public async Task IsUserInGroup_ValidInput_ReturnsFalse()
        {
            var result = await _groupsService.IsUserInGroup(3, 1);
            Assert.That(result, Is.False);
        }

        [Test]
        [Category("GetGroupById")]
        [Category("InvalidInput")]
        public void GetGroupById_GroupDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<GroupNotFoundException>(() => _groupsService.GetGroupById(2));
        }

        [Test]
        [Category("GetGroupById")]
        [Category("ValidInput")]
        public async Task GetGroupById_ValidInput_ReturnsGroup()
        {
            var result = await _groupsService.GetGroupById(1);
            Assert.That(result.GroupId, Is.EqualTo(1));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("InvalidInput")]
        public void UpdateGroup_InvalidNewGroupName_ThrowsException()
        {
            var group = new Group
            {
                GroupId = 1,
                Name = "asd",
                Creator = new User()
            };

            Assert.ThrowsAsync<InvalidInputDataException>(() => _groupsService.UpdateGroup(group));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("InvalidInput")]
        public void UpdateGroup_GroupDoesNotExist_ThrowsException()
        {
            var groupDto = new CreateOrUpdateGroupDto("goodName");
            Assert.ThrowsAsync<GroupNotFoundException>(() => _groupsService.UpdateGroup(groupDto, -1));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("InvalidInput")]
        public void UpdateGroup_InvalidNameInDto_ThrowsException()
        {
            var groupDto = new CreateOrUpdateGroupDto("asd");
            Assert.ThrowsAsync<InvalidInputDataException>(() => _groupsService.UpdateGroup(groupDto, 1));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("ValidInput")]
        public async Task UpdateGroup_ValidInput_ReturnsGroup()
        {
            var groupDto = new CreateOrUpdateGroupDto("goodName");
            var result = await _groupsService.UpdateGroup(groupDto, 1);
            Assert.That(result.Name, Is.EqualTo("goodName"));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("InvalidInput")]
        public void CreateGroupJoinRequest_GroupDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<GroupNotFoundException>(() => _groupsService.CreateGroupJoinRequest(2,1,1));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("InvalidInput")]
        public void CreateGroupJoinRequest_UserDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _groupsService.CreateGroupJoinRequest(1,-1, 1));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("InvalidInput")]
        public void CreateGroupJoinRequest_CreatorDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<UserNotFoundException>(() => _groupsService.CreateGroupJoinRequest(1, 1, -1));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("InvalidInput")]
        public void CreateGroupJoinRequest_UserAlreadyPartOfGroup_ThrowsException()
        {
            Assert.ThrowsAsync<InvalidInputDataException>(() => _groupsService.CreateGroupJoinRequest(1, 2, 1));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("InvalidInput")]
        public void CreateGroupJoinRequest_NonCreatorTriesToInvite_ThrowsException()
        {
            Assert.ThrowsAsync<InvalidInputDataException>(() => _groupsService.CreateGroupJoinRequest(1, 3, 2));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("InvalidInput")]
        public void CreateGroupJoinRequest_GroupJoinRequestAlreadyExists_ThrowsException()
        {
            _groupsRepositoryMock.Setup(x => x.GetGroupJoinRequest(3, 1)).ReturnsAsync(new GroupJoinRequest());
            Assert.ThrowsAsync<InvalidInputDataException>(() => _groupsService.CreateGroupJoinRequest(1, 3, 1));
        }

        [Test]
        [Category("CreateGroupJoinRequest")]
        [Category("ValidInput")]
        public async Task CreateGroupJoinRequest_ValidInput_ReturnsGroupJoinRequest()
        {
            var result = await _groupsService.CreateGroupJoinRequest(1, 3, 1);
            Assert.That(result.GroupId, Is.EqualTo(1));
            Assert.That(result.UserId, Is.EqualTo(3));
        }

        [Test]
        [Category("RemoveGroupJoinRequest")]
        [Category("InvalidInput")]
        public void RemoveGroupJoinRequest_GroupJoinRequestDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<GroupJoinRequestNotFoundException>(() => _groupsService.RemoveGroupJoinRequest(1, 3));
        }

        [Test]
        [Category("RemoveGroupJoinRequest")]
        [Category("ValidInput")]
        public void RemoveGroupJoinRequest_ValidInput_DoesNotThrow()
        {
            _groupsRepositoryMock.Setup(x => x.GetGroupJoinRequest(3, 1)).ReturnsAsync(new GroupJoinRequest());
            Assert.DoesNotThrowAsync(async() => await _groupsService.RemoveGroupJoinRequest(1, 3));
        }

        [Test]
        [Category("AcceptGroupJoinRequest")]
        [Category("InvalidInput")]
        public void AcceptGroupJoinRequest_GroupJoinRequestDoesNotExist_ThrowsException()
        {
            Assert.ThrowsAsync<GroupJoinRequestNotFoundException>(() => _groupsService.AcceptGroupJoinRequest(1, 3));
        }

        [Test]
        [Category("AcceptGroupJoinRequest")]
        [Category("InvalidInput")]
        public void AcceptGroupJoinRequest_GroupJoinRequestExpired_ThrowsException()
        {
            _groupsRepositoryMock.Setup(x => x.GetGroupJoinRequest(3, 1)).ReturnsAsync(new GroupJoinRequest
            {
                CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                ExpiresAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                GroupId = 1,
                UserId = 3
            });
            Assert.ThrowsAsync<InvalidInputDataException>(() => _groupsService.AcceptGroupJoinRequest(1, 3));
        }

        [Test]
        [Category("AcceptGroupJoinRequest")]
        [Category("ValidInput")]
        public async Task AcceptGroupJoinRequest_ValidInput_DoesNotThrow()
        {
            _groupsRepositoryMock.Setup(x => x.GetGroupJoinRequest(3, 1)).ReturnsAsync(new GroupJoinRequest
            {
                CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                GroupId = 1,
                UserId = 3
            });
            
            Assert.DoesNotThrowAsync(async () => await _groupsService.AcceptGroupJoinRequest(1, 3));
        }

        [Test]
        [Category("GetAllGroupJoinRequestsForGroup")]
        [Category("ValidInput")]
        public async Task GetAllGroupJoinRequestsForGroup_ValidInput_ReturnsGroupJoinRequests_WithExpiredRemoved()
        {
            var creator = new User()
            {
                UserId = 1,
                Username = "creator"
            };
            _groupsRepositoryMock.Setup(x => x.GetAllGroupJoinRequestsForGroup(1)).ReturnsAsync(new List<GroupJoinRequest>
            {
                new()
                {
                    CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                    ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                    GroupId = 1,
                    UserId = 3,
                    Creator = creator
                },
                new()
                {
                    CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                    ExpiresAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    GroupId = 1,
                    UserId = 2,
                    Creator = creator
                }
            });
            var result = await _groupsService.GetAllGroupJoinRequestsForGroup(1);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        [Category("GetAllGroupJoinRequestsForUser")]
        [Category("ValidInput")]
        public async Task GetAllGroupJoinRequestsForUser_ValidInput_ReturnsGroupJoinRequests_WithExpiredRemoved()
        {
            var creator = new User()
            {
                UserId = 1,
                Username = "creator"
            };
            _groupsRepositoryMock.Setup(x => x.GetAllGroupJoinRequestsForUser(3)).ReturnsAsync(new List<GroupJoinRequest>
            {
                new()
                {
                    CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                    ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                    GroupId = 1,
                    UserId = 3,
                    Creator = creator
                },
                new()
                {
                    CreatedAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(3)),
                    ExpiresAt = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    GroupId = 2,
                    UserId = 3,
                    Creator = creator
                }
            });
            var result = await _groupsService.GetAllGroupJoinRequestsForUser(3);
            Assert.That(result.Count, Is.EqualTo(1));
        }

    }
}
