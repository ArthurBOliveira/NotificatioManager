namespace NotifManager.Repositories
{
	public class ClientRepository : BaseRepository<Models.Client>
	{
		public ClientRepository(string tablePrefix = "") : base(tablePrefix)
		{ }
	}
}
