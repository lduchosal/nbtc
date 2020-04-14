using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dapper;
using Nbtc.Util;

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

            _logger.Trace("Init");
            string sqlcreate = @"

                CREATE TABLE IF NOT EXISTS node (
                    id INTEGER PRIMARY KEY,
                    ip VARCHAR(64) UNIQUE NOT NULL,
                    port INTEGER NOT NULL,
                    useragent VARCHAR(256) NULL,
                    src VARCHAR(64) NULL,
                    srcid INTEGER NULL,
                    srctype INTEGER NOT NULL,
                    creation DATETIME NOT NULL,
                    updated DATETIME NOT NULL,
                    status INTEGER NOT NULL,
                    FOREIGN KEY(srcid) REFERENCES node(id)
                )
                ;
";
            _logger.Trace("Init {sqlcreate}", sqlcreate);

            var conn = _conn.Node();
            conn.Execute(sqlcreate);
        }



        public void Insert(
            IEnumerable<(IPAddress, ushort)> hosts,
            string src,
            SourceTypeEnum srctype,
            uint? identifier = null)
        {
            
            _logger.Trace("Insert {@log}]", new { identifier, src, hosts = hosts.Count()});

            string sqlinsert = @"

                INSERT OR IGNORE
                    INTO node (ip, port, src, srcid, srctype, creation, updated, status) 
                VALUES (@ip, @port, @src, @srcid, @srctype, @creation, @updated, @status)
                
                    ;
            ";

            string sqlupdate = @"

                UPDATE node 
                    SET updated = @updated, 
                        status = @status
                WHERE id = @id
                    ;
            ";

            _logger.Trace("Insert {@sqlinsert}", new { sqlinsert, sqlupdate});

            var now = DateTime.Now;
            
            var inserts = hosts.Select((item, i) =>
                new Node
                {
                    Ip = item.Item1.ToString(),
                    Port = item.Item2,
                    Src = src,
                    SrcId = identifier,
                    SrcType = srctype,
                    Creation = now,
                    Updated = now,
                    Status = StatusEnum.New,
                });

            var update = new
            {
                updated = now,
                status = StatusEnum.Done,
                id = identifier
            };
            var conn = _conn.Node();
            int icount = conn.Execute(sqlinsert, inserts);
            int ucount = conn.Execute(sqlupdate, update);
            
            _logger.Debug("Insert {@count}", new { src, identifier, icount, ucount });
        }

        public void Delete(uint identifier)
        {
            UpdateStatus(identifier, StatusEnum.Deleted);
        }

        public void Deactivate(uint identifier)
        {
            UpdateStatus(identifier, StatusEnum.Deactivate);
        }
        
        private void UpdateStatus(uint identifier, StatusEnum status)
        {
            _logger.Trace("UpdateStatus {@data}]", new { identifier, status });
            string sqlupdate = @"

                UPDATE node 
                    SET updated = @updated, 
                        status = @status
                WHERE id = @id
                    ;

            ";
            _logger.Trace("BulkInsert [sqlupdate: {sqlupdate}]", sqlupdate);

            var now = DateTime.Now;
            var update = new
            {
                updated = now,
                status = status,
                id = identifier
            };
            
            var conn = _conn.Node();
            int updated = conn.Execute(sqlupdate, update);
            
            _logger.Trace("UpdateStatus {@count}", new { identifier, status, updated });

        }
        
        
        public void UserAgent(uint identifier, string useragent)
        {
            _logger.Trace("UserAgent {@version}", new { identifier, useragent });
            string sqlupdate = @"

                UPDATE node 
                    SET useragent = @useragent,
                        updated = @updated
                WHERE id = @id
                    ;

            ";
            _logger.Trace("UserAgent {sqlupdate}", sqlupdate);

            var now = DateTime.Now;
            var update = new
            {
                updated = now,
                useragent = useragent,
                id = identifier
            };
            
            var conn = _conn.Node();
            conn.Execute(sqlupdate, update);
        }

        public IEnumerable<Node> Select(StatusEnum status, int limit)
        {
            _logger.Trace("Select {@version}", new { status, limit });
            
            string sqlselect = @"

                 SELECT id, ip, port, src, srctype, creation, updated, status
                   FROM node
                  WHERE status = @status
               ORDER BY id DESC
                  LIMIT @limit
                    ;

            ";
            _logger.Trace("Select {sqlselect}", sqlselect);

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

