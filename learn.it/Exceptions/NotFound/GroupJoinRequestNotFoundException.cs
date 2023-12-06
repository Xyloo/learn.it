namespace learn.it.Exceptions.NotFound
{
    public class GroupJoinRequestNotFoundException : NotFoundException
    {
        public GroupJoinRequestNotFoundException(string userId, string groupId) : base($"Nie odnaleziono zaproszenia/prośby o dołączenie dla użytkownika o id [{userId}] oraz id grupy [{groupId}].")
        {
        }

        public GroupJoinRequestNotFoundException(string message) : base(message)
        {
        }
    }
}
