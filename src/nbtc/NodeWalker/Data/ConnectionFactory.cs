using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace NodeWalker.Data
{
    public sealed class ConnectionFactory
    {
        private readonly DbConnection _conn;
        public ConnectionFactory()
        {
            var cnnString = ConfigurationManager.ConnectionStrings["node.sqlite"].ConnectionString;
            _conn = new SQLiteConnection(cnnString);
        }
        public DbConnection Node()
        {
            return _conn;
        }
    }
}