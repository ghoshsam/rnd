using DataAccessWithDapper;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DapperOrm
{
    public class ConnectionFactory : IConnectionFactory<DbConnection>
    {
        private readonly string _connectionString;
        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }       
        public DbConnection CreateDatabase()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
