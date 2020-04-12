using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Dapper;
using Nbtc.Util;

namespace Nbtc.NodeWalker
{
    public sealed class NodeProvider
    {
        private readonly ConnectionFactory _conn;
        private readonly ILogger _logger;

        public NodeProvider(ILogger logger, ConnectionFactory conn)
        {
            _conn = conn;
            _logger = logger.For<NodeProvider>();
        }

        public void Init()
        {

            string sqlcreate = @"

                CREATE TABLE IF NOT EXISTS node (
                    id  INTEGER PRIMARY KEY,
                    ip  VARCHAR(64) UNIQUE NOT NULL,
                    src VARCHAR(64) NOT NULL,
                    creation DATETIME NOT NULL,
                    updated DATETIME NOT NULL,
                    status INTEGER NOT NULL
                    )
                    ;
";

            using var conn = _conn.Node();
            using var trans = new TransactionScope();
            conn.Execute(sqlcreate);
            trans.Complete();
        }


        public enum StatusEnum
        {
            New = 0,
            Valid = 1,
            Deleted = 2,
            Deactivate = 4,
        }

        public void Bulkinsert(
            List<string> ips,
            string src,
            uint identifier)
        {

            string sqlinsert = @"

                INSERT OR IGNORE
                    INTO node (ip, src, creation, updated, status) 
                VALUES (@ip, @src, @creation, @updated, @status)
                
                    ;
            ";

            string sqlupdate = @"

                UPDATE node 
                    SET updated = @updated, 
                        status = @status
                WHERE id = @id
                    ;
            ";

            
            var now = DateTime.Now;

            var inserts = ips.Select(ip =>
                new
                {
                    ip = ip,
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

            using var conn = _conn.Node();
            using var trans = new TransactionScope();
            conn.Execute(sqlinsert, inserts);
            conn.Execute(sqlupdate, update);
            trans.Complete();
            
        }
        
        
        
        public void Delete(int identifier)
        {
            UpdateStatus(identifier, StatusEnum.Deleted);
        }

        public void Deactivate(int identifier)
        {
            UpdateStatus(identifier, StatusEnum.Deactivate);
        }
        
        private void UpdateStatus(int identifier, StatusEnum status)
        {
            string sqlupdate = @"

                UPDATE node 
                    SET updated = @updated, 
                        status = @status
                WHERE id = @id
                    ;

            ";

            var now = DateTime.Now;
            var update = new
            {
                updated = now,
                status = status,
                id = identifier
            };
            
            using var conn = _conn.Node();
            using var trans = new TransactionScope();
            conn.Execute(sqlupdate, update);
            trans.Complete();
        }
        
        
        public IEnumerable<Node> Select(StatusEnum status, int limit)
        {
            string sqlselect = @"

                 SELECT id, ip, src, creation, updated, status
                   FROM node
                  WHERE status = @status
               ORDER BY id DESC
                  LIMIT @limit
                    ;

            ";

            var now = DateTime.Now;
            var select = new
            {
                status = status,
                limit = limit
            };

            using var conn = _conn.Node();
            using var trans = new TransactionScope();
            var result = conn.Query<Node>(sqlselect, select);
            trans.Complete();

            return result;
        }

    }
}

