using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace NotifManager.Repositories
{
    public abstract class BaseRepository<T> : IDisposable
    {
        public string tablePrefix;

        public BaseRepository(string tablePrefix = "")
        {
            this.tablePrefix = tablePrefix;
        }

        protected IEnumerable<string> GetProperties(Type t)
        {
            return t.GetProperties().Select(x => x.Name);
        }

        protected String GetTableName(Type t)
        {
            return tablePrefix + t.Name;
        }

        public virtual IEnumerable<T> GetData<T>(Boolean all = true)
        {
            IEnumerable<T> result = new List<T>();
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                result = conexaoBD.Query<T>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(T)) +
                    (all ? "" : " where active = 1"));
            }

            return result;
        }

        public virtual T GetData<T>(Guid Id)
        {
            T result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                result = conexaoBD.Query<T>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(T)) + " where active = 1 AND Id = @Id", new { Id = Id.ToString() }).SingleOrDefault();
            }

            return result;
        }

        public virtual IEnumerable<T> GetData<T>(IEnumerable<Guid> Id)
        {
            IEnumerable<T> result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                result = conexaoBD.Query<T>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(T)) + " where active = 1 AND Id in @Id" , new { Id });
            }

            return result;
        }

        public virtual IEnumerable<T> GetData<T>(IEnumerable<Guid> Id, bool all)
        {
            IEnumerable<T> result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                result = conexaoBD.Query<T>("select " + string.Join(",", properties) + " from " + GetTableName(typeof(T)) + " where Id in @Id" + (all ? "" : " And active = 1"), new { Id });
            }

            return result;
        }

        public virtual bool ExistsData<T>(Guid Id)
        {
            int result;
            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                result = conexaoBD.ExecuteScalar<int>("select 1 from " + GetTableName(typeof(T)) + " where Id = @Id", new { Id = Id.ToString() });
            }

            return result.Equals(1);
        }

        public virtual bool PostData<T>(T obj)
        {
            int result = 0;

            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                string query = "insert into " + GetTableName(typeof(T)) + " (" + string.Join(",", properties) +
                    ") values (" + string.Join(",", properties.Select(x => string.Format("@{0}", x))) + ")";

                result = conexaoBD.Execute(query, obj);
            }

            return result > 0;
        }

        public virtual bool PutData<T>(T obj)
        {
            int result = 0;

            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));

                string query = "update " + GetTableName(typeof(T)) + " set " + string.Join(",", properties.Select(x => string.Format("{0} = @{1}", x, x))) +
                   " where Id = @Id";

                result = conexaoBD.Execute(query, obj);
            }

            return result > 0;
        }

        public virtual bool DeleteData<T>(Guid Id)
        {
            int result = 0;

            using (var conexaoBD = new SqlConnection(ConfigurationManager.ConnectionStrings["JMContext"].ConnectionString))
            {
                IEnumerable<string> properties = GetProperties(typeof(T));
              
                string query = "update " + GetTableName(typeof(T)) + " set active = 0 where Id = '" + Id.ToString() + "'";

                result = conexaoBD.Execute(query);
            }

            return result > 0;
        }

        public void Dispose()
        {
            //this.Dispose();
        }
    }
}