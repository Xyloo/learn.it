namespace learn.it.Exceptions
{
    public class GroupJoinRequestExistsException : Exception
    {
        public GroupJoinRequestExistsException(string userId, string groupId) : base($"A group join request/invitation for user id [{userId}] and group id [{groupId}] already exists.")
        {
        }
    }
}
