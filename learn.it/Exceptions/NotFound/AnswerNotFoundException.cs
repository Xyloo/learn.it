namespace learn.it.Exceptions.NotFound
{
    public class AnswerNotFoundException : NotFoundException
    {
        public AnswerNotFoundException(int id) : base($"Nie odnaleziono odpowiedzi o id [{id}].")
        {
        }
    }
}
