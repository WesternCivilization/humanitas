using System;
using System.Data;

namespace SqlCache
{
    public class SqlCacheCommand : IDbCommand
    {

        public SqlCacheCommand()
        {
            this.Parameters = new SqlCacheDataParameterCollection();
        }

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDataParameterCollection Parameters { get; }

        public UpdateRowSource UpdatedRowSource { get; set; }

        public void Cancel()
        {
        }

        public IDbDataParameter CreateParameter()
        {
            return new SqlCacheDataParameter();
        }

        public void Dispose()
        {
        }

        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader()
        {
            var manager = ((SqlCacheConnection)this.Connection)._server;
            return new SqlCacheDataReader(this.CommandText, manager);
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            var manager = ((SqlCacheConnection)this.Connection)._server;
            var result = new SqlCacheDataReader(this.CommandText, manager);
            return result;
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }
    }
}
