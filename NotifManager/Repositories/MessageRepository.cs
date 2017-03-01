using System;
using System.Collections.Generic;
using NotifManager.Models;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;

namespace NotifManager.Repositories
{
    public class MessageRepository : BaseRepository<Models.Message>
    {
        public MessageRepository(string tablePrefix = "") : base(tablePrefix)
        { }

        public IEnumerable<Message> GetMessagesByApp(IEnumerable<Guid> appsId)
        {
            IEnumerable<Message> result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(Message));

                result = conexaoBD.Query<Message>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(Message)) + " where active = 1 AND AppId in @appsId", new { appsId });
            }
            return result;
        }
    }
}
