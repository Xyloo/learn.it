namespace learn.it.Exceptions.NotFound
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string id) : base($"Nie znaleziono użytkownika o id lub nazwie użytkownika: [{id}]")
        {
        }
    }
}
