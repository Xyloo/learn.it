using learn.it.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using learn.it.Models.Dtos.Request;
using learn.it.Models.Dtos.Response;
using learn.it.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace learn.it.Tests.IntegrationTests
{
    public class GroupsControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private List<User> _users;
        private List<Group> _groups;
        private LearnitDbContext _db;

        [SetUp]
        public void Init()
        {
            _factory = new TestWebApplicationFactory<Program>();
            _db = _factory.Services.CreateScope().ServiceProvider
                .GetRequiredService<LearnitDbContext>();
            var data = DatabaseSeeder.Prepare(_db);
            _users = data.Users;
            _groups = data.Groups;
        }

        [Test]
        [Category("GetAllGroups")]
        [Category("Admin")]
        public async Task GetAllGroups_WithAdminUser_ShouldReturnAllGroups()
        {
            // Arrange
            var client = _factory.CreateClient();
            var admin = _users[0];
            var token = await Utilities.LoginUser(admin, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/groups");
            var groups = await response.Content.ReadFromJsonAsync<List<Group>>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(groups, Has.Count.EqualTo(_groups.Count));
        }

        [Test]
        [Category("GetAllGroups")]
        [Category("User")]
        public async Task GetAllGroups_WithUser_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync("/api/groups");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("GetGroupDetails")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task GetGroupDetails_WithAdminUser_ShouldReturnAnyGroupDetailedDto()
        {
            // Arrange
            var client = _factory.CreateClient();
            var admin = _users[0];
            var token = await Utilities.LoginUser(admin, client);
            var group = _groups[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{group.GroupId}");
            var groupDetails = await response.Content.ReadFromJsonAsync<GroupDto>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(groupDetails.GroupId, Is.EqualTo(group.GroupId));
            Assert.That(groupDetails.Name, Is.EqualTo(group.Name));
        }

        [Test]
        [Category("GetGroupDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetGroupDetails_WithUser_ShouldReturnDetailedDto_IfCreator()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);
            var group = _groups[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{group.GroupId}");
            var groupDetails = await response.Content.ReadFromJsonAsync<GroupDto>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(groupDetails.GroupId, Is.EqualTo(group.GroupId));
            Assert.That(groupDetails.Name, Is.EqualTo(group.Name));
        }

        [Test]
        [Category("GetGroupDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetGroupDetails_WithUser_ShouldReturnDetailedDto_IfPartOfGroup()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var group = _groups[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{group.GroupId}");
            var groupDetails = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.Multiple(() =>
            {

                // Assert
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(groupDetails.GroupId, Is.EqualTo(group.GroupId));
                Assert.That(groupDetails.Name, Is.EqualTo(group.Name));
            });
        }

        [Test]
        [Category("GetGroupDetails")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetGroupDetails_WithUser_ShouldReturnBasicDto_IfNotPartOfGroup()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var group = _groups[0];

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{group.GroupId}");
            var groupDetails = await response.Content.ReadFromJsonAsync<BasicGroupDto>();
            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(groupDetails.GroupId, Is.EqualTo(group.GroupId));
                Assert.That(groupDetails.Name, Is.EqualTo(group.Name));
            });
        }

        [Test]
        [Category("GetGroupDetails")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetGroupDetails_WithUser_ShouldReturnNotFound_IfGroupDoesntExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups.Count+1}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("FindGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task FindGroup_WithUser_ShouldReturnBasicDtos()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/find/testGroup");
            var groupDetails = await response.Content.ReadFromJsonAsync<List<BasicGroupDto>>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(groupDetails, Has.Count.EqualTo(2));
        }

        [Test]
        [Category("FindGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task FindGroup_WithUser_NoMatchingValue_ShouldReturnEmptyList()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/find/x");
            var str = await response.Content.ReadAsStringAsync();
            var groupDetails = await response.Content.ReadFromJsonAsync<List<BasicGroupDto>>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(groupDetails, Has.Count.EqualTo(0));
        }

        [Test]
        [Category("CreateGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateGroup_WithUser_ShouldReturnCreatedGroup()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var group = new CreateOrUpdateGroupDto("testGroup");

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/groups", group);
            var createdGroup = await response.Content.ReadFromJsonAsync<GroupDto>();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(createdGroup.Name, Is.EqualTo(group.Name));
        }

        [Test]
        [Category("CreateGroup")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateGroup_WithUser_ShouldReturnBadRequest_IfNameIsTooLong()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var group = new CreateOrUpdateGroupDto(new string('x', 151));

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/groups", group);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateGroup")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateGroup_WithUser_ShouldReturnBadRequest_IfNameIsTooShort()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);
            var group = new CreateOrUpdateGroupDto("x");

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.PostAsJsonAsync("/api/groups", group);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateJoinRequest")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateJoinRequest_IfUserNotPartOfGroup_ShouldReturnCreatedRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[3];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("CreateJoinRequest")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateJoinRequest_IfUserPartOfGroup_ShouldReturnBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateJoinRequest")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateJoinRequest_IfJoinRequestAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[3];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [Test]
        [Category("CreateInvitation")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task CreateInvitation_IfInviteeNotPartOfGroup_AndRequestingUserIsCreator()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[0].UserId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        [Category("CreateInvitation")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateInvitation_IfInviteeNotPartOfGroup_AndRequestingUserIsNotCreator()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[2];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[0].UserId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateInvitation")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateInvitation_IfInviteeIsPartOfGroup_ShouldReturnBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[1].UserId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("CreateInvitation")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task CreateInvitation_IfInvitationAlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = _users[1];
            var token = await Utilities.LoginUser(user, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        }

        [Test]
        [Category("AcceptJoinRequest")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task AcceptJoinRequest_IfRequestExists_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user3 = _users[3];
            var token3 = await Utilities.LoginUser(user3, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token3}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join/{_users[3].UserId}/accept");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(3));
            Assert.That(group.Users.Any(u => u.Username == _users[3].Username), Is.True);
        }

        [Test]
        [Category("AcceptInvitation")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task AcceptInvitation_IfInvitationExists_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var user3 = _users[3];
            var token3 = await Utilities.LoginUser(user3, client);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token3}");

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/accept");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(3));
            Assert.That(group.Users.Any(u => u.Username == _users[3].Username), Is.True);
        }

        [Test]
        [Category("DeclineJoinRequest")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task DeclineJoinRequest_IfRequestExists_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user3 = _users[3];
            var token3 = await Utilities.LoginUser(user3, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token3}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join/{_users[3].UserId}/decline");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(2));
            Assert.That(group.Users.Any(u => u.Username == _users[3].Username), Is.False);
        }

        [Test]
        [Category("DeclineInvitation")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task DeclineInvitation_IfInvitationExists_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var user3 = _users[3];
            var token3 = await Utilities.LoginUser(user3, client);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token3}");

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/decline");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");
            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(2));
            Assert.That(group.Users.Any(u => u.Username == _users[3].Username), Is.False);
        }

        [Test]
        [Category("LeaveGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task LeaveGroup_IfUserIsMember_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[2];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/leave");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var creator = _users[1];
            var token2 = await Utilities.LoginUser(creator, client);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token2}");
            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(1));
            Assert.That(group.Users.Any(u => u.Username == _users[2].Username), Is.False);
        }

        [Test]
        [Category("LeaveGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task LeaveGroup_IfUserIsCreator_ShouldRemoveGroup()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/leave");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("LeaveGroup")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task LeaveGroup_IfUserIsNotMember_ShouldReturnBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[3];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/leave");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [Category("RemoveUserFromGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task RemoveUserFromGroup_IfUserIsCreator_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/remove/{_users[2].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(1));
            Assert.That(group.Users.Any(u => u.Username == _users[2].Username), Is.False);
        }

        [Test]
        [Category("RemoveUserFromGroup")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task RemoveUserFromGroup_IfUserIsNotCreator_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[2];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/remove/{_users[1].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("RemoveUserFromGroup")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task RemoveUserFromGroup_IfUserIsAdmin_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[0];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/remove/{_users[2].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(1));
            Assert.That(group.Users.Any(u => u.Username == _users[2].Username), Is.False);
        }

        [Test]
        [Category("AddUserToGroup")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task AddUserToGroup_IfUserIsAdmin_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[0];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/add/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var group = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(group.Users, Has.Count.EqualTo(3));
            Assert.That(group.Users.Any(u => u.Username == _users[3].Username), Is.True);
        }

        [Test]
        [Category("AddUserToGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task AddUserToGroup_IfUserIsCreator_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/add/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task UpdateGroup_IfUserIsCreator_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            var group = new CreateOrUpdateGroupDto("Updated Group Name");

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.PutAsJsonAsync($"/api/groups/{_groups[0].GroupId}", group);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var updatedGroup = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(updatedGroup.Name, Is.EqualTo(group.Name));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task UpdateGroup_IfUserIsNotCreator_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[2];
            var token1 = await Utilities.LoginUser(user1, client);

            var group = new CreateOrUpdateGroupDto("Updated Group Name");

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.PutAsJsonAsync($"/api/groups/{_groups[0].GroupId}", group);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("UpdateGroup")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task UpdateGroup_IfUserIsAdmin_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[0];
            var token1 = await Utilities.LoginUser(user1, client);

            var group = new CreateOrUpdateGroupDto("Updated Group Name");

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.PutAsJsonAsync($"/api/groups/{_groups[0].GroupId}", group);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            var updatedGroup = await response.Content.ReadFromJsonAsync<GroupDto>();
            Assert.That(updatedGroup.Name, Is.EqualTo(group.Name));
        }

        [Test]
        [Category("DeleteGroup")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task DeleteGroup_IfUserIsCreator_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.DeleteAsync($"/api/groups/{_groups[0].GroupId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("DeleteGroup")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task DeleteGroup_IfUserIsNotCreator_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[2];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.DeleteAsync($"/api/groups/{_groups[0].GroupId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("DeleteGroup")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task DeleteGroup_IfUserIsAdmin_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[0];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.DeleteAsync($"/api/groups/{_groups[0].GroupId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        [Category("GetGroupJoinRequests")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetGroupJoinRequests_IfUserIsCreator_ShouldReturnOk_WithInvite()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[1];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/invite/{_users[3].UserId}");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join-requests");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var requests = await response.Content.ReadFromJsonAsync<List<GroupJoinRequestDto>>();
            Assert.That(requests, Has.Count.EqualTo(1));
            Assert.That(requests[0].UserId, Is.EqualTo(_users[3].UserId));
        }

        [Test]
        [Category("GetGroupJoinRequests")]
        [Category("User")]
        [Category("ValidInput")]
        public async Task GetGroupJoinRequests_IfUserIsCreator_ShouldReturnOk_WithJoinRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[3];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var creator = _users[1];
            var token2 = await Utilities.LoginUser(creator, client);
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token2}");

            response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join-requests");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var requests = await response.Content.ReadFromJsonAsync<List<GroupJoinRequestDto>>();
            Assert.That(requests, Has.Count.EqualTo(1));
            Assert.That(requests[0].UserId, Is.EqualTo(_users[3].UserId));
        }

        [Test]
        [Category("GetGroupJoinRequests")]
        [Category("User")]
        [Category("InvalidInput")]
        public async Task GetGroupJoinRequests_IfUserIsNotCreator_ShouldReturnForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[2];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join-requests");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        [Category("GetGroupJoinRequests")]
        [Category("Admin")]
        [Category("ValidInput")]
        public async Task GetGroupJoinRequests_IfUserIsAdmin_ShouldReturnOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user1 = _users[0];
            var token1 = await Utilities.LoginUser(user1, client);

            // Act
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token1}");

            var response = await client.GetAsync($"/api/groups/{_groups[0].GroupId}/join-requests");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var requests = await response.Content.ReadFromJsonAsync<List<GroupJoinRequestDto>>();
            Assert.That(requests, Has.Count.EqualTo(0));
        }

    }
}
