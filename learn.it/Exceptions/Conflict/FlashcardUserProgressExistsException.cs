namespace learn.it.Exceptions.Conflict
{
    public class FlashcardUserProgressExistsException : AlreadyExistsException
    {
        public FlashcardUserProgressExistsException(string? message) : base(message) { }
    }
}
