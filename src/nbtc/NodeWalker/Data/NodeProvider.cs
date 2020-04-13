using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Dapper;
using Nbtc.Util;
using IsolationLevel = System.Data.IsolationLevel;

namespace NodeWalker.Data
{
    public sealed class NodeProvider
    {
        private readonly ConnectionFactory _conn;
        private readonly ILogger _logger;

        public NodeProvider(ILogger logger, ConnectionFactory conn)
        {
            _logger = logger.For<NodeProvider>();
            _conn = conn;
            Init();
        }

        public void Init()
        {

            _logger.Debug("Init");
            string sqlcreate = @"

                CREATE TABLE IF NOT EXISTS node (
                    id INTEGER PRIMARY KEY,
                    ip VARCHAR(64) UNIQUE NOT NULL,
                    port INTEGER NOT NULL,
                    src VARCHAR(64) NOT NULL,
                    creation DATETIME NOT NULL,
                    updated DATETIME NOT NULL,
                    status INTEGER NOT NULL
                    )
                    ;
";
            _logger.Debug("Init [sqlcreate: {sqlcreate}]", sqlcreate);

            var conn = _conn.Node();
            conn.Execute(sqlcreate);
        }


        public enum StatusEnum
        {
            New = 0,
            Valid = 1,
            Deleted = 2,
            Deactivate = 4,
        }

        public void BulkInsert(
            IEnumerable<(string, int)> hosts,
            string src,
            uint identifier)
        {
            _logger.Debug("BulkInsert [identifier: {identifier}]", identifier);
            _logger.Debug("BulkInsert [src: {src}]", src);
            _logger.Debug("BulkInsert [hosts: {hosts}]", hosts);

            string sqlinsert = @"

                INSERT OR IGNORE
                    INTO node (ip, port, src, creation, updated, status) 
                VALUES (@ip, @port, @src, @creation, @updated, @status)
                
                    ;
            ";

            string sqlupdate = @"

                UPDATE node 
                    SET updated = @updated, 
                        status = @status
                WHERE id = @id
                    ;
            ";

            _logger.Debug("BulkInsert [sqlinsert: {sqlinsert}]", sqlinsert);
            _logger.Debug("BulkInsert [sqlupdate: {sqlupdate}]", sqlupdate);

            var now = DateTime.Now;

            var inserts = hosts.Select(i =>
                new
                {
                    ip = i.Item1,
                    port = i.Item2,
                    src = src,
                    creation = now,
                    updated = now,
                    status = StatusEnum.New,
                });

            var update = new
            {
                updated = now,
                status = StatusEnum.Valid,
                id = identifier
            };
            var conn = _conn.Node();
            int icount = conn.Execute(sqlinsert, inserts);
            _logger.Debug("BulkInsert [icount: {icount}]", icount);
            int ucount = conn.Execute(sqlupdate, update);
            _logger.Debug("BulkInsert [ucount: {ucount}]", icount);
        }

        public void Delete(int identifier)
        {
            _logger.Debug("Delete [identifier: {identifier}]", identifier);
            UpdateStatus(identifier, StatusEnum.Deleted);
        }

        public void Deactivate(int identifier)
        {
            _logger.Debug("Deactivate [identifier: {identifier}]", identifier);
            UpdateStatus(identifier, StatusEnum.Deactivate);
        }
        
        private void UpdateStatus(int identifier, StatusEnum status)
        {
            _logger.Debug("UpdateStatus [identifier: {identifier}]", identifier);
            _logger.Debug("UpdateStatus [status: {status}]", status);
            string sqlupdate = @"

                UPDATE node 
                    SET updated = @updated, 
                        status = @status
                WHERE id = @id
                    ;

            ";
            _logger.Debug("BulkInsert [sqlupdate: {sqlupdate}]", sqlupdate);

            var now = DateTime.Now;
            var update = new
            {
                updated = now,
                status = status,
                id = identifier
            };
            
            var conn = _conn.Node();
            conn.Execute(sqlupdate, update);
        }
        
        
        public IEnumerable<Node> Select(StatusEnum status, int limit)
        {
            _logger.Debug("Select [status: {status}]", status);
            _logger.Debug("Select [limit: {limit}]", limit);
            
            string sqlselect = @"

                 SELECT id, ip, port, src, creation, updated, status
                   FROM node
                  WHERE status = @status
               ORDER BY id DESC
                  LIMIT @limit
                    ;

            ";
            _logger.Debug("Select [sqlselect: {sqlselect}]", sqlselect);

            var now = DateTime.Now;
            var select = new
            {
                status = status,
                limit = limit
            };

            var conn = _conn.Node();
            var result = conn.Query<Node>(sqlselect, select);
            return result;
        }

    }
}

