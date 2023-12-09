namespace learn.it.Exceptions.Conflict
{
    public class EmailExistsException : AlreadyExistsException
    {
        public EmailExistsException() : base("Istnieje już użytkownik o takim adresie email.")
        {
        }
    }
}
