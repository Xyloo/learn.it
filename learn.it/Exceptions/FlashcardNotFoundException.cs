namespace learn.it.Exceptions
{
    public class FlashcardNotFoundException : Exception
    {
        public FlashcardNotFoundException(int flashcardId) : base($"Flashcard with id {flashcardId} was not found")
        {
        }
    }
}
