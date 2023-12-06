namespace learn.it.Exceptions.NotFound
{
    public class GroupNotFoundException : NotFoundException
    {
        public GroupNotFoundException(string id) : base($"Nie odnaleziono grupy o id [{id}].")
        {
        }
    }
}
