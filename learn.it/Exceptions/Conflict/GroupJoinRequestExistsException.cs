namespace learn.it.Exceptions.Conflict
{
    public class GroupJoinRequestExistsException : AlreadyExistsException
    {
        public GroupJoinRequestExistsException(string userId, string groupId) : base($"Zaproszenie/prośba o dołączenie do grupy o id [{groupId}] przez użytkownika [{userId}] już istnieje.")
        {
        }
    }
}
