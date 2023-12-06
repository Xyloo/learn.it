namespace learn.it.Exceptions.Conflict
{
    public class UsernameExistsException : AlreadyExistsException
    {
        public UsernameExistsException() : base("Użytkownik o takiej nazwie już istnieje.")
        {
        }
    }
}
