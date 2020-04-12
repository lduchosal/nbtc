using System;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;

namespace Nbtc.NodeWalker
{
    public sealed class ConnectionFactory
    {
        private readonly string _nodeConnection;
        public ConnectionFactory(ConnectionStringSettingsCollection connections)
        {
            _nodeConnection = connections["node.sqlite"].ConnectionString;
        }
        public DbConnection Node()
        {
            return new SQLiteConnection(_nodeConnection);
        }
    }
}