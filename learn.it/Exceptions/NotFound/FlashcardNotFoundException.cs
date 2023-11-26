namespace learn.it.Exceptions.NotFound
{
    public class FlashcardNotFoundException : NotFoundException
    {
        public FlashcardNotFoundException(int flashcardId) : base($"Flashcard with id {flashcardId} was not found")
        {
        }
    }
}
