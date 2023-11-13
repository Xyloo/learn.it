namespace learn.it.Exceptions
{
    public class EmailExistsException : Exception
    {
        public EmailExistsException() : base("Email already exists.")
        {
        }
    }
}
