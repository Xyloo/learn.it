namespace learn.it.Exceptions
{
    public class UsernameExistsException : Exception
    {
        public UsernameExistsException() : base("Username already exists.")
        {
        }
    }
}
