namespace learn.it.Exceptions.NotFound
{
    public class GroupJoinRequestNotFoundException : NotFoundException
    {
        public GroupJoinRequestNotFoundException(string userId, string groupId) : base($"No group join request/invitation was found for the pair of user id: [{userId}] and group id: [{groupId}]")
        {
        }

        public GroupJoinRequestNotFoundException(string message) : base(message)
        {
        }
    }
}
