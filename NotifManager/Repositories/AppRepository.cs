namespace NotifManager.Repositories
{
	public class AppRepository : BaseRepository<Models.App>
	{
		public AppRepository(string tablePrefix = "") : base(tablePrefix)
		{ }
	}
}
