namespace learn.it.Exceptions.NotFound
{
    public class AchievementNotFoundException : NotFoundException
    {
        public AchievementNotFoundException(int id) : base($"Achievement with id {id} was not found")
        {
        }
    }
}
