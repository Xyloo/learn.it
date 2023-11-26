namespace learn.it.Exceptions.Conflict
{
    public class UsernameExistsException : AlreadyExistsException
    {
        public UsernameExistsException() : base("Username already exists.")
        {
        }
    }
}
