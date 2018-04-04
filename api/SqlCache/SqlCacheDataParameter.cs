using System;
using System.Data;

namespace SqlCache
{
    public class SqlCacheDataParameter : IDbDataParameter
    {
        public byte Precision { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte Scale { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Size { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbType DbType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ParameterDirection Direction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsNullable => throw new NotImplementedException();

        public string ParameterName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SourceColumn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DataRowVersion SourceVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object Value { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
