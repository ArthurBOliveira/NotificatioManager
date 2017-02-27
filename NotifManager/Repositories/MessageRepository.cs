namespace NotifManager.Repositories
{
	public class MessageRepository : BaseRepository<Models.Message>
	{
		public MessageRepository(string tablePrefix = "") : base(tablePrefix)
		{ }
	}
}
