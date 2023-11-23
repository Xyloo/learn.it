namespace learn.it.Exceptions.Conflict
{
    public class EmailExistsException : AlreadyExistsException
    {
        public EmailExistsException() : base("Email already exists.")
        {
        }
    }
}
