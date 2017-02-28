using Dapper;
using NotifManager.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace NotifManager.Repositories
{
	public class ClientRepository : BaseRepository<Models.Client>
	{
		public ClientRepository(string tablePrefix = "") : base(tablePrefix)
		{ }

        public virtual Client GetClientByEmail(string email)
        {
            Client result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(Client));

                result = conexaoBD.Query<Client>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(Client)) + " where active = 1 AND Email = @Email", new { email }).SingleOrDefault();
            }

            return result;
        }
    }
}
