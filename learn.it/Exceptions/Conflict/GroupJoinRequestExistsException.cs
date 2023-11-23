namespace learn.it.Exceptions.Conflict
{
    public class GroupJoinRequestExistsException : AlreadyExistsException
    {
        public GroupJoinRequestExistsException(string userId, string groupId) : base($"A group join request/invitation for user id [{userId}] and group id [{groupId}] already exists.")
        {
        }
    }
}
