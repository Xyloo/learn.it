namespace learn.it.Exceptions.NotFound
{
    public class StudySetNotFoundException : NotFoundException
    {
        public StudySetNotFoundException(string idOrName) : base($"Nie odnaleziono zestawu o id lub nazwie [{idOrName}].")
        {
        }

        public StudySetNotFoundException(int id) : base($"Nie odnaleziono zestawu o id [{id}].")
        {
        }
    }
}
