namespace learn.it.Exceptions.NotFound
{
    public class FlashcardUserProgressNotFoundException : NotFoundException
    {
        public FlashcardUserProgressNotFoundException(string? message) : base(message) { }
        public FlashcardUserProgressNotFoundException(int userId, int flashcardId) : base($"FlashcardUserProgress with userId: {userId} and flashcardId: {flashcardId} was not found.") { }

    }
}
