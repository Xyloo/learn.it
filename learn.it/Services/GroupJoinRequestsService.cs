using learn.it.Exceptions;
using learn.it.Models;
using learn.it.Repos.Interfaces;
using learn.it.Services.Interfaces;

namespace learn.it.Services
{
    public class GroupJoinRequestsService : IGroupJoinRequestsService
    {
        private readonly IGroupJoinRequestsRepository _groupJoinRequestsRepository;
        private readonly IGroupsRepository _groupsRepository;
        private readonly IUsersRepository _usersRepository;

        public GroupJoinRequestsService(IGroupJoinRequestsRepository groupJoinRequestsRepository, IGroupsRepository groupsRepository, IUsersRepository usersRepository)
        {
            _groupJoinRequestsRepository = groupJoinRequestsRepository;
            _groupsRepository = groupsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<GroupJoinRequest> CreateGroupJoinRequest(GroupJoinRequest groupJoinRequest)
        {
            if (await _groupsRepository.GetGroupById(groupJoinRequest.GroupId) == null)
            {
                throw new GroupNotFoundException(groupJoinRequest.GroupId.ToString());
            }

            if (await _usersRepository.GetUserById(groupJoinRequest.UserId) == null)
            {
                throw new UserNotFoundException(groupJoinRequest.UserId.ToString());
            }

            var groupJoinRequests = await _groupJoinRequestsRepository.GetAllGroupJoinRequestsForGroup(groupJoinRequest.GroupId);
            if (groupJoinRequests.Any(g => g.UserId == groupJoinRequest.UserId))
            {
                throw new GroupJoinRequestExistsException(groupJoinRequest.UserId.ToString(), groupJoinRequest.GroupId.ToString());
            }
            var newGroupJoinRequest = await _groupJoinRequestsRepository.CreateGroupJoinRequest(groupJoinRequest);
            return newGroupJoinRequest;
        }

        public async Task DeleteGroupJoinRequest(GroupJoinRequest groupJoinRequest)
        {
            var requests = await _groupJoinRequestsRepository.GetAllGroupJoinRequestsForGroup(groupJoinRequest.GroupId);
            if (requests.All(g => g.UserId != groupJoinRequest.UserId))
            {
                throw new GroupJoinRequestNotFoundException(groupJoinRequest.UserId.ToString(), groupJoinRequest.GroupId.ToString());
            }
            await _groupJoinRequestsRepository.DeleteGroupJoinRequest(groupJoinRequest);
        }

        public async Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForGroup(int groupId)
        {
            return await _groupJoinRequestsRepository.GetAllGroupJoinRequestsForGroup(groupId);
        }

        public async Task<IEnumerable<GroupJoinRequest>> GetAllGroupJoinRequestsForUser(int userId)
        {
            return await _groupJoinRequestsRepository.GetAllGroupJoinRequestsForUser(userId);
        }
    }
}
