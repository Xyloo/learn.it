namespace learn.it.Exceptions.NotFound
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string id) : base($"User with id or username: [{id}] was not found.")
        {
        }
    }
}
