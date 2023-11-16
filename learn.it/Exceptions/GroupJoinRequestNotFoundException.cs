namespace learn.it.Exceptions
{
    public class GroupJoinRequestNotFoundException : Exception
    {
        public GroupJoinRequestNotFoundException(string userId, string groupId) : base($"No group join request/invitation was found for the pair of user id: [{userId}] and group id: [{groupId}]")
        {
        }
    }
}
