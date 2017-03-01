using Dapper;
using NotifManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace NotifManager.Repositories
{
	public class AppRepository : BaseRepository<Models.App>
	{
		public AppRepository(string tablePrefix = "") : base(tablePrefix)
		{ }

        public virtual IEnumerable<App> GetAppsByClient(Guid Id)
        {
            IEnumerable<App> result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(App));

                result = conexaoBD.Query<App>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(App)) + " where active = 1 AND ClientId = @Id", new { Id });
            }

            return result;
        }
    }
}
