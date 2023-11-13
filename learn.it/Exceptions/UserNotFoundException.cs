namespace learn.it.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string id) : base($"User with id or username: [{id}] was not found.")
        {
        }
    }
}
