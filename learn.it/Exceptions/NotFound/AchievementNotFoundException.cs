namespace learn.it.Exceptions.NotFound
{
    public class AchievementNotFoundException : NotFoundException
    {
        public AchievementNotFoundException(int id) : base($"Nie odnaleziono osiągnięcia o id [{id}].")
        {
        }
    }
}
